using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectRoot : MonoBehaviour
{
    private static LevelSelectRoot instance;

    private void Awake()
    {
        // 单例判断，防止重复
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 切场景不销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
