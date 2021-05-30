using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    // 왜 싱글턴사용하나?
    // static인 DelayCoroutine 함수가 
    // 접근제한을 둔 상태로
    // RunDelayCoroutineCo를 실행하려고 할 때
    // 싱글턴을 사용해야하기때문에

    // 왜 싱글턴을 저렇게 할당하나?
    static CoroutineManager instance;
    static CoroutineManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameObject(
                    nameof(CoroutineManager), typeof(CoroutineManager)
                    ).GetComponent<CoroutineManager>();
            return instance;
        }
    }
    internal static void DelayCoroutine(float delayTime, Action action)
    {
        if (action == null)
            return;

        Instance.StartCoroutine(Instance.RunDelayCoroutineCo(delayTime, action));
    }

    private IEnumerator RunDelayCoroutineCo(float delayTime, Action action)
    {
        yield return new WaitForSeconds(delayTime);

        if (action != null)
            action();
    }
}
