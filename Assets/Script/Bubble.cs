using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    public int moveForwardFrame = 6;
    public int currentFrame = 0;
    public float speed = 0.7f;
    public float gravityScale = -0.2f;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rigid.gravityScale = 0;
    }

    void Update()
    {
        var pos = transform.position;
        if (currentFrame++ < moveForwardFrame)
        {
            pos.x += speed * transform.forward.z;
            transform.position = pos;
        }
        else
        {
            rigid.gravityScale = gravityScale;
            SetAnimation("Normal");
        }
    }
    #region Animator
    void SetAnimation(string value)
    {
        anim.Play(value);
    }
    #endregion
}