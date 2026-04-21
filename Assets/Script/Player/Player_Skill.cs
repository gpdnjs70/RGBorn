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
    [SerializeField] private Transform skill1_Pos;
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
            Fireball fb = new Fireball(this.gameObject, skill1_Prefab, skill1_Pos);
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
        if (skills.Count > 0 && skill1_Prefab != null && skill1_Pos != null)
        {
            GameObject b = Instantiate(skill1_Prefab, skill1_Pos.position, Quaternion.identity);
            Bullet bulletScript = b.GetComponent<Bullet>();

            Player_Movement movement = GetComponent<Player_Movement>();
            if (bulletScript != null && movement != null)
            {
                Vector2 fireDir = new Vector2(movement.dirX, movement.dirY);
                if (fireDir == Vector2.zero) fireDir = Vector2.right;

                bulletScript.SetDirection(fireDir);
                bulletScript.Init(GetComponent<Entity>());
            }
            Debug.Log("애니메이션 이벤트: 파이어볼 실제 생성!");
        }
    }
}
