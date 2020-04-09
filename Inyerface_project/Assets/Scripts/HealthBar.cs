using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider slider;

    PlayerStats stats;
    [Tooltip("The amount of health this will add when it is unlocked")]
    public float health = 100f;

    private void Start()
    {
        stats = FindObjectOfType<PlayerStats>();
        if(stats.healthBarUnlocked)
        {
            stats.maxHealth += health;
            SetMaxHealth(stats.maxHealth);
        }
        else
        {
            stats.healthBarUnlocked = true;
            stats.addHealth(stats.maxHealth);
        }
    }
    public void SetHealth(float h)
    {
        slider.value = h;
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
}
