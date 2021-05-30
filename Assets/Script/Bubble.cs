using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    Rigidbody2D rigid;
    CircleCollider2D col;
    Animator anim;
    // 스태틱으로 둬야 개체당 할당이 아니라 클래스자체에 할당
    static List<Bubble> bubbles = new List<Bubble>();
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
        bubbles.Add(this);
    }
    private void OnDestroy()
    {
        bubbles.Remove(this);
    }
    public float nearPlayerCheckDistance = 1.9f;
    void FixedUpdate()
    {
        if (currentFrame++ < moveForwardFrame)
            FastMove();
        else
        {
            float distance = Vector3.Distance(Player.instance.transform.position
                    , transform.position);

            if (distance < nearPlayerCheckDistance)
            {
                // 공룡이 인근에 있으면 자신(버블)을 터트리자
                ExplosionByPlayer();
            }
            else
            {
                Normal();
            }
        }
    }

    #region Animator
    void SetAnimation(string value)
    {
        anim.Play(value);
    }
    #endregion
    #region Trigger
    void SetTrigger(bool value)
    {
        col.isTrigger = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTouchedBubble(collision.transform);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTouchedBubble(collision.transform);
    }

    private void OnTouchedBubble(Transform collisionTr)
    {
        if (State == StateType.FreeFly)
        {
            if (collisionTr.CompareTag("Player"))
            {
                ExplosionByPlayer();
            }
        }
        else if (State == StateType.FastMove)
        {
            if (collisionTr.CompareTag("Monster"))
            {
                GrabMonster(collisionTr);
            }
        }
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

    #region ExplosionBubble
    private void ExplosionByPlayer()
    {
        // 주변 버블을 모으자
        // 같이 터트리자
        var nearBubbles = new List<Bubble>();

        FindNearBubbles(this.transform, nearBubbles);
        nearBubbles.ForEach(x => Destroy(x.gameObject));
    }
    public float nearBubbleCheckDistance = 2.2f;
    void FindNearBubbles(Transform tr, List<Bubble> nearBubbles)
    {
        nearBubbles.Add(this);
        foreach (var item in bubbles)
        {
            if (nearBubbles.Contains(item))
                continue;
            float distance = Vector2.Distance(tr.position, item.transform.position);
            if (distance < nearBubbleCheckDistance)
            {
                nearBubbles.Add(item);
                FindNearBubbles(item.transform, nearBubbles);
            }
        }
    }
    #endregion
    #region GrabMonster
    private void GrabMonster(Transform collisionTr)
    {
        // 몬스터를 숨기자
        // 몬스터를 버블의 자식으로 돌리자
        // 버블을 몬스터이름 + 1, 2, 3, 4 돌리자
        collisionTr.gameObject.SetActive(false);
        collisionTr.parent = transform;

        StartCoroutine(GrabMonsterBubbleCo(collisionTr.name));
    }

    private IEnumerator GrabMonsterBubbleCo(string name)
    {
        yield return new WaitForSeconds(1);
    }

    #endregion

    #region FastMove
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

    #region Normal
    private void Normal()
    {
        State = StateType.FreeFly;

        rigid.gravityScale = gravityScale;
        SetAnimation("Normal");
        SetTrigger(false);
    }
    #endregion
}