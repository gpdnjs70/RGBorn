using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_RangedAttack : MonoBehaviour
{
    public GameObject bullet;
    public Transform pos;

    public float cooltime;
    private float curtime;

    [SerializeField] private Player_Movement movement;

    void Start()
    {
        
    }

    void Update()
    {
        if (curtime > 0)
        {
            curtime -= Time.deltaTime;
        }

        if (curtime <= 0f && Input.GetKeyDown(KeyCode.R))
        {
            GameObject b = Instantiate(bullet, pos.position, Quaternion.identity);

            Bullet bulletScript = b.GetComponent<Bullet>();

            if (bulletScript != null && movement != null)
            {
                bulletScript.SetDirection(movement.lastDir);
            }

            curtime = cooltime;
        }
    }
}
