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

    private enum State
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

    public void OnAttackButtonClicked()
    {
        // 현재 상태가 플레이어 턴인지 확인
        if (state != State.playerTurn)
        {
            Debug.Log("지금은 공격할 수 있는 타이밍이 아닙니다.");
            return;
        }

        // 공격 대상을 정함
        CharacterBattle targetEnemy = enemyTeamList[0];

        if (targetEnemy != null)
        {
            state = State.enemyTurn; // 공격 중에는 다시 버튼이 안 눌리게 상태 변경

            activeCharacterBattle.Attack(targetEnemy, () =>
            {
                ChooseNextActiveCharacter();
            });
        }

        if (selectionUI != null) selectionUI.SetActive(false);
    }
}
