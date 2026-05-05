using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class savePoint : MonoBehaviour
{
    public enum SavePointType { Respawn, StageClear }

    [SerializeField] private SavePointType pointType;
    [SerializeField] private string nextSceneName;
    [SerializeField] private GameObject sceneChange;
    private bool isProcessed = false;

    [Header("성장 수치 설정 (StageClear 전용)")]
    [SerializeField] private float upgradeMaxHP = 100f;       // 도달할 최종 HP (값)
    [SerializeField][Range(0f, 2f)] private float attackUpgradePercent = 0.2f; // 상승할 공격력 (20% 등)

    [SerializeField] private List<Sprite> upgradeSprites;
    [SerializeField] private List<string> upgradeTexts;

    private void Start()
    {
        // 씬 시작 시 데이터 로드
        if (SaveManager.Instance != null)
        {
            SaveData data = SaveManager.Instance.LoadGame();

            if (pointType == SavePointType.Respawn)
            {
                // 현재 씬 이름이 일치하고, 리스폰 상태(사망/이어하기)일 때만 맵에 남겨둠
                if (data != null && data.isRespawn && data.lastSceneName == SceneManager.GetActiveScene().name)
                {
                    Debug.Log("리스폰 포인트: 활성화 상태 유지");
                    // 아무것도 안 함 (활성화 유지)
                }
                else
                {
                    Debug.Log("리스폰 포인트: 비활성화 (파괴되지 않고 꺼짐)");
                    this.gameObject.SetActive(false); // 여기서 오브젝트가 꺼짐
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isProcessed) return;

        if (pointType == SavePointType.StageClear)
        {
            isProcessed = true;

            if (other.TryGetComponent(out Player_Movement playerMove))
            {
                playerMove.SetControlEnabled(false);
                StartCoroutine(InteractionSequence(playerMove));
            }
        }
        else if (pointType == SavePointType.Respawn)
        {
            
            Debug.Log("리스폰 지점 통과");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 리스폰 포인트인 경우, 플레이어가 이 영역을 벗어나면 오브젝트를 비활성화함
            if (pointType == SavePointType.Respawn)
            {
                Debug.Log("리스폰 포인트 영역 이탈: 오브젝트 비활성화");
                this.gameObject.SetActive(false);
            }
            else if (pointType == SavePointType.StageClear && isProcessed)
            {
                Debug.Log("스테이지 클리어 포인트 영역 이탈: 비활성화");
                this.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator InteractionSequence(Player_Movement playerMove)
    {
        Player_HP playerHP = playerMove.GetComponent<Player_HP>();
        Player_Attack playerAtk = playerMove.GetComponent<Player_Attack>();

        // 1. 공통 로직: 무조건 풀피 회복 (수치 증가 전/후 상관없이 풀피)
        playerHP.RestoreFullHP();

        if (pointType == SavePointType.StageClear)
        {
            isProcessed = true;

            // 2. 능력치 변화 기록
            float oldHP = playerHP.stats.MaxHP.BaseValue;

            // HP는 고정 값으로 대입
            playerHP.ApplyHPValue(upgradeMaxHP);

            // 공격력은 현재 데미지에서 입력한 퍼센트만큼 강화 (예: 2 * 1.2 = 2.4)
            playerAtk.damage *= (1f + attackUpgradePercent);

            // 3. 변경된 데이터를 JSON 저장용 객체에 갱신
            SaveData data = SaveManager.Instance.LoadGame() ?? new SaveData();
            data.currentMaxHP = upgradeMaxHP;
            // 공격력 배율도 누적으로 관리하고 싶다면 여기서 갱신
            data.attackMultiplier *= (1f + attackUpgradePercent);

            SaveManager.Instance.SaveGame(false, nextSceneName, data);

            // 4. UI 출력 (HP: 50 -> 100 형태)
            StatUIController.Instance.StartUISequence(upgradeSprites, upgradeTexts, () => {
                // UI 끝나면 다음 씬으로 넘어가거나 조작 활성화
                if (sceneChange != null) sceneChange.SetActive(true);
                playerMove.SetControlEnabled(true);
            });

            yield break;
        }
        else
        {
            // 리스폰 포인트는 회복만 시켜주고 저장
            SaveManager.Instance.SaveGame(true);
            yield return new WaitForSeconds(0.5f);

            playerMove.SetControlEnabled(true);
        }
    }
}
