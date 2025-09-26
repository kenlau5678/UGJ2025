using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [Header("Ĭ�ϲ���")]
    [SerializeField] private float defaultIntensity = 2f; // Ĭ����ǿ��
    [SerializeField] private float defaultTime = 0.5f;    // Ĭ�ϳ���ʱ��
    [SerializeField] private float frequency = 2f;        // Ĭ��Ƶ��

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    /// <summary>
    /// ��Ԥ��ֵ������
    /// </summary>
    public void Shake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(DoShake(defaultIntensity, defaultTime));
    }

    private IEnumerator DoShake(float intensity, float time)
    {
        if (noise == null) yield break;

        noise.m_AmplitudeGain = intensity;
        noise.m_FrequencyGain = frequency;

        yield return new WaitForSeconds(time);

        //�𶯽������ָ�Ϊ 0.3
        noise.m_AmplitudeGain = 0.3f;
        noise.m_FrequencyGain = 0.3f;

        shakeCoroutine = null;
    }

}
