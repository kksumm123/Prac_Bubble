using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GODestroy : MonoBehaviour
{
    /// <summary>
    /// 0���� Ŭ��� Ȱ��ȭ�� destroyTime�� ���� �ı���
    /// </summary>
    public float destroyTime;

    // ������Ʈ ������
    private void Start()
    {
        if (destroyTime > 0)
            Destroy(gameObject, destroyTime);
    }

}