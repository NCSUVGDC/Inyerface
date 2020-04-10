using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCounter : MonoBehaviour
{
    // Start is called before the first frame update

    public Text clipText;
    public Text storageText;

    public void Start()
    {
        Gun gun = FindObjectOfType<Gun>();
        gun.stats.ammoCounter = this;
        switch (gun.DamageType)
        {
            case AgentStats.DamageType.pistol:
                SetAmmoCounter(gun.currentPistolMagazine, gun.stats.pistolAmmo);
                break;
            case AgentStats.DamageType.shotgun:
                SetAmmoCounter(gun.currentShotgunMagazine, gun.stats.shotgunAmmo);
                break;
            case AgentStats.DamageType.melee:
                break;
            case AgentStats.DamageType.basic:
                break;
            default:
                break;
        }


    }

    public void SetClipText(int clipStorage)
    {
        clipText.text = clipStorage.ToString();
    }
    public void SetStorageText(int ammoStorage)
    {
        storageText.text = ammoStorage.ToString();
    }

    public void SetAmmoCounter(int clipStorage, int ammoStorage)
    {
        SetClipText(clipStorage);
        SetStorageText(ammoStorage);
    }
}
