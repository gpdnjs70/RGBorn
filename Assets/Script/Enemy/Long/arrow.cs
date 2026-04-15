using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class arrow : MonoBehaviour
{
    public float speed;
    private Vector2 moveDir = Vector2.right;

    public float distance;
    public LayerMask isLayer;

    public float damage = 20f;
    private Entity source;

    [SerializeField] private AttributeDatabase attributeDB;

    void Start()
    {
        Invoke("DestroyBullet", 2);
    }

    public void Init(Entity sourceEntity)
    {
        source = sourceEntity;
    }

    void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right, distance, isLayer);
        if (ray.collider != null)
        {
            Entity target = ray.collider.GetComponent<Entity>();
            if (target != null && source != null)
            {
                Entity actualTarget = source.ResolveAttackTarget(target);

                actualTarget.TakeDamage(damage, source.gameObject);
            }

            DestroyBullet();
            return;
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }


    void DestroyBullet()
    {
        Destroy(gameObject);
    }

    public void SetDirection(Vector2 direction)
    {
        moveDir = direction.normalized;

        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

