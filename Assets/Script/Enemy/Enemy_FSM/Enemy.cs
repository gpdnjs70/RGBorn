using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator animator;
    public Transform player;
    public float speed;
    public Vector2 home;

    public float atkCoolTime = 4;
    public float atkDelay;

    public Transform target;
    public Enemy_HP hp;
    public Entity targetHP;

    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        home = transform.position;
    }

    private void Update()
    {
        if(atkDelay >= 0)
        {
            atkDelay -= Time.deltaTime;
        }
    }

    public void DirectionEnemy(float target, float baseobj)
    {
        if(target < baseobj)
        {
            animator.SetFloat("Direction", -1);
        }
        else
        {
            animator.SetFloat("Direction", 1);
        }
    }
}
