using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("You entered the kill zone");
            other.GetComponentInParent<PlayerStats>()?.Die();
    }
}
