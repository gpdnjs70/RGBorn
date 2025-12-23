using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    public float speed;
    private Vector2 dir = Vector2.right;

    public float distance;
    public LayerMask isLayer;

    public float damage = 20f;
    
    private ColorButtonController colorController;

    void Start()
    {
        Invoke("DestroyBullet", 2);
    }

    private void Awake()
    {
        colorController = FindFirstObjectByType<ColorButtonController>();
    }

    void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right, distance, isLayer);
        if(ray.collider != null)
        {
            if(ray.collider.tag == "Enemy")
            {
                Debug.Log("명중");

                Enemy_HP enemy_HP = ray.collider.GetComponent<Enemy_HP>();
                if (enemy_HP != null)
                {
                    if (colorController != null)
                    {
                        Debug.Log($"현재 색상은 {colorController.currentMode}");
                    }

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
