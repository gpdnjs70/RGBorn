using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    private Vector2 dir = Vector2.right;

    public float distance;
    public LayerMask isLayer;

    [SerializeField] public float damage = 20f;

    void Start()
    {
        Invoke("DestroyBullet", 2);
    }

    void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right, distance, isLayer);
        if(ray.collider != null)
        {
            if(ray.collider.tag == "Enemy")
            {
                Debug.Log("ИэСп");

                Enemy_HP enemy_HP = ray.collider.GetComponent<Enemy_HP>();
                if (enemy_HP != null)
                {
                    enemy_HP.TakeDamage(damage);
                }
            }

            DestroyBullet();
        }

        transform.Translate(dir * speed * Time.deltaTime);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }

    public void SetDirection(Vector2 direction)
    {
        float sign = direction.x >= 0 ? 1f : -1f;
        dir = new Vector2(sign, 0f);

        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1 : -1);
            transform.localScale = scale;
        }
    }
}
