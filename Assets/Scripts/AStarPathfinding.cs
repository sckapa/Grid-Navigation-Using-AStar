using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    private ObstacleDataSO obstacleData;

    public AStarPathfinding(ObstacleDataSO obstacleData)
    {
        this.obstacleData = obstacleData;
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> openList = new List<Vector3Int>();
        List<Vector3Int> closedList = new List<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, int> gCost = new Dictionary<Vector3Int, int>();
        Dictionary<Vector3Int, int> fCost = new Dictionary<Vector3Int, int>();

        openList.Add(start);
        gCost[start] = 0;
        fCost[start] = GetDistance(start, end);

        while (openList.Count > 0)
        {
            Vector3Int current = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (fCost[openList[i]] < fCost[current] || (fCost[openList[i]] == fCost[current] && gCost[openList[i]] < gCost[current]))
                {
                    current = openList[i];
                }
            }

            openList.Remove(current);
            closedList.Add(current);

            if (current == end)
            {
                return RetracePath(cameFrom, start, end);
            }

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (closedList.Contains(neighbor) || IsObstacle(neighbor))
                {
                    continue;
                }

                int tentativeGCost = gCost[current] + GetDistance(current, neighbor);
                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else if (tentativeGCost >= gCost[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gCost[neighbor] = tentativeGCost;
                fCost[neighbor] = gCost[neighbor] + GetDistance(neighbor, end);
            }
        }

        return null;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int node)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>
        {
            new Vector3Int(node.x + 1, node.y, node.z),
            new Vector3Int(node.x - 1, node.y, node.z),
            new Vector3Int(node.x, node.y, node.z + 1),
            new Vector3Int(node.x, node.y, node.z - 1),
            new Vector3Int(node.x + 1, node.y, node.z + 1), 
            new Vector3Int(node.x - 1, node.y, node.z - 1),
            new Vector3Int(node.x + 1, node.y, node.z - 1),
            new Vector3Int(node.x - 1, node.y, node.z + 1)
        };

        return neighbors;
    }

    private bool IsObstacle(Vector3Int position)
    {
        if (position.x < -5 || position.x > 4 || position.z < -5 || position.z > 4)
        {
            return true;
        }

        int index = (4 - position.z) * 10 + position.x + 5; 
        return obstacleData.obstacleGrid[index];
    }

    private List<Vector3Int> RetracePath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int current = end;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }

    private int GetDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
    }
}