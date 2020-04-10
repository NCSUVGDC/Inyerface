using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawnPoint : MonoBehaviour
{
    public PickupSpawnData[] pickupSpawns;
    [Range(0,100)]
    [Tooltip("Percentage of the time that an item will spawn")]
    public int dropRate = 25;

    private static System.Random rand = new System.Random();
    private void Start()
    {
        int dropResult = rand.Next(101);
        if(dropResult <= dropRate)
        {
            Instantiate(weightedChance(ref pickupSpawns, rand), transform.position, transform.rotation);
        }
    }

    private GameObject weightedChance(ref PickupSpawnData[] pickupEntries, System.Random rand)
    {
        int total = 0;
        foreach (PickupSpawnData entry in pickupEntries)
        {
            total += entry.weight;
        }
        int randomResult = rand.Next(total);
        float sum = 0;
        foreach (PickupSpawnData entry in pickupEntries)
        {
            if (randomResult < (sum + entry.weight))
            {
                return entry.pickupPrefab;
            }
            else
            {
                sum += entry.weight;
            }
        }
        Debug.LogError("didn't get a weighted chance");
        return null;
    }
}

[System.Serializable]
public class PickupSpawnData
{
    public GameObject pickupPrefab;
    [Range(0f,100f)]
    [Tooltip("If you want this to only spawn one type, set the other's weights to 0")]
    public int weight = 50;
}