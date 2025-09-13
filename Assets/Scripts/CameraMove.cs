using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // 虚拟摄像机
    public float moveSpeed = 2f;                   // 移动速度
    public float offsetLimit = 3f;                 // 相对初始offset的最大偏移
    public float returnSpeed = 5f;                 // 返回初始offset的平滑速度

    private CinemachineTransposer transposer;
    private Vector3 initialOffset;  // 初始offset
    private Vector3 currentOffset;

    void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        initialOffset = transposer.m_FollowOffset; // 记录初始offset
        currentOffset = initialOffset;
    }

    void Update()
    {
        // 获取输入
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // 检查是否有输入
        if (moveX != 0f || moveY != 0f)
        {
            // 计算移动量
            currentOffset += new Vector3(moveX, moveY, 0f) * moveSpeed * Time.deltaTime;

            // 根据初始offset计算动态边界
            currentOffset.x = Mathf.Clamp(currentOffset.x, initialOffset.x - offsetLimit, initialOffset.x + offsetLimit);
            currentOffset.y = Mathf.Clamp(currentOffset.y, initialOffset.y - offsetLimit, initialOffset.y + offsetLimit);
        }
        else
        {
            // 无输入时平滑返回初始offset
            currentOffset = Vector3.Lerp(currentOffset, initialOffset, returnSpeed * Time.deltaTime);
        }

        // 应用offset
        transposer.m_FollowOffset = currentOffset;
    }
}