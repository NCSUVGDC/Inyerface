using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject playerPrefab;

    public GameObject SpawnPlayer()
    {
        GameObject Player = FindObjectOfType<PlayerStats>()?.gameObject;

        if(Player == null)
        {
            Instantiate(playerPrefab, this.transform.position, this.transform.rotation);
        }
        else
        {
            Debug.Log("Spawning "+ Player.name + " at: " + transform.position.ToString());

            CharacterController cc = Player.GetComponent<CharacterController>();
            cc.enabled = false;
            Player.transform.position = transform.position;
            Player.transform.rotation = transform.rotation;
            cc.enabled = true;
        }
        return Player;
    }
}
