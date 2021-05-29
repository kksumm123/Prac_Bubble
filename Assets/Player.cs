using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    public float speed = 0.1f;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float moveX = 0;
        if (Input.GetKey(KeyCode.A))
            moveX = -1;
        if (Input.GetKey(KeyCode.D))
            moveX = 1;


        if (moveX != 0)
        {
            var pos = transform.position;
            pos.x = moveX * speed;
            transform.position = pos;
            anim.Play("run");
        }
        else
            anim.Play("idle");
    }
}