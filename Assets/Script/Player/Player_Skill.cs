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

    private List<ISkill> skills = new List<ISkill>();
    private List<SkillCoolTime> skillUIs = new List<SkillCoolTime>();

    private Entity entity;

    void Start()
    {
        if (skill1_Prefab != null)
        {
            skills.Add(new Fireball(this.gameObject, skill1_Prefab, skill1_Pos));
            skillUIs.Add(skill1_UI);
        }
    }

    void Update()
    {
        if (entity != null && !entity.CanAct) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseSkill(0);
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
