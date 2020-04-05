using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    public Arena[,] map = new Arena[5,5];
    public int openWallWeight = 1; //Ratio of open walls to closed walls when deciding if it should be open or close. Higher values give more open

    public float gridSize = 40f;
    public Transform startPoint;

    public bool useSeed;
    public int seed;

    public TileEntry[] hub_prefabs;
    public TileEntry[] t_prefabs;
    public TileEntry[] straight_prefabs;
    public TileEntry[] bend_prefabs;
    public TileEntry[] start_prefabs;
    public TileEntry[] end_prefabs;



    public bool paintCriticalPath = false;
    public Material critPathMaterial;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                map[i, j] = new Arena();

        System.Random rand;
        if (useSeed)
        {
            rand = new System.Random(seed);
        }
        else
            rand = new System.Random();
        if(GenerateMap(rand))        
            PopulateMap(rand);
    }

    private void PopulateMap(System.Random rand)
    {
        Arena currentArena;
        UnityEngine.Vector3 startPosition = startPoint.position;

        for(int x = 0; x < map.GetLength(0); x++)
            for(int y = 0; y < map.GetLength(1); y++)
            {
                currentArena = map[x, y];
                //my grid is in the x-z plane
                UnityEngine.Vector3 spawnLoc = new UnityEngine.Vector3(- (x * gridSize) + startPosition.x, startPosition.z, y * gridSize + startPosition.y );
                UnityEngine.Quaternion spawnRot = new UnityEngine.Quaternion();

                switch (currentArena._orientation)
                {
                    case Orientation.North:
                        spawnRot = UnityEngine.Quaternion.Euler(0, 180, 0);
                        break;
                    case Orientation.East:
                        spawnRot = UnityEngine.Quaternion.Euler(0, 270, 0);
                        break;
                    case Orientation.South:
                        spawnRot = UnityEngine.Quaternion.Euler(0, 0, 0);
                        break;
                    case Orientation.West:
                        spawnRot = UnityEngine.Quaternion.Euler(0, 90, 0);
                        break;
                    case Orientation.Unassigned:
                        spawnRot = spawnRot = UnityEngine.Quaternion.Euler(0, 0, 0);
                        Debug.LogError("Orientation was unassigned when spawning tile");
                        break;
                    default:
                        spawnRot = spawnRot = UnityEngine.Quaternion.Euler(0, 0, 0);
                        Debug.LogError("Orientation was unexpected when spawning tile");
                        break;
                }
                Debug.Log("Arena being spawned: " + currentArena._arenaType.ToString() +  " "  +currentArena._orientation.ToString());
                GameObject spawn = getRandomArena(currentArena._arenaType, rand);
                if (spawn != null)
                {
                    
                    spawn = Instantiate(spawn, spawnLoc, spawnRot);

                    if(currentArena._criticalPath && paintCriticalPath && !currentArena._startTile && !currentArena._endTile)
                    {
                        spawn.GetComponentInChildren<Renderer>().material = critPathMaterial;
                    }
                    spawn.name = "( " + x + " , " + y + " )";
                }
            }
    }

    private GameObject getRandomArena(ArenaType arenaType, System.Random rand)
    {
        switch (arenaType)
        {
            case ArenaType.Hub:
                return weightedChance(ref hub_prefabs,rand);
                
            case ArenaType.T:
                return weightedChance(ref t_prefabs, rand);

            case ArenaType.Straight:
                return weightedChance(ref straight_prefabs, rand);

            case ArenaType.Bend:
                return weightedChance(ref bend_prefabs, rand);

            case ArenaType.Start:
                return weightedChance(ref start_prefabs, rand);

            case ArenaType.End:
                return weightedChance(ref end_prefabs, rand);

            case ArenaType.assigning:
                Debug.LogError("ArenaType is assigning while trying to spawn");
                return null;
                
            case ArenaType.Unassigned:
                Debug.LogError("ArenaType is unassigned while trying to spawn");
                return null;
                
            default:
                Debug.LogError("ArenaType is null while trying to spawn");
                return null;
        }
    }

    private GameObject weightedChance(ref TileEntry[] tileEntries, System.Random rand)
    {
        int total = 0;
        foreach(TileEntry entry in tileEntries)
        {
            total += entry.weight;
        }
        int randomResult = rand.Next(total);
        int sum = 0;
        foreach( TileEntry entry in tileEntries)
        {
            if (randomResult < (sum + entry.weight))
            {
                return entry.prefab;
            }
            else
            {
                sum += entry.weight;
            }
        }
        Debug.LogError("didn't get a weighted chance");
        return null;
    }

    private bool GenerateMap(System.Random rand)
    {
        
        int startTile_x = rand.Next(0, 5); //Choose which of the 5 start blocks will be the beginning (0,rand)
        int endTile_x = rand.Next(0, 5);   //Choose end tile (4,rand)

        map[startTile_x,0]._startTile = true;
        map[endTile_x,4]._endTile = true;

        Point nextTile = new Point( startTile_x, 0);
        Point endTile = new Point(endTile_x, 4);

        map[startTile_x, 0]._criticalPath = true;
        while(nextTile != endTile)
        {
            Point currentTile = nextTile;
            
            
            nextTile = chooseNextTile(currentTile, endTile, rand);
            map[nextTile._x, nextTile._y]._criticalPath = true;
            if(nextTile == new Point(-1,-1))
            {
                Debug.LogError("got stuck");
                return false;
            }
            
            ChooseTileType(currentTile, nextTile, rand);
            Debug.Log("currentTile: " + currentTile.ToString() + " " + map[currentTile._x, currentTile._y]._arenaType.ToString() + " " + map[currentTile._x, currentTile._y]._orientation.ToString() + ", endTile: " + endTile.ToString());
        }

        Debug.Log("Finished critical path");

        foreach(Arena arena in map)
        {
            if(arena._arenaType == ArenaType.Unassigned)
            {
                int type = rand.Next(3);
                arena._arenaType = (ArenaType) type;

                int orientation = rand.Next(3);
                arena._orientation = (Orientation) orientation;

            }
        }
        return true;
    }
    

    private Point chooseNextTile(Point point, Point exit, System.Random rand)
    {
        List<int> neighbors = getUnassignedNeighbors(point);
        List<int> candidates = new List<int>();
        if (neighbors.Count == 0)
            Debug.LogError("No unassigned neighbors");
        for(int i = 0; i < neighbors.Count; i++)
        {
            
            if(exitReachable(getTwoDimIndex(neighbors[i]), exit))
            {
                candidates.Add(neighbors[i]);
            }
        }

        if (candidates.Count == 0)
            return new Point(-1, -1);


        return getTwoDimIndex(candidates[rand.Next(candidates.Count)]); //choose a random candidate

    }

    //Assigns open sides and sets fields in the Arena class that specify orientation and type
    private void ChooseTileType(Point point, Point nextPoint, System.Random rand) //pass in random to allow seeding
    {
        Arena currentArena = map[point._x, point._y];
        Arena nextArena = map[nextPoint._x, nextPoint._y];


        if (nextPoint._x == point._x)
        {
            if(nextPoint._y == point._y + 1)
            {
                //nextpoint is south of point
                currentArena._openEdges[(int)Orientation.South] = true;
                nextArena._openEdges[(int)Orientation.North] = true;
            }
            else
            if (nextPoint._y == point._y - 1)
            {
                //nextPoint is north of point
                currentArena._openEdges[(int)Orientation.North] = true;
               nextArena._openEdges[(int)Orientation.South] = true;
            }
        }
        else
        if(nextPoint._y == point._y)
        {
            if(nextPoint._x == point._x + 1)
            {
                //nextpoint is east of point
                currentArena._openEdges[(int)Orientation.East] = true;
                nextArena._openEdges[(int)Orientation.West] = true;
            }
            else
            if(nextPoint._x == point._x -1 )
            {
                //nextpoint is west of point
                currentArena._openEdges[(int)Orientation.West] = true;
                nextArena._openEdges[(int)Orientation.East] = true;
            }
        }

        int openCount = 0;

        if (nextArena._endTile)
        {
            bool[] openEnd = nextArena._openEdges;
            nextArena._arenaType = ArenaType.End;
            if (openEnd[(int)Orientation.West])
            {
                nextArena._orientation = Orientation.West;
            }
            else
                if (openEnd[(int)Orientation.East])
            {
                nextArena._orientation = Orientation.East;
            }
            else
                if (openEnd[(int)Orientation.North])
            {
                nextArena._orientation = Orientation.North;
            }
            else
                if (openEnd[(int)Orientation.South])
            {
                nextArena._orientation = Orientation.South;
            }

            Debug.Log("setting nextArena to end: " + nextArena._orientation.ToString());
        }

        if (currentArena._startTile)
        {
            currentArena._arenaType = ArenaType.Start;
            openCount = 1;
        }
        else         //If a side is closed, roll to open it
        {
            for (int i = 0; i < 4; i++)
            {
                if (!currentArena._openEdges[i])
                {
                    if (rand.Next(openWallWeight) != 0)
                        currentArena._openEdges[i] = true;
                }

                if (currentArena._openEdges[i])
                    openCount++;
            }
            //if not enough sides are open, pick a valid one to open
            if (openCount == 1)
            {

                if (point._x + 1 <= 4 && !currentArena._openEdges[(int)Orientation.East])
                {
                    currentArena._openEdges[(int)Orientation.East] = true;
                    openCount++;
                }
                else
                if (point._x - 1 >= 0 && !currentArena._openEdges[(int)Orientation.West])
                {
                    currentArena._openEdges[(int)Orientation.West] = true;
                    openCount++;
                }
                else
                if (point._y + 1 <= 4 && !currentArena._openEdges[(int)Orientation.South])
                {
                    currentArena._openEdges[(int)Orientation.South] = true;
                    openCount++;
                }
                else
                if (point._y - 1 >= 0 && !currentArena._openEdges[(int)Orientation.North])
                {
                    currentArena._openEdges[(int)Orientation.North] = true;
                    openCount++;
                }
            }
        }
        bool[] open = currentArena._openEdges;
        switch (openCount)
        {
            case 0:
                Debug.LogError("0 edges open");
                break;
            case 1:
                if(open[(int) Orientation.West])
                {
                    currentArena._orientation = Orientation.West;
                }
                else
                if (open[(int)Orientation.East])
                {
                    currentArena._orientation = Orientation.East;
                }
                else
                if (open[(int)Orientation.North])
                {
                    currentArena._orientation = Orientation.North;
                }
                else
                if (open[(int)Orientation.South])
                {
                    currentArena._orientation = Orientation.South;
                }
                break;
            case 2:
                if(open[(int) Orientation.West])
                {
                    if(open[(int) Orientation.South])
                    {
                        currentArena._orientation = Orientation.North;
                        currentArena._arenaType = ArenaType.Bend;
                    }
                    else
                    if(open[(int) Orientation.North])
                    {
                        currentArena._orientation = Orientation.East;
                        currentArena._arenaType = ArenaType.Bend;
                    }
                    else
                    if (open[(int)Orientation.East])
                    {
                        currentArena._orientation = Orientation.North;
                        currentArena._arenaType = ArenaType.Straight;
                    }
                }
                else
                if (open[(int) Orientation.North])
                {
                    if(open[(int) Orientation.East])
                    {
                        currentArena._orientation = Orientation.South;
                        currentArena._arenaType = ArenaType.Bend;
                    }
                    else
                    if(open[(int) Orientation.South])
                    {
                        currentArena._orientation = Orientation.East;
                        currentArena._arenaType = ArenaType.Straight;
                    }
                }
                if(open[(int) Orientation.South] && open[(int) Orientation.East])
                {
                    currentArena._orientation = Orientation.West;
                    currentArena._arenaType = ArenaType.Bend;
                }
                break;
            case 3:
                currentArena._arenaType = ArenaType.T;
                if (!open[(int)Orientation.North])
                {
                    currentArena._orientation = Orientation.North;
                }
                else
                if (!open[(int)Orientation.East])
                {
                    currentArena._orientation = Orientation.East;
                }
                else
                if (!open[(int)Orientation.South])
                {
                    currentArena._orientation = Orientation.South;
                }
                else
                if (!open[(int)Orientation.West])
                {
                    currentArena._orientation = Orientation.West;
                }
                break;
            case 4:
                currentArena._orientation = Orientation.North;
                currentArena._arenaType = ArenaType.Hub;
                break;
            default:
                Debug.LogError("Open side count not expected");
                break;
        }


    }

    private bool exitReachable(Point tile, Point exit)
    {
        int exitIndex = getOneDimIndex(exit);
        bool[] visited = new bool[25]; //Map the grid to a 1D array

        Queue<int> queue = new Queue<int>();

        //set first tile as visited
        int firstTile = getOneDimIndex(tile);
        visited[firstTile] = true;
        queue.Enqueue(firstTile);
        while(queue.Count != 0)
        {
            int current = queue.Dequeue();
            if (current == exitIndex)
                return true;
            List<int> neighbors = getUnassignedNeighbors(getTwoDimIndex(current));
            for (int i = 0; i < neighbors.Count; i++)
            {

                if (neighbors[i] == exitIndex)
                {
                    return true;
                }
                else
                    if (!visited[neighbors[i]]) //if this neighbor has not been visited
                {
                    visited[neighbors[i]] = true;
                    queue.Enqueue(neighbors[i]);
                }
            }

        }
        return false;
    }

    private int getOneDimIndex(Point point)
    {
        return point._x + (5 * point._y);
    }

    private int getOneDimIndex(int x, int y)
    {
        return x + (5 * y);
    }

    private Point getTwoDimIndex(int index)
    {
        
        int x = index % 5;
        int y = index / 5;
        return new Point(x, y);
    }

    //returns the 1D indexes of unassigned neighbors to this point
    private List<int> getUnassignedNeighbors(Point point)
    {
        List<int> unassignedNeighbors = new List<int>();

        if (point._x - 1 >= 0)
        {
            if (!(map[point._x - 1, point._y]._criticalPath))
            {
               
                unassignedNeighbors.Add(getOneDimIndex(point._x - 1, point._y));
            }
        }

        //checking East neighbor
        if (point._x + 1 <= 4)
        {
            if (!(map[point._x + 1, point._y]._criticalPath))
            {
                
                unassignedNeighbors.Add(getOneDimIndex(point._x + 1, point._y));
            }
        }

        //checking North neighbor
        if (point._y - 1 >= 0)
        {
            
            if (!(map[point._x, point._y - 1]._criticalPath))
            {
                unassignedNeighbors.Add(getOneDimIndex(point._x, point._y - 1));
            }
        }

        //checking South
        if (point._y + 1 <= 4)
        {
            
            if (!(map[point._x, point._y + 1]._criticalPath))
            {
                unassignedNeighbors.Add(getOneDimIndex(point._x, point._y + 1));
            }
        }
        return unassignedNeighbors;
    }
}



