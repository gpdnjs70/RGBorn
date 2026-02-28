using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class CharacterSpawnInfo
{
    public Transform prefab;
    public Vector3 spawnPosition;
}

public class BattleHandler : MonoBehaviour
{
    [SerializeField] private List<CharacterSpawnInfo> playerTeamSetup;
    [SerializeField] private List<CharacterSpawnInfo> enemyTeamSetup;

    private List<CharacterBattle> playerTeamList = new List<CharacterBattle>();
    private List<CharacterBattle> enemyTeamList = new List<CharacterBattle>();

    private List<CharacterBattle> allCharacterList = new List<CharacterBattle>();
    private int activeCharacterIndex = 0;

    private CharacterBattle activeCharacterBattle;
    private State state;
    private bool isSelectingTarget = false; // 타겟 선택 모드인지 확인

    [SerializeField] private GameObject selectionUI;
    [SerializeField] private GameObject colorSelectionUI;

    public enum State
    {
        start, 
        playerTurn, 
        enemyTurn, 
        win,
    }

    private void Awake()
    {
        state = State.start;
    }

    private void Start()
    {
        // 플레이어 팀 생성
        foreach (var info in playerTeamSetup)
        {
            playerTeamList.Add(SpawnCharacter(info, true));
        }

        // 적 팀 생성
        foreach (var info in enemyTeamSetup)
        {
            enemyTeamList.Add(SpawnCharacter(info, false));
        }

        // 전체 턴 순서를 위해 리스트 합치기
        allCharacterList.AddRange(playerTeamList);
        allCharacterList.AddRange(enemyTeamList);

        SetActiveCharacterBattle(allCharacterList[0]);

        state = State.playerTurn;
        ShowPlayerUI();
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state = State.playerTurn;
            activeCharacterBattle.Attack(enemyTeamList[0], () =>
            {
                ChooseNextActiveCharacter();
            });
        }
        */

