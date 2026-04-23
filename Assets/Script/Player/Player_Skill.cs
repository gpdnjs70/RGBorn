using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    string SkillName { get; }
    float Cooldown { get; }
    void Execute(); // 스킬 실행 로직
}

public class Player_Skill : MonoBehaviour
{
    [SerializeField] private GameObject skill1_Prefab;
    [SerializeField] private Transform[] skill1_Positions;
    [SerializeField] private SkillCoolTime skill1_UI;

    [SerializeField] private SkillCoolTime skill2_UI;

    [SerializeField] private SkillCoolTime skill3_UI;

    private List<ISkill> skills = new List<ISkill>();
    private List<SkillCoolTime> skillUIs = new List<SkillCoolTime>();

    private Entity entity;

    void Start()
    {
        if (skill1_Prefab != null)
        {
            Fireball fb = new Fireball(this.gameObject, skill1_Prefab);
            skills.Add(fb);
            skillUIs.Add(skill1_UI);
            skill1_UI.SetMaxCooldown(fb.Cooldown);
        }

        if (skill2_UI != null)
        {
            InvincibleSkill invinc = new InvincibleSkill(this.gameObject);
            skills.Add(invinc);
            skillUIs.Add(skill2_UI);
            skill2_UI.SetMaxCooldown(invinc.Cooldown);
        }

        if (skill3_UI != null)
        {
            LifeStealSkill ls = new LifeStealSkill(this.gameObject);
            skills.Add(ls);
            skillUIs.Add(skill3_UI);
            skill3_UI.SetMaxCooldown(ls.Cooldown);
        }
    }

    void Update()
    {
        if (entity != null && !entity.CanAct) return;
        if (Input.GetKeyDown(KeyCode.Q)) UseSkill(1);
        if (Input.GetKeyDown(KeyCode.E)) UseSkill(0);
        if (Input.GetKeyDown(KeyCode.R)) UseSkill(2);
    }

    void UseSkill(int index)
    {
        if (index < skills.Count && skills[index] != null)
        {
            // 쿨타임 체크
            if (skillUIs[index].TryUseSkill())
            {
                skills[index].Execute();
            }
        }
    }

    public void OnFireballRelease()
    {
        // 첫 번째 스킬이 파이어볼이라고 가정
        if (skills.Count > 0 && skill1_Prefab != null)
        {
            Transform spawnPos = GetSpawnPositionByDirection();

            GameObject b = Instantiate(skill1_Prefab, spawnPos.position, Quaternion.identity);
            Bullet bulletScript = b.GetComponent<Bullet>();

            Player_Movement movement = GetComponent<Player_Movement>();
            if (bulletScript != null && movement != null)
            {
                float dx = movement.dirX > 0 ? 1 : (movement.dirX < 0 ? -1 : 0);
                float dy = movement.dirY > 0 ? 1 : (movement.dirY < 0 ? -1 : 0);
                Vector2 fireDir = new Vector2(dx, dy);

                if (fireDir == Vector2.zero) fireDir = Vector2.right;

                bulletScript.SetDirection(fireDir); // 여기서 발사체 애니메이터가 설정됨
                bulletScript.Init(GetComponent<Entity>());
            }
        }
    }

    private Transform GetSpawnPositionByDirection()
    {
        Player_Movement movement = GetComponent<Player_Movement>();
        float x = movement.dirX;
        float y = movement.dirY;

        // 시계 방향 순서대로 인덱스를 정한다고 가정 (예: 0=북, 1=북동, 2=동...)
        int index = 0;

        if (x == 0 && y > 0) index = 0;      // 북 (Up)
        else if (x > 0 && y > 0) index = 1; // 북동 (Up-Right)
        else if (x > 0 && y == 0) index = 2; // 동 (Right)
        else if (x > 0 && y < 0) index = 3; // 남동 (Down-Right)
        else if (x == 0 && y < 0) index = 4; // 남 (Down)
        else if (x < 0 && y < 0) index = 5; // 남서 (Down-Left)
        else if (x < 0 && y == 0) index = 6; // 서 (Left)
        else if (x < 0 && y > 0) index = 7; // 북서 (Up-Left)

        // 방어 코드: 배열 범위를 벗어나지 않게 처리
        if (index >= skill1_Positions.Length) return skill1_Positions[0];

        return skill1_Positions[index];
    }
}
