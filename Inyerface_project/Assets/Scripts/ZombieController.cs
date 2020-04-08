using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ZombieController : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent agent;
    public ZombieStates state = ZombieStates.Wander;
    public float wanderRadius;
    public float wanderReachedRadius = .5f;
    [Tooltip("Agent enters attack mode if an enemy appears in this radius")]
    public float alertRadius = 25f;
    private GameObject targetEnemy;
    public float attackRange = 10f;

    public Collider attackInRangeDetector;
    public Collider hitBox;

    private Vector3 wanderPosition;
    private bool wanderingToPosition = false;
    private IFFTag.IFF iff_channel;
    private AgentStats stats;
    private Animator anim;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        iff_channel = GetComponent<IFFTag>().IFF_channel;
        stats = GetComponent<AgentStats>();
        anim = GetComponent<Animator>();
        anim.SetInteger("ZombieState", 0);
    }


    void Update()
    {
        switch (state)
        {
            case ZombieStates.Wander:
                //search for someone to attack or a place to wander
                Wander();
                
                break;
            case ZombieStates.Pursuit:
                Pursuit();
                
                break;
            default:
                break;
        }
    }




    private void Pursuit()
    {
        agent.SetDestination(targetEnemy.transform.position);
    }

    private void Wander()
    {
        //continually search for enemy to attack
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, alertRadius);

        int highestAggroPriority = -1;
        foreach(Collider hit in hitColliders)
        {
            IFFTag otherAgent = hit.GetComponentInParent<IFFTag>();
            if (otherAgent == null)
                continue;
            if(otherAgent.IFF_channel != iff_channel)
            {
                if(otherAgent.aggroPriority >= highestAggroPriority)
                {
                    targetEnemy = otherAgent.gameObject;
                }
            }
        }

        //if previous goal has been achieved (enemy death or wander point reached) then make a new wander point
        if (targetEnemy == null)
        {
            if (wanderingToPosition)
            {
                //check if position has been reached
                wanderPosition.y = transform.position.y;
                if (Vector3.Distance(wanderPosition, transform.position) <= wanderReachedRadius)
                {
                    wanderingToPosition = false;
                }
            }
            
            //select new wander position
            if(!wanderingToPosition)
            {
                wanderPosition = RandomNavmeshLocation(wanderRadius);
                Debug.DrawRay(wanderPosition, Vector3.up * 10f, Color.blue, 10f);
                wanderingToPosition = true;
                agent.SetDestination(wanderPosition);
                agent.speed = stats.wanderMovementSpeed;
            }
        }
        else
        {
            wanderingToPosition = false;
            state = ZombieStates.Pursuit;
            anim.SetInteger("ZombieState", (int)ZombieStates.Pursuit);
            agent.speed = stats.attackMovementSpeed;
        }
    }

    public enum ZombieStates
    {
        Wander,
        Pursuit
    }
    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
