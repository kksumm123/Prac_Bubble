using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    CircleCollider2D col;
    public GameObject bubble;
    public Transform bubbleSpawnPosTr;

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
        DownJump();
        ShootBubble();
    }

    #region State Declare
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
    #endregion
    #region State
    private void CurState()
    {
        if (State == StateType.Jump && rigid.velocity.y < 0)
            State = StateType.JumpFall;

        if (State == StateType.JumpFall
            || (State == StateType.DownJumpFall && col.isTrigger == false))
        {
            if (isGround())
            {
                State = StateType.Ground;
                SetTrigger(false);
            }
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
            Debug.Assert(wallLayer != 0, "wallLayer지정안됨");
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

    #region Move
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
    #endregion

    #region Jump
    public float jumpForce = 1100f;
    private void Jump()
    {
        // 힘을 줘야 함
        // 벽을 뚫어야 함
        // 떨어질땐 뚫으면 안됨


        if (State == StateType.Ground)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                SetTrigger(true);
                rigid.AddForce(new Vector2(0, jumpForce));
                State = StateType.Jump;
            }
        }
    }
    #endregion

    #region DownJump
    private void DownJump()
    {
        if (State == StateType.Ground)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (ChkUnderGround())
                {
                    SetTrigger(true);
                    State = StateType.DownJumpFall;
                }
            }
        }
    }

    public float underGroundOffsetY = -2.1f;
    private bool ChkUnderGround()
    {
        var hit = Physics2D.Raycast(transform.position + new Vector3(0, underGroundOffsetY, 0)
            , new Vector2(0, -1), 50, wallLayer);
        Debug.Assert(wallLayer != 0, "wallLayer지정안됨");
        Debug.Log($"{transform.position}, {hit.point}");
        return (hit.transform != null);
    }
    #endregion

    #region ShootBubble
    private void ShootBubble()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
    #endregion
}