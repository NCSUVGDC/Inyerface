using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDamager : MonoBehaviour
{
    private AgentStats stats;

    private void Start()
    {
        stats = GetComponentInParent<AgentStats>();
    }


    private void OnTriggerEnter(Collider collision)
    {
        PlayerStats enemystats = collision.gameObject.GetComponentInParent<PlayerStats>();
        if(enemystats != null)
            enemystats.ApplyDamage(stats.GetDamageValue(AgentStats.DamageType.melee), AgentStats.DamageType.melee);
    }
}
