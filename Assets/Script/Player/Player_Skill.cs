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
            IdentifySkill idSkill = new IdentifySkill();
            skills.Add(idSkill); // 매개변수 없는 생성자 사용
            skillUIs.Add(skill2_UI);
            skill2_UI.SetMaxCooldown(idSkill.Cooldown);
        }
    }

    void Update()
    {
        if (entity != null && !entity.CanAct) return;
        if (Input.GetKeyDown(KeyCode.Q)) UseSkill(0);
        // if (Input.GetKeyDown(KeyCode.E)) UseSkill(1);
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
}
