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

    public ShotgunAnimations shotgunAnimations;
    public ShotgunAnimations pistolAnimations;

    public GameObject shotgunModel;
    public GameObject pistolModel;

    [HideInInspector]
    public AudioManager audio;

    public Vector3 pistolADSPos;
    public Vector3 pistolADSRot;
    public Vector3 pistolHipPos;
    public Vector3 pistolHipRot;
    public Vector3 shotgunADSPos;
    public Vector3 shotgunADSRot;
    public Vector3 shotgunHipPos;
    public Vector3 shotgunHipRot;



    public float pistolADSFOV;
    public float shotgunADSFOV;
    public float normalFOV;

    public Transform pistolTransform;
    public Transform shotgunTransform;

    

    private void Start()
    {
        layerMask = LayerMask.GetMask("NonShootable");
        layerMask = ~layerMask;
        currentPistolMagazine = pistolMagazineMax;
        currentShotgunMagazine = shotgunMagazineMax;
        audio = FindObjectOfType<AudioManager>();
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

        if(Input.GetButton("Fire2") && !shotgunAnimations.reloading)
        {
            switch (DamageType)
            {
                case AgentStats.DamageType.pistol:
                    fpsCam.fieldOfView = pistolADSFOV;
                    break;
                case AgentStats.DamageType.shotgun:

                    fpsCam.fieldOfView = shotgunADSFOV;
                    break;
                case AgentStats.DamageType.melee:
                    break;
                case AgentStats.DamageType.basic:
                    break;
                default:
                    break;
            }
            pistolTransform.localPosition = pistolADSPos;
            pistolTransform.localRotation = Quaternion.Euler(pistolADSRot);
            shotgunTransform.localPosition = shotgunADSPos;
            shotgunTransform.localRotation = Quaternion.Euler(shotgunADSRot);
        }
        else
        {
            pistolTransform.localPosition = pistolHipPos;
            pistolTransform.localRotation = Quaternion.Euler(pistolHipRot);
            shotgunTransform.localPosition = shotgunHipPos;
            shotgunTransform.localRotation = Quaternion.Euler(shotgunHipRot);
            fpsCam.fieldOfView = normalFOV;
        }
    }

    void Shoot()
    {
        


        RaycastHit hit;
        if (DamageType == AgentStats.DamageType.pistol)
        {
            if (pistolAnimations.shooting || pistolAnimations.reloading)
            {
                Debug.Log("Cannot shoot while shooting or reloading");
                return;
            }

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

            audio.Play("PistolShot");
            currentPistolMagazine--;
            pistolAnimations.anim.SetInteger("RoundsLoaded", currentPistolMagazine);
            pistolAnimations.anim.SetTrigger("Fire");

            
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
            if (shotgunAnimations.shooting || shotgunAnimations.reloading)
            {
                Debug.Log("Cannot shoot while shooting or reloading");
                return;
            }

            /////////////////////////////Alert Nearby agents////////////////////////////////////////
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, shotgunNoiseRange);

            foreach(Collider col in hitColliders)
            {
                ZombieController enemy = col.GetComponent<ZombieController>();
                if (enemy != null)
                {
                    enemy.Alert(this.gameObject);
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////

            if (currentShotgunMagazine <= 0)
                return;

            //Start shooting animation
            shotgunAnimations.anim.SetInteger("RoundsLoaded", currentShotgunMagazine);
            shotgunAnimations.anim.SetTrigger("Fire");
            audio.Play("ShotgunShot");
            currentShotgunMagazine--;
            stats.ammoCounter?.SetAmmoCounter(currentShotgunMagazine, stats.shotgunAmmo);
            RaycastHit shotHit;

            for(int i = 0; i < shotgunPellets; i++)
            {
                var v3Offset = transform.up * Random.Range(0.0f, shotgunVariance);
                v3Offset = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), transform.forward) * v3Offset;
                Vector3 v3Hit = fpsCam.transform.forward * shotgunRange + v3Offset;


                if (Physics.Raycast(fpsCam.transform.position, v3Hit, out shotHit,shotgunRange, layerMask))
                {
                    AgentStats enemyStat = shotHit.transform.GetComponentInParent<AgentStats>();
                    if (enemyStat != null)
                    {
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

                if (pistolAnimations.shooting || pistolAnimations.reloading)
                {
                    Debug.Log("Cannot reload while shooting or reloading");
                    return;
                }
                if (currentPistolMagazine < pistolMagazineMax)
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
                        return;
                    }
                    pistolAnimations.anim.SetInteger("RoundsLoaded", currentPistolMagazine);
                    if (currentPistolMagazine < 12)
                        pistolAnimations.anim.SetTrigger("Reload");


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


                    if (oneInTheChamber)
                    {
                        currentPistolMagazine++;
                    }


                    int temp = currentPistolMagazine;
                    pistolAnimations.anim.SetTrigger("Reload");
                    pistolAnimations.anim.SetInteger("RoundsLoaded", temp);

                    stats.ammoCounter?.SetAmmoCounter(currentPistolMagazine, stats.pistolAmmo);
                }
                else
                if(currentPistolMagazine >= pistolMagazineMax)
                {
                    //don't reload
                }

                break;
            case AgentStats.DamageType.shotgun:
                if (shotgunAnimations.shooting || shotgunAnimations.reloading)
                {
                    Debug.Log("Cannot reload while shooting or reloading");
                    return;
                }


                if (currentShotgunMagazine < shotgunMagazineMax)
                {
                    int temp = currentShotgunMagazine;
                    shotgunAnimations.anim.SetTrigger("Reload");
                    shotgunAnimations.anim.SetInteger("RoundsLoaded", temp);

                    
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
            //shotgunAnimations.shotgunAnim.SetInteger("RoundsLoaded", currentShotgunMagazine);
            shotgunAnimations.reloading = false;
            shotgunAnimations.shooting = false;
            shotgunModel.SetActive(true);
            shotgunAnimations.anim.SetTrigger("Swap");
            pistolModel.SetActive(false);
        }
        else
        if(DamageType == AgentStats.DamageType.shotgun)
        {
            DamageType = AgentStats.DamageType.pistol;
            stats.ammoCounter?.SetAmmoCounter(currentPistolMagazine, stats.pistolAmmo);
            pistolAnimations.reloading = false;
            pistolAnimations.shooting = false;
            shotgunModel.SetActive(false);
            shotgunAnimations.anim.SetTrigger("Sheathe");
            pistolModel.SetActive(true);
        }
    }
}

