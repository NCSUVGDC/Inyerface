using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class XPCounter : MonoBehaviour
{
    public Text XPtext;
    [Tooltip("Names for the level that will appear alongside the counter")]
    public List<string> xpNames = new List<string>();
    public string currentXPname;

    System.Random rand = new System.Random();

    private void Start()
    {
        PlayerStats stats = FindObjectOfType<PlayerStats>();
        stats.XPCounter = this;
        SetXPName();
        SetXPCounter(stats.exp);
    }

    public void SetXPCounter(int XP)
   {
        XPtext.text = currentXPname + " " + XP.ToString();
    }

    public void SetXPName()
    {
        int selection = rand.Next(xpNames.Count);
        currentXPname = xpNames[selection];
    }
}
