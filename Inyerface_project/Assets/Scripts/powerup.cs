using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerup : MonoBehaviour
{
    public float addedammo = 10f;

    public GameObject pickupEffect;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup(other);
        }
    }
    void Pickup(Collider player)
    {
        Instantiate(pickupEffect, transform.position, transform.rotation).SetActive(true);
        
        Stats Stats = player.GetComponent<Stats>();
        Stats.ammo += addedammo;

        Destroy(gameObject);
    }
}