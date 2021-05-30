using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public string monsterName;
    public float speed = 0.05f;
    CircleCollider2D col;
    float colRadius;
    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        colRadius = col.radius;
    }
    private void Update()
    {
        // 몬스터는 앞으로 이동
        Move();

        // 더이상 길이 없으면 반대로 이동
        CheckLoadExist();
    }


    private void Move()
    {
        var pos = transform.position;
        pos.x += speed * transform.forward.z;
        transform.position = pos;
    }
    #region CheckLoadExist
    RaycastHit2D hit;
    public LayerMask wallLayer;
    private void CheckLoadExist()
    {
        Debug.Assert(wallLayer != 0, "wallLayer지정안됨");
        hit = Physics2D.Raycast(transform.position + new Vector3(colRadius * transform.forward.z, 0)
            , Vector2.down, colRadius + 0.5f, wallLayer);

        if (hit.transform == null)
            transform.rotation = new Quaternion(0, (transform.rotation.y == 0 ? 180 : 0), 0, 0);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position + new Vector3(colRadius * transform.forward.z, 0)
            , Vector2.down * (colRadius + 0.5f));
    }
    #endregion
}
