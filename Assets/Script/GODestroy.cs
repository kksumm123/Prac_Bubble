using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GODestroy : MonoBehaviour
{
    /// <summary>
    /// 0보다 클경우 활성화후 destroyTime초 이후 파괴됨
    /// </summary>
    public float destroyTime;

    // 업데이트 직전에
    private void Start()
    {
        if (destroyTime > 0)
            Destroy(gameObject, destroyTime);
    }
}