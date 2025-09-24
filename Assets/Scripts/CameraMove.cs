using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // ���������
    public float moveSpeed = 2f;                   // �ƶ��ٶ�
    public float offsetLimit = 3f;                 // ��Գ�ʼoffset�����ƫ��
    public float returnSpeed = 5f;                 // ���س�ʼoffset��ƽ���ٶ�

    private CinemachineTransposer transposer;
    private Vector3 initialOffset;  // ��ʼoffset
    private Vector3 currentOffset;
    private Transform currentTarget;

    void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        initialOffset = transposer.m_FollowOffset; // ��¼��ʼoffset
        currentOffset = initialOffset;
        currentTarget = virtualCamera.Follow;      // ��¼��ʼ�������
    }

    void Update()
    {
        // ��ȡ����
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // ����Ƿ�������
        if (moveX != 0f || moveY != 0f)
        {
            // �����ƶ���
            currentOffset += new Vector3(moveX, moveY, 0f) * moveSpeed * Time.deltaTime;

            // ���ݳ�ʼoffset���㶯̬�߽�
            currentOffset.x = Mathf.Clamp(currentOffset.x, initialOffset.x - offsetLimit, initialOffset.x + offsetLimit);
            currentOffset.y = Mathf.Clamp(currentOffset.y, initialOffset.y - offsetLimit, initialOffset.y + offsetLimit);
        }
        else
        {
            // ������ʱƽ�����س�ʼoffset
            currentOffset = Vector3.Lerp(currentOffset, initialOffset, returnSpeed * Time.deltaTime);
        }

        // Ӧ��offset
        transposer.m_FollowOffset = currentOffset;
    }

    /// <summary>
    /// ��̬�л��������
    /// </summary>
    /// <param name="target">�µĸ���Ŀ��</param>
    public void ChangeFollow(GameObject target)
    {
        if (target != null)
        {
            virtualCamera.Follow = target.transform;
            currentTarget = target.transform;

            // ��ѡ���л�Ŀ��ʱ����ƫ��
            currentOffset = initialOffset;
        }
    }

    /// <summary>
    /// ��ȡ��ǰ�������
    /// </summary>
    public Transform GetCurrentFollow()
    {
        return currentTarget;
    }
}
