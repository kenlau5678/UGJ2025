using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class HealthSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public Image healthBar;   // Ѫ����
    public Image shieldBar;   // ����������ѡ��

    private float maxHealth;
    private float maxShield;

    [Header("Smoothing")]
    public float smoothTime = 0.2f;

    // �������Ѫ��
    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        StartCoroutine(SmoothHealthChange(healthBar, health, maxHealth));
    }

    // ���õ�ǰѪ��
    public void SetHealth(float health)
    {
        StartCoroutine(SmoothHealthChange(healthBar, health, maxHealth));
    }

    // ������󻤶�
    public void SetMaxShield(float shield)
    {
        maxShield = shield;
        //if (shieldBar != null)
        //    StartCoroutine(SmoothHealthChange(shieldBar, shield, maxShield));
    }

    // ���õ�ǰ����
    public void SetShield(float shieldValue)
    {
        if (shieldBar != null)
        {
            // ʹ�� fillAmount ֱ�Ӱ�������ʾ�����Ժ�Ѫ������ͬ�����ֵ
            StartCoroutine(SmoothHealthChange(shieldBar, shieldValue, maxShield));
        }
    }


    // ƽ���仯Э��
    private IEnumerator SmoothHealthChange(Image bar, float targetValue, float maxValue)
    {
        float currentValue = bar.fillAmount * maxValue;
        float elapsedTime = 0f;

        while (elapsedTime < smoothTime)
        {
            elapsedTime += Time.deltaTime;
            bar.fillAmount = Mathf.Lerp(currentValue, targetValue, elapsedTime / smoothTime) / maxValue;
            yield return null;
        }

        // ��ȷ��������ֵ
        bar.fillAmount = targetValue / maxValue;
    }
}
