using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum ArenaType
{
    Hub,
    T,
    Straight,
    Bend
}


//Each tile type has a canonical orientation. The orientation is the way that the
//Canonical orientation arrow is pointing
public enum Orientation
{
    North,
    East,
    South,
    West
}
