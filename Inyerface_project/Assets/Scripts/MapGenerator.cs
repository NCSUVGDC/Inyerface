using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private Arena[,] map = new Arena[5,5];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void GenerateMap()
    {
        System.Random rand = new System.Random();
        int startTile_x = rand.Next(0, 5); //Choose which of the 5 start blocks will be the beginning (0,rand)
        int endTile_x = rand.Next(0, 5);   //Choose end tile (4,rand)

        map[0, startTile_x]._startTile = true;
        map[0, endTile_x]._endTile = true;

        Point nextTile = new Point(0, startTile_x);
        Point endTile = new Point(0, endTile_x);
        while(nextTile != endTile)
        {
            nextTile = ChooseTileType(nextTile, endTile, rand);   
        }

    }
    
    private Point ChooseTileType(Point point, Point exit, System.Random rand) //pass in random to allow seeding
    {
        //mark walls that need to be closed as true
        bool[] closedWalls = new bool[4]; // indexes follow orientation enum

        //mark potential critical path nextTiles
        List<Orientation> critPathCandidates = new List<Orientation>();

        //Check neighbors
        //Checking west neighbor
        if(point._x - 1 >= 0) 
        {
            if (map[point._x - 1, point._y]._arenaType == ArenaType.Unassigned)
            {
                // this neighbor can pass on critical path
                critPathCandidates.Add(Orientation.West);
            }
            else
            {
                //this neighbor cannot pass on critical path
            }
        }
        else
        {
            //this neighbor is on the edge, make sure this edge is closed
            closedWalls[(int)Orientation.West] = true;
        }

        //checking East neighbor
        if (point._x + 1 <= 4)
        {
            if (map[point._x + 1, point._y]._arenaType == ArenaType.Unassigned)
            {
                // this neighbor can pass on critical path
                critPathCandidates.Add(Orientation.East);
            }
            else
            {
                //this neighbor cannot pass on critical path
            }
        }
        else
        {
            //this neighbor is on the edge, make sure this edge is closed
            closedWalls[(int)Orientation.East] = true;

        }

        //checking North neighbor
        if (point._y - 1 >= 0)
        {
            if (map[point._x, point._y - 1]._arenaType == ArenaType.Unassigned)
            {
                // this neighbor can pass on critical path
                critPathCandidates.Add(Orientation.North);
            }
            else
            {
                //this neighbor cannot pass on critical path
            }
        }
        else
        {
            //this neighbor is on the edge, make sure this edge is closed
            closedWalls[(int)Orientation.North] = true;
        }

        //checking South
        if (point._x + 1 <= 4)
        {
            if (map[point._x, point._y + 1]._arenaType == ArenaType.Unassigned)
            {
                // this neighbor can pass on critical path
                critPathCandidates.Add(Orientation.South);
            }
            else
            {
                //this neighbor cannot pass on critical path 
            }
        }
        else
        {
            //this neighbor is on the edge, make sure this edge is closed
            closedWalls[(int)Orientation.South] = true;
        }

        //Choose nextTile position

        //remove if the candidate does not have a path to the exit
        for (int i = 0; i < critPathCandidates.Count; i++)
        {
            bool reachable = false;
            switch (critPathCandidates[i])
            {
                case Orientation.North:
                    reachable = exitReachable(new Point(point._x, point._y - 1), exit);
                    break;
                case Orientation.East:
                    reachable = exitReachable(new Point(point._x + 1, point._y), exit );
                    break;
                case Orientation.South:
                    reachable = exitReachable(new Point(point._x, point._y + 1), exit);
                    break;
                case Orientation.West:
                    reachable = exitReachable(new Point(point._x - 1, point._y), exit);
                    break;
                case Orientation.Unassigned:
                    reachable = false;
                    break;
                default:
                    reachable = false;
                    break;
            }
            if (!reachable)
                critPathCandidates.RemoveAt(i);
        }

        //choose next path and return the point
        int nextCritPath = rand.Next(critPathCandidates.Count);

        Orientation nextDirection = critPathCandidates[nextCritPath];
        switch (nextDirection)
        {
            case Orientation.North:
                return new Point(point._x, point._y - 1);
            case Orientation.East:
                return new Point(point._x, point._y - 1);
            case Orientation.South:
                return new Point(point._x, point._y - 1);
            case Orientation.West:
                return new Point(point._x, point._y - 1);
            case Orientation.Unassigned:
                return null;
            default:
                return null;
        }
    }

    private bool exitReachable(Point tile, Point exit)
    {
        int exitIndex = getOneDimIndex(exit);
        bool[] visited = new bool[25]; //Map the grid to a 1D array

        Queue<int> queue = new Queue<int>();

        //set first tile as visited
        visited[getOneDimIndex(tile)] = true;
        queue.Enqueue(getOneDimIndex(tile));

        while(!(queue.Count == 0))
        {
            int current = queue.Dequeue();
            List<int> neighbors = getUnassignedNeighbors(getTwoDimIndex(current));

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i] == exitIndex)
                    return true;
                else
                {
                    if(!visited[neighbors[i]]) //if this neighbor has not been visited
                    {
                        queue.Enqueue(neighbors[i]);
                    }
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
                // this neighbor can pass on critical path
                unassignedNeighbors.Add(getOneDimIndex(point._x - 1, point._y)); 
            }
        }

        //checking East neighbor
        if (point._x + 1 <= 4)
        {
            if (map[point._x + 1, point._y]._arenaType == ArenaType.Unassigned)
            {
                // this neighbor can pass on critical path
                unassignedNeighbors.Add(getOneDimIndex(point._x + 1, point._y));
            }
        }

        //checking North neighbor
        if (point._y - 1 >= 0)
        {
            if (map[point._x, point._y - 1]._arenaType == ArenaType.Unassigned)
            {
                // this neighbor can pass on critical path
                unassignedNeighbors.Add(getOneDimIndex(point._x, point._y - 1));
            }
        }

        //checking South
        if (point._x + 1 <= 4)
        {
            if (map[point._x, point._y + 1]._arenaType == ArenaType.Unassigned)
            {
                // this neighbor is unassigned neighbor
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
}
public enum ArenaType
{
    Hub,
    T,
    Straight,
    Bend,
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

public class Arena
{
    public ArenaType _arenaType = ArenaType.Unassigned;
    public Orientation _orientation = Orientation.Unassigned;
    public bool _startTile = false;
    public bool _endTile = false;
    //true if closed
    public bool[] _closedEdges = new bool[4] { true, true, true, true }; //follows indexes shown in Orientation enum

}
