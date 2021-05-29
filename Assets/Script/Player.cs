using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    internal static Player instance;
    Rigidbody2D rigid;
    Animator anim;
    CircleCollider2D col;
    public GameObject bubble;
    public Transform bubbleSpawnPosTr;
    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<CircleCollider2D>();
        CheckOutWall();
    }
    void Update()
    {
        CurState();
        Move();
        Jump();
        DownJump();
        ShootBubble();
    }
    #region CheckOutWall
    public float wallMinMaxOffsetX = 0.02f;
    public float minX;
    public float maxX;
    void CheckOutWall()
    {
        // 레이를 쏴서 바깥벽 좌표값을 찾자
        // 안찾아질때까지 안쪽으로 들어오면서 들어와서 반대로 스캔

        // 오른쪽 벽 = 오른쪽 밖에서 안쪽으로 쏘자
        RaycastHit2D hit;
        Vector2 hitPos;
        hit = Physics2D.Raycast(transform.position + new Vector3(100, 0), Vector2.left, 100, wallLayer);
        do
        {
            // 안찾아질때까지
            hitPos = hit.point;
            hit = Physics2D.Raycast(hit.point + Vector2.left, Vector2.left, 1, wallLayer);
        } while (hit.transform == null);
        // 반대로 쏘자
        hit = Physics2D.Raycast(hitPos + new Vector2(-2, 0), Vector2.right, 2, wallLayer);
        if (hit.transform)
            maxX = hit.point.x - col.radius - wallMinMaxOffsetX;


        // 왼쪽 벽 = 왼쪽 밖에서 안쪽으로 쏘자
        hit = Physics2D.Raycast(transform.position + new Vector3(-100, 0), Vector2.right, 100, wallLayer);
        do
        {
            // 안찾아질때까지
            hitPos = hit.point;
            hit = Physics2D.Raycast(hit.point + Vector2.right, Vector2.right, 1, wallLayer);
        } while (hit.transform != null);
        // 반대로 쏘자
        hit = Physics2D.Raycast(hitPos + new Vector2(2, 0), Vector2.left, 2, wallLayer);
        if (hit.transform)
            minX = hit.point.x + col.radius + wallMinMaxOffsetX;
    }
    #endregion

    #region Animator
    void SetAnimation(string value)
    {
        anim.Play(value);
    }
    #endregion

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
            pos.x = Mathf.Max(minX, pos.x);
            pos.x = Mathf.Min(maxX, pos.x);
            transform.position = pos;

            float rotate = 0f;
            if (moveX < 0)
                rotate = 180f;

            transform.rotation = new Quaternion(0, rotate, 0, 0);
            SetAnimation("run");
        }
        else
        {
            //if (anim.GetCurrentAnimatorStateInfo(0).IsName("jump") == false)
            if (State == StateType.Ground)
                SetAnimation("idle");
        }
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
                SetAnimation("jump");
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
                    SetAnimation("jump");
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
        return (hit.transform != null);
    }
    #endregion

    #region ShootBubble
    private void ShootBubble()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bubble, bubbleSpawnPosTr.position, transform.rotation);
        }
    }
    #endregion
}