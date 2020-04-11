using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public PlayerStats stats;
    

    public Camera fpsCam;

    int layerMask;

    public AgentStats.DamageType DamageType;

    public GameObject impactEffect;

    [Header("Ammo counts")]
    public int pistolMagazineMax = 12;
    public int shotgunMagazineMax = 8;
    public int currentPistolMagazine;
    public int currentShotgunMagazine;

    [Header("Gun stats")]
    public int shotgunPellets = 10;
    public float shotgunRange = 20;
    public float shotgunVariance;
    public float pistolRange;
    public float shotgunNoiseRange;
    public float pistolNoiseRange;

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
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, pistolNoiseRange);

            foreach (Collider col in hitColliders)
            {
                ZombieController enemy = col.GetComponent<ZombieController>();
                if (enemy != null)
                {
                    enemy.Alert(this.gameObject);
                }
            }

            if (currentPistolMagazine <= 0)
                return;

            currentPistolMagazine--;
            stats.ammoCounter?.SetAmmoCounter(currentPistolMagazine, stats.pistolAmmo);


            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, pistolRange, layerMask))
            {
                AgentStats enemyStat = hit.transform.GetComponentInParent<AgentStats>();
                if (enemyStat != null)
                {
                    enemyStat.ApplyDamage(stats.GetDamageValue(DamageType), DamageType, stats);
                }

                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2.5f);
            }

        }
        else
        if(DamageType == AgentStats.DamageType.shotgun)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, shotgunNoiseRange);

            foreach(Collider col in hitColliders)
            {
                ZombieController enemy = col.GetComponent<ZombieController>();
                if (enemy != null)
                {
                    enemy.Alert(this.gameObject);
                }
            }

            if (currentShotgunMagazine <= 0)
                return;
            currentShotgunMagazine--;
            stats.ammoCounter?.SetAmmoCounter(currentShotgunMagazine, stats.shotgunAmmo);
            RaycastHit shotHit;

            for(int i = 0; i < shotgunPellets; i++)
            {
                var v3Offset = transform.up * Random.Range(0.0f, shotgunVariance);
                v3Offset = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), transform.forward) * v3Offset;
                Vector3 v3Hit = fpsCam.transform.forward * shotgunRange + v3Offset;


                if (Physics.Raycast(fpsCam.transform.position, v3Hit, out shotHit))
                {
                    AgentStats enemyStat = shotHit.transform.GetComponentInParent<AgentStats>();
                    if (enemyStat != null)
                    {
                        Debug.Log("Damage output: " + stats.GetDamageValue(DamageType));
                        enemyStat.ApplyDamage(stats.GetDamageValue(DamageType), DamageType, stats);
                    }
                }

                GameObject impactGO = Instantiate(impactEffect, shotHit.point, Quaternion.LookRotation(shotHit.normal));
                Destroy(impactGO, 2.5f);
            }
        }
        else
        {
            return;
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
                if (currentShotgunMagazine < shotgunMagazineMax)
                {
                    bool oneInTheChamber = false;
                    if (currentShotgunMagazine > 0)
                    {
                        oneInTheChamber = true;
                    }
                    if (stats.shotgunAmmo <= 0)
                    {
                        //No bullets to reload
                        Debug.Log("Out of ammo");
                    }
                    else
                    if (stats.shotgunAmmo < shotgunMagazineMax)
                    {
                        currentShotgunMagazine = stats.shotgunAmmo;
                        stats.shotgunAmmo = 0;
                    }
                    if (stats.shotgunAmmo >= shotgunMagazineMax)
                    {
                        stats.shotgunAmmo -= shotgunMagazineMax - currentShotgunMagazine;
                        currentShotgunMagazine = shotgunMagazineMax;

                    }

                    if (oneInTheChamber)
                    {
                        currentShotgunMagazine++;
                    }

                    stats.ammoCounter?.SetAmmoCounter(currentShotgunMagazine, stats.shotgunAmmo);
                }
                else
                if (currentShotgunMagazine >= shotgunMagazineMax)
                {
                    //don't reload
                }
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
