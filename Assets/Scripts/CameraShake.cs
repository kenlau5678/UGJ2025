using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [Header("默认参数")]
    [SerializeField] private float defaultIntensity = 2f; // 默认震动强度
    [SerializeField] private float defaultTime = 0.5f;    // 默认持续时间
    [SerializeField] private float frequency = 2f;        // 默认频率

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
    /// 用预设值触发震动
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

        //震动结束，恢复为 0.3
        noise.m_AmplitudeGain = 0.3f;
        noise.m_FrequencyGain = 0.3f;

        shakeCoroutine = null;
    }

}
