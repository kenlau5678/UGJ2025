using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class HealthSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public Image healthBar;   // 血量条
    public Image shieldBar;   // 护盾条（可选）

    private float maxHealth;
    private float maxShield;

    [Header("Smoothing")]
    public float smoothTime = 0.2f;

    // 设置最大血量
    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        StartCoroutine(SmoothHealthChange(healthBar, health, maxHealth));
    }

    // 设置当前血量
    public void SetHealth(float health)
    {
        StartCoroutine(SmoothHealthChange(healthBar, health, maxHealth));
    }

    // 设置最大护盾
    public void SetMaxShield(float shield)
    {
        maxShield = shield;
        //if (shieldBar != null)
        //    StartCoroutine(SmoothHealthChange(shieldBar, shield, maxShield));
    }

    // 设置当前护盾
    public void SetShield(float shieldValue)
    {
        if (shieldBar != null)
        {
            // 使用 fillAmount 直接按比例显示，可以和血量条不同步最大值
            StartCoroutine(SmoothHealthChange(shieldBar, shieldValue, maxShield));
        }
    }


    // 平滑变化协程
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

        // 精确设置最终值
        bar.fillAmount = targetValue / maxValue;
    }
}
