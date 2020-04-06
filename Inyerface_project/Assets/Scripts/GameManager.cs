using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [Tooltip("The level the player is on. This is used for stat tracking and for enemy scaling")]
    public int LevelNumber;
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