public class Point
{
    public int _x;
    public int _y;

    public Point(int x , int y)
    {
        _x = x;
        _y = y;
    }
    public static bool operator ==(Point a, Point b)
    {
        if (a._x == b._x && a._y == b._y)
        {
            return true;
        }
        else
            return false;
    }
    public static bool operator !=(Point a, Point b)
    {
        if (a._x == b._x && a._y == b._y)
        {
            return false;
        }
        else
            return true;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "( " + _x + " , " + _y + " )";
    }
}
public enum ArenaType
{
    Hub,
    T,
    Straight,
    Bend,
    Start,
    End,
    assigning,
    Unassigned
}


//Each tile type has a canonical orientation. The orientation is the way that the
//Canonical orientation arrow is pointing
public enum Orientation
{
    North,
    East,
    South,
    West,
    Unassigned
}

[System.Serializable]
public class Arena
{
    public ArenaType _arenaType = ArenaType.Unassigned;
    public Orientation _orientation = Orientation.Unassigned;
    public bool _startTile = false;
    public bool _endTile = false;
    public bool[] _openEdges = { false, false, false, false };
    public bool _criticalPath = false;
    
}

[System.Serializable]
public class TileEntry
{
    public GameObject prefab;
    [Range(1,100)]
    public int weight = 1;
}
