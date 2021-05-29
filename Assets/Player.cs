using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    CircleCollider2D col;
    enum StateType
    {
        Ground,
        Jump,
        JumpFall,
        DownJumpFall
    }
    [SerializeField]
    StateType state;
    StateType State
    {
        get { return state; }
        set { state = value; }
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        CurState();
        Move();
        Jump();
    }
    #region State
    void SetState(StateType _state)
    {
        State = _state;
    }

    private void CurState()
    {
        if (State == StateType.Jump && rigid.velocity.y < 0)
            SetState(StateType.JumpFall);

        if (State == StateType.JumpFall && isGround()) 
        {
            SetState(StateType.Ground);
            SetTrigger(false);
        }
    }

    public LayerMask wallLayer;
    public float chkGroundOffsetX = 0.4f;
    private bool isGround()
    {
        if (ChkGroundRay(transform.position))
            return true;
        if (ChkGroundRay(transform.position + new Vector3(-chkGroundOffsetX, 0, 0)))
            return true;
        if (ChkGroundRay(transform.position + new Vector3(chkGroundOffsetX, 0, 0)))
            return true;

        return false;

        bool ChkGroundRay(Vector3 pos)
        {
            // true = Ground
            // false = Aerial
            var hit = Physics2D.Raycast(pos, new Vector2(0, -1), 1.1f, wallLayer);
            Debug.Assert(wallLayer != 0, "wallLayer�����ȵ�");
            return (hit.transform != null);
        }
    }
    #endregion
    #region istrigger
    void SetTrigger(bool value)
    {
        col.isTrigger = value;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (State == StateType.DownJumpFall)
        {
            SetTrigger(false);
        }
    }
    #endregion

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

            float rotate = 0f;
            if (moveX < 0)
                rotate = 180f;

            transform.rotation = new Quaternion(0, rotate, 0, 0);
            anim.Play("run");
        }
        else
            anim.Play("idle");
    }

    public float jumpForce = 1100f;
    private void Jump()
    {
        // ���� ��� ��
        // ���� �վ�� ��
        // �������� ������ �ȵ�


        if (State == StateType.Ground)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                SetTrigger(true);
                rigid.AddForce(new Vector2(0, jumpForce));
                SetState(StateType.Jump);
            }
        }
    }
}