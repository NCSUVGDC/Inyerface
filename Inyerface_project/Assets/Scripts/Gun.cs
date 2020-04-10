using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public PlayerStats stats;
    public float range;

    public Camera fpsCam;

    int layerMask;

    public AgentStats.DamageType DamageType;

    public GameObject impactEffect;

    [Header("Ammo counts")]
    public int pistolMagazineMax = 12;
    public int shotgunMagazineMax = 8;
    public int currentPistolMagazine;
    public int currentShotgunMagazine;



    private void Start()
    {
        layerMask = LayerMask.GetMask("NonShootable");
        layerMask = ~layerMask;
        currentPistolMagazine = pistolMagazineMax;
        currentShotgunMagazine = shotgunMagazineMax;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        else
        if(Input.GetButtonDown("Reload"))
        {
            ReloadWeapon();
        }
        else
        if(Input.GetButtonDown("WeaponSwap"))
        {
            SwitchWeapon();
        }

    }

    void Shoot()
    {
        RaycastHit hit;
        if (DamageType == AgentStats.DamageType.pistol)
        {
            if (currentPistolMagazine <= 0)
                return;

            currentPistolMagazine--;
            stats.ammoCounter?.SetAmmoCounter(currentPistolMagazine, stats.pistolAmmo);
        }
        else
        if(DamageType == AgentStats.DamageType.shotgun)
        {
            if (currentShotgunMagazine <= 0)
                return;
            currentShotgunMagazine--;
            stats.ammoCounter?.SetAmmoCounter(currentShotgunMagazine, stats.shotgunAmmo);
        }
        else
        {
            return;
        }
        
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, layerMask))
        {
            AgentStats enemyStat = hit.transform.GetComponentInParent<AgentStats>();
            if(enemyStat != null)
                enemyStat.ApplyDamage(stats.GetDamageValue(DamageType), DamageType);

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2.5f);
        }
    }
    private void ReloadWeapon()
    {
        switch (DamageType)
        {
            case AgentStats.DamageType.pistol:
                if(currentPistolMagazine < pistolMagazineMax)
                {
                    bool oneInTheChamber = false;
                    if(currentPistolMagazine > 0)
                    {
                        oneInTheChamber = true;   
                    }
                    if(stats.pistolAmmo <= 0)
                    {
                        //No bullets to reload
                        Debug.Log("Out of ammo");
                    }
                    else
                    if (stats.pistolAmmo < pistolMagazineMax)
                    {
                        currentPistolMagazine = stats.pistolAmmo;
                        stats.pistolAmmo = 0;
                    }
                    if(stats.pistolAmmo >= pistolMagazineMax)
                    {
                        stats.pistolAmmo -= pistolMagazineMax - currentPistolMagazine;
                        currentPistolMagazine = pistolMagazineMax;
                       
                    }

                    if(oneInTheChamber)
                    {
                        currentPistolMagazine++;
                    }

                    stats.ammoCounter?.SetAmmoCounter(currentPistolMagazine, stats.pistolAmmo);
                }
                else
                if(currentPistolMagazine >= pistolMagazineMax)
                {
                    //don't reload
                }

                break;
            case AgentStats.DamageType.shotgun:
                if (currentShotgunMagazine <= 0)
                {
                    currentShotgunMagazine = shotgunMagazineMax;
                }
                else
                if (currentShotgunMagazine >= shotgunMagazineMax)
                {
                    //don't reload
                }
                else
                {
                    //have a bullet in the chamber
                    currentShotgunMagazine = shotgunMagazineMax + 1;
                }
                stats.ammoCounter?.SetAmmoCounter(currentShotgunMagazine, stats.shotgunAmmo);
                break;
            default:
                break;
        }
    }
    private void SwitchWeapon()
    {
        if(DamageType == AgentStats.DamageType.pistol)
        {
            DamageType = AgentStats.DamageType.shotgun;
            stats.ammoCounter?.SetAmmoCounter(currentShotgunMagazine, stats.shotgunAmmo);
        }
        else
        if(DamageType == AgentStats.DamageType.shotgun)
        {
            DamageType = AgentStats.DamageType.pistol;
            stats.ammoCounter?.SetAmmoCounter(currentPistolMagazine, stats.pistolAmmo);
        }
    }


}
