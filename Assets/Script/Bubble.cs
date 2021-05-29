using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    Rigidbody2D rigid;
    CircleCollider2D col;
    Animator anim;
    public int moveForwardFrame = 6;
    public int currentFrame = 0;
    public float speed = 0.7f;
    public float gravityScale = -0.2f;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        rigid.gravityScale = 0;
        SetTrigger(true);
    }

    void FixedUpdate()
    {
        if (currentFrame++ < moveForwardFrame)
        {
            FastMove();
        }
        else
        {
            State = StateType.FreeFly;
            rigid.gravityScale = gravityScale;
            SetAnimation("Normal");
            SetTrigger(false);
        }
    }

    
    #region Animator
    void SetAnimation(string value)
    {
        anim.Play(value);
    }
    #endregion
    #region istrigger
    void SetTrigger(bool value)
    {
        col.isTrigger = value;
    }
    #endregion
    #region State Declare
    enum StateType
    {
        FastMove,
        FreeFly,
        GrapEnemy
    }
    [SerializeField]
    StateType state;
    StateType State
    {
        get { return state; }
        set { state = value; }
    }
    #endregion
    #region Move
    public LayerMask wallLayer;
    private void FastMove()
    {
        State = StateType.FastMove;

        var pos = transform.position;
        pos.x += speed * transform.forward.z;

        // 벽을 뚫으면 안됨
        if (transform.forward.z > 0)
        { // 우측으로
            var hit = Physics2D.Raycast(transform.position, Vector2.right, 100, wallLayer);
            if (hit.transform != null)
                pos.x = Mathf.Min(pos.x, hit.point.x);
        }
        else
        { // 좌측으로
            var hit = Physics2D.Raycast(transform.position, Vector2.left, 100, wallLayer);
            if (hit.transform != null)
                pos.x = Mathf.Max(pos.x, hit.point.x);
        }
        transform.position = pos;
    }
    #endregion
}