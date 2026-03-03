using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.5f;
    private Rigidbody2D rb;
    private Vector2 input;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input.Normalize();

        if (input.magnitude > 0)
        {
            anim.SetFloat("DirX", input.x);
            anim.SetFloat("DirY", input.y);
            // anim.SetBool("IsMoving", true);
        }
        else
        {
            // anim.SetBool("IsMoving", false);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = input * speed;
    }
}
