using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ZombieController : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent agent;
    public ZombieStates state = ZombieStates.Wander;


    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
    }


    void Update()
    {
        switch (state)
        {
            case ZombieStates.Wander:
                break;
            case ZombieStates.Attack:
                agent.SetDestination(player.transform.position);
                break;
            default:
                break;
        }
    }

    public enum ZombieStates
    {
        Wander,
        Attack
    }
}
