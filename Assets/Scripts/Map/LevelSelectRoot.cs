using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectRoot : MonoBehaviour
{
    private static LevelSelectRoot instance;

    private void Awake()
    {
        // �����жϣ���ֹ�ظ�
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �г���������
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
