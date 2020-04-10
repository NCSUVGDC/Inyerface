using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelCounter : MonoBehaviour
{
    public Text levelCounter;
    [Tooltip("Names for the level that will appear alongside the counter")]
    public List<string> levelNames = new List<string>();

    System.Random rand = new System.Random();

    public void Start()
    {
        SetLevelCounter(FindObjectOfType<GameManager>().LevelNumber);
    }
    public void SetLevelCounter(int level)
    {
        int selection = rand.Next(levelNames.Count);

        levelCounter.text = levelNames[selection] + " " + level.ToString();
    }
}
