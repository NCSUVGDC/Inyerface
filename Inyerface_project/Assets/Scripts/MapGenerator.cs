using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    public Arena[,] map = new Arena[5,5];
    public int openWallWeight = 1; //Ratio of open walls to closed walls when deciding if it should be open or close. Higher values give more open
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                map[i, j] = new Arena();
        GenerateMap();
    }

    private void GenerateMap()
    {
        System.Random rand = new System.Random();
        int startTile_x = rand.Next(0, 5); //Choose which of the 5 start blocks will be the beginning (0,rand)
        int endTile_x = rand.Next(0, 5);   //Choose end tile (4,rand)

        map[0, startTile_x]._startTile = true;
        map[0, endTile_x]._endTile = true;

        Point nextTile = new Point( startTile_x, 0);
        Point endTile = new Point(endTile_x, 4);
        while(nextTile != endTile)
        {
            Debug.Log("currentTile: " + nextTile.ToString() + ", endTile: " + endTile.ToString() );
            Point currentTile = nextTile;
            nextTile = chooseNextTile(currentTile, endTile, rand);
            if(nextTile == new Point(-1,-1))
            {
                Debug.Log("got stuck");
                break;
            }
            
            ChooseTileType(currentTile, nextTile, rand);
        }

        Debug.Log("Finished choosing layout");
    }
    

    private Point chooseNextTile(Point point, Point exit, System.Random rand)
    {
        List<int> neighbors = getUnassignedNeighbors(point);
        List<int> candidates = new List<int>();

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
       
        //If a side is closed, roll to open it
        int openCount = 0;
        for(int i = 0; i < 4; i++)
        {
            if(!currentArena._openEdges[i])
            {
                if (rand.Next(openWallWeight) != 0)
                    currentArena._openEdges[i] = true;
            }

            if (currentArena._openEdges[i])
                openCount++;
        }
        //if not enough sides are open, pick a valid one to open
        if(openCount == 1)
        {
            
            if(point._x + 1 <= 4 && !currentArena._openEdges[(int)Orientation.East])
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

        bool[] open = currentArena._openEdges;
        switch (openCount)
        {
            case 0:
                Debug.LogError("0 edges open");
                break;
            case 1:
                Debug.LogError("1 edge open");
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

        map[tile._x, tile._y]._arenaType = ArenaType.assigning;

        Queue<int> queue = new Queue<int>();

        //set first tile as visited
        int firstTile = getOneDimIndex(tile);
        visited[firstTile] = true;
        queue.Enqueue(getOneDimIndex(tile));

        while(queue.Count != 0)
        {
            int current = queue.Dequeue();
            if (current == exitIndex)
                return true;
            List<int> neighbors = getUnassignedNeighbors(getTwoDimIndex(current));
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i] == exitIndex)
                    return true;
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
            if (map[point._x - 1, point._y]._arenaType == ArenaType.Unassigned)
            {
               
                unassignedNeighbors.Add(getOneDimIndex(point._x - 1, point._y));
            }
        }

        //checking East neighbor
        if (point._x + 1 <= 4)
        {
            if (map[point._x + 1, point._y]._arenaType == ArenaType.Unassigned)
            {
                
                unassignedNeighbors.Add(getOneDimIndex(point._x + 1, point._y));
            }
        }

        //checking North neighbor
        if (point._y - 1 >= 0)
        {
            
            if (map[point._x, point._y - 1]._arenaType == ArenaType.Unassigned)
            {
                unassignedNeighbors.Add(getOneDimIndex(point._x, point._y - 1));
            }
        }

        //checking South
        if (point._y + 1 <= 4)
        {
            
            if (map[point._x, point._y + 1]._arenaType == ArenaType.Unassigned)
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

    

}
