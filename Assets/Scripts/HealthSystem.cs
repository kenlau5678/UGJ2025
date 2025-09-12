using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public Image healthBar;
    private float maxHealth;
    void Start()
    {

    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        StartCoroutine(SmoothHealthChange(health));
    }

    // Update is called once per frame
    public float smoothTime = 0.2f;

    // Update the health bar smoothly
    public void SetHealth(float health)
    {
        StartCoroutine(SmoothHealthChange(health));
    }

    private IEnumerator SmoothHealthChange(float targetHealth)
    {
        float currentHealth = healthBar.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < smoothTime)
        {
            elapsedTime += Time.deltaTime;
            healthBar.fillAmount = Mathf.Lerp(currentHealth, (targetHealth / maxHealth), elapsedTime / smoothTime);
            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set precisely
        //healthBar.fillAmount = targetHealth;
    }
}
