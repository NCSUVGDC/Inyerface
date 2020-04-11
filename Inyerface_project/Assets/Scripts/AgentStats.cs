using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStats : MonoBehaviour
{
    [Header("Combat Stats")]
    public float wanderMovementSpeed = 4f;
    public float attackMovementSpeed = 6f;
    public float baseHealth;
    [Tooltip("Don't set this manually")]
    [SerializeField]
    private float currentHealth;


    [Header("Received Damage Modifiers")]
    [Tooltip("Set this between 0 and 1 to give a damage resistance, >1 for damage weakness")]
    public float pistolDamageModifier = 1.0f;
    [Tooltip("Set this between 0 and 1 to give a damage resistance, >1 for damage weakness")]
    public float shotgunDamageModifier = 1.0f;
    [Tooltip("Set this between 0 and 1 to give a damage resistance, >1 for damage weakness")]
    public float meleeDamageModifier = 1.0f;

    [Header("Damage Output")]
    public float pistolDamageOutput = 50f;
    public float shotgunDamageOutput = 75f;
    public float meleeDamageOutput = 85f;
    public float basicDamageOutput = 50f;


    [Header("Power scaling")]
    [Tooltip("For every level the player clears, base damage gets buffed by this percentage")]
    [Range(0f, 100f)]
    public float damageBuff = 5f;
    [Tooltip("For every level the player clears, base health gets buffed by this percentage")]
    [Range(0f, 100f)]
    public float healthBuff = 5f;
    [Tooltip("For every level the player clears, base attackSpeed gets buffed by this percentage")]
    [Range(0f, 100f)]
    public float attackSpeedBuff = 5f;


    [Header("Item Drops")]
    public ItemDrop[] possibleDrops;
    [Range(0, 100)]
    public int dropRate = 1;
    public int XPAmount = 100;

    private static System.Random rand = new System.Random();


    public enum DamageType
    {
        pistol,
        shotgun, 
        melee,
        basic
    }

    private GameManager gmRef;

    public void Start()
    {
        gmRef = FindObjectOfType<GameManager>();
        currentHealth = baseHealth;
        currentHealth += gmRef.LevelNumber * baseHealth * (healthBuff / 100f); //increase currentHealth by baseHealth and damage buff
        pistolDamageOutput += (gmRef.LevelNumber * pistolDamageOutput * (damageBuff / 100f));
        shotgunDamageOutput += (gmRef.LevelNumber * shotgunDamageOutput * (damageBuff / 100f));
        meleeDamageOutput += (gmRef.LevelNumber * meleeDamageOutput * (damageBuff / 100f));
        attackMovementSpeed += (gmRef.LevelNumber * attackMovementSpeed * (attackSpeedBuff / 100f));
        Debug.Log("currentHealth: " + currentHealth + " Base health " + baseHealth);
    }

    public void ApplyDamage(float damageAmount, DamageType damageType, PlayerStats player)
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

        currentHealth -= damageAmount;
        Debug.Log("Current Health: " + currentHealth + " damageAmount " + damageAmount);
        if(currentHealth <= 0f)
        {
            Die(player);
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

    public void Die(PlayerStats player)
    {
        //Decide to drop
        //this just gets random float

        if (player != null)
            player.AddExp(XPAmount);

        int chance = rand.Next(101);
        if (chance <= dropRate)
        {
            
            //Decide what to drop
            GameObject drop = randomDrop();
            if (drop != null)
            {
                Instantiate(drop, transform.position,transform.rotation);
            }
        }

        GameObject.Destroy(gameObject);
    }

    private GameObject randomDrop()
    {
        int total = 0;
        foreach (ItemDrop entry in possibleDrops)
        {
            total += entry.weight;
        }
        int randomResult = rand.Next(total);
        int sum = 0;
        foreach (ItemDrop entry in possibleDrops)
        {
            if (randomResult < (sum + entry.weight))
            {
                return entry.itemDropPrefab;
            }
            else
            {
                sum += entry.weight;
            }
        }
        Debug.LogError("didn't get a randomDrop from randomDrop()");
        return null;
    }
}

[System.Serializable]
public class ItemDrop
{

    public GameObject itemDropPrefab;
    [Range(0,100)]
    public int weight;

}

