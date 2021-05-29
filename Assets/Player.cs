using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    enum StateType
    {
        Ground,
        Jump,
        JumpFall,
        DownFall
    }
    StateType state;
    [SerializeField] StateType State
    {
        get { return state; }
        set { state = value; }
    }
    
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        Move();
        Jump();
    }
    public float speed = 0.1f;
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
            pos.x += moveX * speed;
            transform.position = pos;

            if (moveX < 0)
                transform.rotation = new Quaternion(0, 180, 0, 0);
            else
                transform.rotation = new Quaternion(0, 0, 0, 0);
            anim.Play("run");
        }
        else
            anim.Play("idle");
    }

    public float jumpForce = 1100f;
    private void Jump()
    {
        if (state != StateType.Ground)
            return;

        // ÈûÀ» Áà¾ß ÇÔ
        // º®À» ¶Õ¾î¾ß ÇÔ
        if (Input.GetKeyDown(KeyCode.W))
        {
            rigid.AddForce(new Vector2(0, jumpForce));
            state = StateType.Jump;
        }

    }
}