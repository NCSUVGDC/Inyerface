using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AgentStats;

public class PlayerStats : MonoBehaviour
{
    [Header("Status")]
    public float currentHealth;
    public bool healthBarUnlocked = false;
    public float currentStamina;
    public bool staminaBarUnlocked = false;
    public float exp;


    [Header("Inventory")]
    public int pistolAmmo;
    public int shotgunAmmo;
    public int maxPistolAmmo;
    public int maxShotgunAmmo;

    [Header("Combat Stats")]
    public float maxHealth;
    public float movementSpeed;
    public float sprintingSpeed;
    public float maxStamina;
    [Tooltip("Hitpoints per second")]
    public float staminaDrainRate;
    public float criticalHitRate;

    [Header("Received Damage Modifiers")]
    [Tooltip("Set this between 0 and 1 to give a damage resistance, >1 for damage weakness")]
    public float pistolDamageModifier = 1.0f;
    [Tooltip("Set this between 0 and 1 to give a damage resistance, >1 for damage weakness")]
    public float shotgunDamageModifier = 1.0f;
    [Tooltip("Set this between 0 and 1 to give a damage resistance, >1 for damage weakness")]
    public float meleeDamageModifier = 1.0f;

    [Header("Damage Output")]
    public const float pistolDamageOutput = 50f;
    public const float shotgunDamageOutput = 75f;
    public const float meleeDamageOutput = 85f;
    public const float basicDamageOutput = 50f;


    public List<HealthBar> healthBars = new List<HealthBar>();

    public AmmoCounter ammoCounter;


    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void ApplyDamage(float damageAmount, DamageType damageType)
    {
        if (healthBarUnlocked)
        {
            switch (damageType)
            {
                case DamageType.pistol:
                    damageAmount *= pistolDamageModifier;
                    break;
                case DamageType.shotgun:
                    damageAmount *= shotgunDamageModifier;
                    break;
                case DamageType.melee:
                    damageAmount *= meleeDamageModifier;
                    break;
                case DamageType.basic:
                //no modifier
                default:
                    break;
            }
            Debug.Log("Taking " + damageAmount + " points of damage");
            currentHealth -= damageAmount;
            foreach(HealthBar healthBar in healthBars)
            {
                healthBar.SetHealth(currentHealth);
            }
            if (currentHealth <= 0f)
            {
                Die();
            }
        }
        else
        {
            Debug.Log("You are immortal so this damage means nothing to you");
        }
    }

    //Use this when this agent is attacking to get the damage value
    public float GetDamageValue(DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.pistol:
                return pistolDamageOutput;
            case DamageType.shotgun:
                return shotgunDamageOutput;
            case DamageType.melee:
                return meleeDamageOutput;
            case DamageType.basic:
                return basicDamageOutput;
            default:
                return 0f;
        }
    }

    public void Die()
    {
        Debug.Log("You Died");   
    }

    public void addHealth(float heal)
    {
        currentHealth += heal;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        foreach(HealthBar healthBar in healthBars)
        {
            healthBar.SetHealth(currentHealth);
        }
    }

    public void addPistolAmmo(int ammo)
    {
        pistolAmmo += ammo;
        if (pistolAmmo > maxPistolAmmo)
        {
            pistolAmmo = maxPistolAmmo;
        }
        
    }
    public void addShotgunAmmo(int ammo)
    {
        shotgunAmmo += ammo;
        if (shotgunAmmo > maxShotgunAmmo)
        {
            shotgunAmmo = maxShotgunAmmo;
        }
    }
}
