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
    public bool generateLevel = true;

    private System.Random rand = new System.Random();

    public Material[] skyBoxes;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        Cursor.visible = false;
        if (generateLevel)
        {
            ChooseRandomSkybox();
            mapGen = FindObjectOfType<MapGenerator>();
            mapGen.GenerateLevel();
            FindObjectOfType<PlayerSpawn>().SpawnPlayer();
        }
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("EditorOnly")) Destroy(go);

    }

    public void StartNextLevel()
    {
        LevelNumber++;
        ChooseRandomSkybox();
        XPCounter xpCounter = FindObjectOfType<XPCounter>();
        if(xpCounter!= null)
        {
            xpCounter.SetXPName();
        }
        LevelCounter levelText = FindObjectOfType<LevelCounter>();
        if(levelText != null)
        {
            levelText.SetLevelCounter(LevelNumber);
        }
        Pickup[] pickups = FindObjectsOfType<Pickup>();
        foreach(Pickup pick in pickups)
        {
            Destroy(pick.gameObject);
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

    private void ChooseRandomSkybox()
    {
        int result = rand.Next(skyBoxes.Length);
        RenderSettings.skybox = skyBoxes[result];
    }
}
