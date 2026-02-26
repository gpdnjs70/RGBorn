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

    [SerializeField] private GameObject selectionUI;

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
        if (Pointer.SelectedTarget == null)
        {
            RestoreOrSetDefaultTarget();
        }
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
            if (interaction != null) interaction.selectionUI = selectionUI;
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

            if (Pointer.SelectedTarget == null)
            {
                RestoreOrSetDefaultTarget();
            }
        }
        else
        {
            // 적의 턴일 경우 AI 공격 (플레이어 팀의 첫 번째 캐릭터 공격)
            state = State.enemyTurn;
            activeCharacterBattle.Attack(playerTeamList[0], () =>
            {
                ChooseNextActiveCharacter();
            });
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
        // 현재 상태가 플레이어 턴인지 확인
        if (state != State.playerTurn)
        {
            Debug.Log("지금은 공격할 수 있는 타이밍이 아닙니다.");
            return;
        }

        if (Pointer.SelectedTarget == null)
        {
            Debug.Log("공격할 대상을 먼저 선택하세요!");
            return;
        }

        // 공격 대상을 정함
        CharacterBattle targetEnemy = Pointer.SelectedTarget.GetComponent<CharacterBattle>();

        if (targetEnemy != null)
        {
            state = State.enemyTurn; // 공격 중에는 다시 버튼이 안 눌리게 상태 변경

            activeCharacterBattle.Attack(targetEnemy, () =>
            {
                ChooseNextActiveCharacter();
            });
            Pointer.ClearSelection();
        }

        if (selectionUI != null) selectionUI.SetActive(false);
    }

    public State GetState()
    {
        return state;
    }
}
