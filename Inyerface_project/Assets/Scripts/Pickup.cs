using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float quantity = 10f;
    public PickupType pickupType;

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
        Instantiate(pickupEffect, transform.position, transform.rotation).SetActive(true);
        
        PlayerStats stats = player.GetComponent<PlayerStats>();
        switch (pickupType)
        {
            case PickupType.health:
                stats.addHealth(quantity);
                break;
            case PickupType.shotgunAmmo:
                stats.addShotgunAmmo((int) quantity);
                break;
            case PickupType.pistolAmmo:
                stats.addPistolAmmo((int)quantity);
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
    pistolAmmo
}