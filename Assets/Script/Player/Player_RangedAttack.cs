using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_RangedAttack : MonoBehaviour
{
    public GameObject bullet;
    public Transform pos;

    [SerializeField] private Player_Movement movement;
    [SerializeField] private Player_WeaponController weaponController;
    [SerializeField] private Animator animator;
    [SerializeField] private SkillCoolTime rangedSkillCooldown;

    void Start()
    {
        
    }

    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        // R키 입력
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 쿨타임 체크
            if (rangedSkillCooldown != null && !rangedSkillCooldown.TryUseSkill())
            {
                // 쿨타임 중이면 그냥 리턴
                return;
            }

            // 여기까지 왔다는 건 스킬 사용 가능하다는 뜻
            if (animator != null)
            {
                animator.SetTrigger("Skill");
            }

            GameObject b = Instantiate(bullet, pos.position, Quaternion.identity);

            Bullet bulletScript = b.GetComponent<Bullet>();

            if (bulletScript != null && movement != null)
            {
                bulletScript.SetDirection(movement.lastDir);
            }
        }
    }
}
