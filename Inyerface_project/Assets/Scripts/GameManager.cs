using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Tooltip("The level the player is on. This is used for stat tracking and for enemy scaling")]
    public int LevelNumber;
    private MapGenerator mapGen;
    private GameObject player;
    private NavMeshSurface navMesh;
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        mapGen = FindObjectOfType<MapGenerator>();
        mapGen.GenerateLevel();
        FindObjectOfType<PlayerSpawn>().SpawnPlayer();

    }

    public void StartNextLevel()
    {
        LevelNumber++;
        LevelCounter levelText = FindObjectOfType<LevelCounter>();
        if(levelText != null)
        {
            levelText.SetLevelCounter(LevelNumber);
        }
        mapGen.GenerateLevel();
        FindObjectOfType<PlayerSpawn>().SpawnPlayer();
        StartCoroutine(UpdateNavMesh());
    }

    //waits one frame after generating level to generate nav mesh
    private IEnumerator UpdateNavMesh()
    {
        yield return 0;

        FindObjectOfType<NavMeshSurface>().BuildNavMesh();
    }
}
