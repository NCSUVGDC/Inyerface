using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathSceneManager : MonoBehaviour
{
    public Text LevelReachedText;
    public Text XPEarnedText;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        XPEarnedText.text = "Final Score: " +  player.exp;
        LevelReachedText.text = "Level Reached " + FindObjectOfType<GameManager>().LevelNumber;

        Destroy(player.gameObject);
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

}
