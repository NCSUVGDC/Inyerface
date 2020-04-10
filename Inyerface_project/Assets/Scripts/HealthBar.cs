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

    //This health bar only tracks the 100 health that it added. The next bar will drain when this one is done
    private float previousHealth;

    private void Start()
    {
        Cursor.visible = false;

        stats = FindObjectOfType<PlayerStats>();
        previousHealth = stats.healthBars.Count * health;
        stats.healthBars.Add(this);
        SetMaxHealth(health);



        if (stats.healthBarUnlocked)
        {
            stats.maxHealth += health;
            stats.addHealth(health);
            SetHealth(stats.currentHealth);
        }
        else
        {
            stats.healthBarUnlocked = true;
            stats.maxHealth = health;
            stats.addHealth(stats.maxHealth);
            SetHealth(stats.currentHealth);
        }
    }
    public void SetHealth(float h)
    {
        slider.value = h - previousHealth;
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
    }
}
