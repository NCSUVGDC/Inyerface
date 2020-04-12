using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float quantity = 10f;
    public PickupType pickupType;
    public bool UIunlockStartsNextLevel = false;

    public GameObject pickupEffect;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickitup(other);
        }
    }
    void Pickitup(Collider player)
    {
        GameObject effectGO = Instantiate(pickupEffect, transform.position, transform.rotation);
        effectGO.SetActive(true);
        Destroy(effectGO, 3f);

        
        PlayerStats stats = player.GetComponent<PlayerStats>();

        switch (pickupType)
        {
            case PickupType.health:
                stats.addHealth(quantity);
                break;
            case PickupType.shotgunAmmo:
                stats.addShotgunAmmo((int) quantity);
                if(stats.GetComponent<Gun>().DamageType == AgentStats.DamageType.shotgun)
                    stats.ammoCounter?.SetStorageText(stats.shotgunAmmo);
                break;
            case PickupType.pistolAmmo:
                stats.addPistolAmmo((int)quantity);
                if (stats.GetComponent<Gun>().DamageType == AgentStats.DamageType.pistol)
                    stats.ammoCounter?.SetStorageText(stats.pistolAmmo);
                break;
            case PickupType.unlockUI:
                stats.GetComponent<UIUnlockManager>().UnlockAnElement();
                if(UIunlockStartsNextLevel)
                    FindObjectOfType<GameManager>().StartNextLevel();
                break;
            default:
                break;
        }

        Destroy(gameObject);
    }
}

public enum PickupType
{

    health,
    shotgunAmmo,
    pistolAmmo,
    unlockUI
}