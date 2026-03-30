using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player_Movement player;
    [SerializeField] private TutorialDialogue tutorialDialogue;
    [SerializeField] private GameObject npcObject;

    [Header("Camera Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    [Header("Player Opening Move Settings")]
    [SerializeField] private Transform targetPosition; // 플레이어가 걸어갈 목표 위치
    [SerializeField] private float moveSpeed = 3f;     // 컷신 중 플레이어 이동 속도

    [SerializeField] private GameObject tutorialPanel; // UI 전체를 켜고 끌 부모 객체
    [SerializeField] private Text tutorialText;
    [SerializeField] private List<string> tutorialMessages = new List<string>();
    [SerializeField] private Image tutorialImage;      // 스프라이트가 바뀔 이미지 컴포넌트
    [SerializeField] private List<Sprite> tutorialSprites = new List<Sprite>(); // 무한정 추가 가능한 리스트

    private void Start()
    {
        // 처음 시작 시 방향만 설정 (아직 걷기 애니메이션은 켜지 않음)
        if (player != null)
        {
            player.enabled = false;
            Player_Attack attackScript = player.GetComponent<Player_Attack>();
            if (attackScript != null) attackScript.enabled = false;
            PlayerAttribute playerAttributeScript = player.GetComponent<PlayerAttribute>();
            if (playerAttributeScript != null) playerAttributeScript.enabled = false;
            Player_Skill playerSkillScript = player.GetComponent<Player_Skill>();
            if (playerSkillScript != null) playerSkillScript.enabled = false;

            Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
            if (rigid != null)
            {
                rigid.velocity = Vector2.zero;
            }

            player.dirX = 1f;
            player.lastDir = Vector2.right;

            Animator anim = player.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetFloat("DirX", 1f);
                anim.SetFloat("DirY", -1f);
                anim.SetFloat("Direction", 1f);
                anim.SetBool("isMove", false);
            }
        }

        StartCoroutine(TutorialOpeningSequence());
    }

    private IEnumerator TutorialOpeningSequence()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MovePlayerToTarget());
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(CameraShake(shakeDuration, shakeMagnitude));

        // 4~6번 과정을 이 한 줄로 대체 (0번 SO 재생: "으아악!", "살려주세요!")
        yield return StartCoroutine(PlayDialogue(0));

        // 7. 조작법 설명 진입
        StartControlTutorial();
    }

    // 플레이어를 목표 위치로 자연스럽게 걷게 만드는 코루틴
    private IEnumerator MovePlayerToTarget()
    {
        if (targetPosition == null)
        {
            Debug.LogWarning("목표 위치(targetPosition)가 설정되지 않았습니다!");
            yield break;
        }

        Animator anim = player.GetComponent<Animator>();

        // 걷기 애니메이션 켜기
        if (anim != null) anim.SetBool("isMove", true);

        // 목표 위치를 향한 방향 벡터 계산 (애니메이션 방향 설정용)
        Vector3 direction = (targetPosition.position - player.transform.position).normalized;
        if (anim != null)
        {
            anim.SetFloat("DirX", direction.x);
            anim.SetFloat("DirY", direction.y);
        }

        // 플레이어의 현재 위치와 목표 위치의 거리가 0.05f보다 클 때까지 계속 이동
        while (Vector3.Distance(player.transform.position, targetPosition.position) > 0.05f)
        {
            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                targetPosition.position,
                moveSpeed * Time.deltaTime
            );
            yield return null; // 다음 프레임까지 대기
        }

        // 목표 위치에 정확히 안착시키고 걷기 애니메이션 끄기
        player.transform.position = targetPosition.position;
        if (anim != null)
        {
            anim.SetFloat("DirX", 1f);
            anim.SetFloat("DirY", 0f);
            anim.SetBool("isMove", false);
        }
    }

    private IEnumerator WaitForUserInput()
    {
        yield return null;

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                break;
            }
            yield return null;
        }
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }
        Camera.main.transform.localPosition = originalPos;
    }

    private void StartControlTutorial()
    {
        Debug.Log("조작법 튜토리얼 시작!");

        player.enabled = true;
        
        
        //Player_Skill playerSkillScript = player.GetComponent<Player_Skill>();
        //if (playerSkillScript != null) playerSkillScript.enabled = true;        

        StartCoroutine(ControlTutorialSequence());
    }

    // --- 메인 조작 튜토리얼 루프 ---
    private IEnumerator ControlTutorialSequence()
    {
        // --- 1단계: 평타 공격 ---
        if (tutorialSprites.Count > 0)
        {
            // 조작 가이드 UI 설정
            if (tutorialPanel != null) tutorialPanel.SetActive(true);
            tutorialImage.sprite = tutorialSprites[0];
            yield return new WaitForSeconds(0.5f);

            // 공격 스크립트 활성화
            Player_Attack attackScript = player.GetComponent<Player_Attack>();
            if (attackScript != null) attackScript.enabled = true;

            // 유저가 실제로 공격(좌클릭)할 때까지 대기
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            Debug.Log("평타 공격 확인 완료!");
            yield return new WaitForSeconds(0.5f);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
        }

        // --- 2단계: 속성 전환 ---
        if (tutorialSprites.Count > 1)
        {
            // NPC 대사 리스트의 1번 SO 재생
            yield return StartCoroutine(PlayDialogue(1));

            // 조작 가이드 UI 교체
            if (tutorialPanel != null) tutorialPanel.SetActive(true);
            tutorialImage.sprite = tutorialSprites[1];

            // 속성 스크립트 활성화
            PlayerAttribute playerAttributeScript = player.GetComponent<PlayerAttribute>();
            if (playerAttributeScript != null) playerAttributeScript.enabled = true;

            // 유저가 속성 전환(Space)할 때까지 대기
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

            Debug.Log("속성 전환 확인 완료!");
            yield return new WaitForSeconds(0.5f);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
        }

        // --- 3단계: 속성 상성 설명 ---
        if (tutorialSprites.Count > 2)
        {
            // NPC 대사 리스트의 2번 SO 재생
            yield return StartCoroutine(PlayDialogue(2));

            // 상성표 이미지나 텍스트 띄우기
            if (tutorialPanel != null) tutorialPanel.SetActive(true);
            if (tutorialMessages.Count > 2) tutorialText.text = tutorialMessages[2];
            tutorialImage.sprite = tutorialSprites[2];

            // 유저가 내용을 확인(좌클릭/엔터)할 때까지 대기
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return));

            yield return new WaitForSeconds(0.5f);
        }

        // 모든 조작법 가이드 UI 종료
        if (tutorialPanel != null) tutorialPanel.SetActive(false);

        // NPC 대사 리스트의 3번 SO 재생
        yield return StartCoroutine(PlayDialogue(3));

        Debug.Log("조작법 및 구출 튜토리얼 완전 종료!");
    }

    // NPC의 dialogueList 중 하나를 끝까지 재생하고 창을 닫는 공용 함수
    private IEnumerator PlayDialogue(int index)
    {
        player.enabled = false;
        Player_Attack attackScript = player.GetComponent<Player_Attack>();
        if (attackScript != null) attackScript.enabled = false;
        PlayerAttribute playerAttributeScript = player.GetComponent<PlayerAttribute>();
        if (playerAttributeScript != null) playerAttributeScript.enabled = false;
        Player_Skill playerSkillScript = player.GetComponent<Player_Skill>();
        if (playerSkillScript != null) playerSkillScript.enabled = false;

        Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
        if (rigid != null)
        {
            rigid.velocity = Vector2.zero;
        }

        ObjData npcData = npcObject.GetComponent<ObjData>();
        npcData.SetDialogue(index); // NPC에게 대사 SO 교체 명령

        // SO의 대사 줄 수만큼 + 창 닫기용 1번 = 총 횟수
        int totalCalls = npcData.dialogue.lines.Length + 1;

        for (int i = 0; i < totalCalls; i++)
        {
            tutorialDialogue.Action(npcObject);

            // 마지막 호출(창 닫기)이 아닐 때만 유저 입력을 기다림
            if (i < npcData.dialogue.lines.Length)
            {
                yield return StartCoroutine(WaitForUserInput());
            }
        }

        /**
        player.enabled = true;
        if (attackScript != null) attackScript.enabled = true;
        if (playerAttributeScript != null) playerAttributeScript.enabled = true;
        if (playerSkillScript != null) playerSkillScript.enabled = true;
        **/
    }
}