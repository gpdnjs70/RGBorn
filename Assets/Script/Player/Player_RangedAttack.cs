using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_RangedAttack : MonoBehaviour
{
    public GameObject bullet;
    public Transform pos;
    private Entity entity;

    [SerializeField] private Player_Movement movement;
    [SerializeField] private Animator animator;
    [SerializeField] private SkillCoolTime rangedSkillCooldown;

    [SerializeField] private PlayerAttribute playerAttribute;

    void Start()
    {
        
    }

    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        entity = GetComponent<Entity>();
    }

    void Update()
    {
        if (!entity.CanAct)
            return;

        UpdateAttackBoxPosition();

        // R키 입력
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 쿨타임 체크
            if (rangedSkillCooldown != null && !rangedSkillCooldown.TryUseSkill())
            {
                // 쿨타임 중이면 그냥 리턴
                return;
            }

            // 스킬 사용 가능
            if (animator != null)
            {
                animator.SetTrigger("Skill");
            }

            GameObject b = Instantiate(bullet, pos.position, Quaternion.identity);

            Bullet bulletScript = b.GetComponent<Bullet>();

            if (bulletScript != null && movement != null)
            {
                Vector2 fireDir = new Vector2(movement.dirX, movement.dirY);

                if (fireDir == Vector2.zero) fireDir = Vector2.right;

                bulletScript.SetDirection(fireDir);
                bulletScript.Init(GetComponent<Entity>());
            }
        }
    }

    private void UpdateAttackBoxPosition()
    {
        if (movement == null) return;

        float sign = movement.dirX >= 0 ? 1f : -1f;

        // 근접 공격 박스 위치
        if (pos != null)
        {
            Vector3 local = pos.localPosition;
            local.x = Mathf.Abs(local.x) * sign;
            pos.localPosition = local;
        }
    }
}
