using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUnlockManager : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public List<GameObject> LockedUIElements = new List<GameObject>();
    public List<GameObject> UnlockedUIElements = new List<GameObject>();

    private PlayerStats stats;
    private System.Random rand = new System.Random();

    private int sortOrder = 0;
    private void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    public void UnlockAnElement()
    {
        //if it is empty, give a health bar
        if(UnlockedUIElements.Count == 0)
        {
            GameObject spawnedUI = Instantiate(healthBarPrefab,this.transform);
            UnlockedUIElements.Add(spawnedUI);
        }
        else         //otherwise choose a random one
        {
            int index = rand.Next(LockedUIElements.Count);
            if (LockedUIElements.Count == 0)
                return;
            GameObject unlocked = LockedUIElements[index];
            LockedUIElements.RemoveAt(index);
            UnlockedUIElements.Add(unlocked);
            GameObject spawnedUI = Instantiate(unlocked, this.transform);
            spawnedUI.GetComponentInChildren<Canvas>().sortingOrder = ++sortOrder;
            
        }


    }


}
