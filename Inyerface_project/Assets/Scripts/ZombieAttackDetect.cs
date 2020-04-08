using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackDetect : MonoBehaviour
{
    private Animator anim;
    private void Start()
    {
        anim = GetComponentInParent<Animator>();
    }
    private void OnTriggerEnter(Collider collision)
    {
        anim.SetTrigger("Attack");
    }
}