        Debug.Log($"{state}");
    }

    private CharacterBattle SpawnCharacter(CharacterSpawnInfo info, bool isPlayerTeam)
    {
        Transform characterTransform = Instantiate(info.prefab, info.spawnPosition, Quaternion.identity);
        CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();

        if (isPlayerTeam)
        {
            var interaction = characterTransform.GetComponent<playerInteraction>();
            if (interaction != null)
            {
                interaction.selectionUI = selectionUI;
                interaction.colorSelectionUI = colorSelectionUI;
            }

            var colorAttr = characterTransform.GetComponent<PlayerColorAttribute>();
            if (colorAttr != null)
            {
                colorAttr.colorSelectionUI = colorSelectionUI.GetComponent<ColorSelection>();
            }
        }

        characterBattle.Setup(isPlayerTeam);
        return characterBattle;
    }

    private void SetActiveCharacterBattle(CharacterBattle characterBattle)
    {
        if(activeCharacterBattle != null)
        {
            activeCharacterBattle.HideSelectionCircle();
        }

        activeCharacterBattle = characterBattle;
        activeCharacterBattle.ShowSelectionCircle();
    }

    private void ChooseNextActiveCharacter()
    {
        // 다음 인덱스로 이동 (리스트 끝이면 다시 0으로)
        activeCharacterIndex = (activeCharacterIndex + 1) % allCharacterList.Count;
        SetActiveCharacterBattle(allCharacterList[activeCharacterIndex]);

        // 현재 캐릭터가 플레이어 팀인지 확인
        if (activeCharacterBattle.IsPlayerTeam())
        {
            state = State.playerTurn;
            ShowPlayerUI();
        }
        else
        {
            // 적의 턴일 경우
            state = State.enemyTurn;

            Monster currentMonster = activeCharacterBattle.GetComponent<Monster>();

            if (currentMonster != null)
            {
                // 카운트 체크
                if (currentMonster.CheckTurnAndPrepareAttack())
                {
                    // 공격 대상 결정
                    CharacterBattle targetPlayer = playerTeamList[0];

                    // 대상의 PlayerAttribute를 가져와서 몬스터에게 전달]
                    currentMonster.playerAttribute = targetPlayer.GetComponent<PlayerAttribute>();

                    // 공격 실행
                    activeCharacterBattle.Attack(targetPlayer, () =>
                    {
                        ChooseNextActiveCharacter();
                    });
                }
                else
                {
                    // 공격 안 하는 턴
                    ChooseNextActiveCharacter();
                }
            }
        }
    }

    private void ShowPlayerUI()
    {
        if (selectionUI != null && activeCharacterBattle != null)
        {
            selectionUI.SetActive(true);

            // activeCharacterBattle이 붙어있는 오브젝트의 playerInteraction 컴포넌트를 가져와 위치 업데이트
            var interaction = activeCharacterBattle.GetComponent<playerInteraction>();
            if (interaction != null)
            {
                interaction.UpdateUIPosition();
            }
        }
    }

    private void RestoreOrSetDefaultTarget()
    {
        // 마지막으로 기억된 타겟이 있는지 확인
        Pointer.RestoreLastSelection();

        // 만약 기억된 게 없거나 그 적이 죽었다면(null이라면) 첫 번째 적
        if (Pointer.SelectedTarget == null && enemyTeamList.Count > 0)
        {
            Pointer p = enemyTeamList[0].GetComponent<Pointer>();
            if (p != null) p.SelectThis();
        }
    }

    public void OnAttackButtonClicked()
    {
        // 플레이어 턴인지 확인
        if (state != State.playerTurn) return;

        // 타겟 선택 모드 활성화
        isSelectingTarget = true;
        Debug.Log("공격 대상을 선택하세요!");
        RestoreOrSetDefaultTarget();
    }

    // 실제 공격을 수행
    public void ExecuteAttack()
    {
        if (!isSelectingTarget || Pointer.SelectedTarget == null) return;

        CharacterBattle targetEnemy = Pointer.SelectedTarget.GetComponent<CharacterBattle>();

        if (targetEnemy != null)
        {
            isSelectingTarget = false;
            state = State.enemyTurn; // 상태 전환으로 중복 클릭 방지

            activeCharacterBattle.Attack(targetEnemy, () =>
            {
                ChooseNextActiveCharacter();
            });

            // 공격 후 포인터 숨기기
            Pointer.ClearSelection();
            selectionUI.SetActive(false);
        }
    }

    public void OnColorButtonClicked()
    {
        // 플레이어 턴인지 확인
        if (state != State.playerTurn) return;

        // 색상 선택 UI 활성화
        selectionUI.SetActive(false);

        var colorAttr = activeCharacterBattle.GetComponent<PlayerColorAttribute>();
        if (colorAttr != null)
        {
            colorAttr.ActivateColorSelection();
        }
    }

    public void EndTurn()
    {
        // 중복 호출 방지 및 상태 전환
        state = State.enemyTurn;

        // UI들 숨기기
        selectionUI.SetActive(false);
        colorSelectionUI.SetActive(false);

        // 다음 캐릭터로 턴 넘기기
        ChooseNextActiveCharacter();
    }

    public State GetState()
    {
        return state;
    }

    public bool IsSelectingTarget()
    {
        return isSelectingTarget;
    }

    // 캐릭터 사망 시 리스트에서 제거하는 메서드
    public void OnCharacterDeath(CharacterBattle deadCharacter)
    {
        if (playerTeamList.Contains(deadCharacter))
        {
            playerTeamList.Remove(deadCharacter);
        }
        else if (enemyTeamList.Contains(deadCharacter))
        {
            enemyTeamList.Remove(deadCharacter);
        }

        // 전체 리스트에서도 제거
        if (allCharacterList.Contains(deadCharacter))
        {
            // 만약 죽은 캐릭터가 현재 턴을 진행 중이던 캐릭터였다면 인덱스 조절이 필요할 수 있습니다.
            int deadIndex = allCharacterList.IndexOf(deadCharacter);
            allCharacterList.RemoveAt(deadIndex);

            // 죽은 캐릭터 뒤의 캐릭터들이 당겨지므로 인덱스 보정
            if (deadIndex <= activeCharacterIndex)
            {
                activeCharacterIndex--;
            }
        }

        // 승리/패배 조건 체크
        CheckBattleOver();
    }

    private void CheckBattleOver()
    {
        if (enemyTeamList.Count == 0)
        {
            state = State.win;
            Debug.Log("전투 승리!");
        }
        else if (playerTeamList.Count == 0)
        {
            Debug.Log("전투 패배...");
        }
    }
}
