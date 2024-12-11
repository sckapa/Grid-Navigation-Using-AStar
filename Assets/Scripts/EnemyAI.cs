using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IAI
{
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private ObstacleDataSO obstacleData;
    [SerializeField]
    private float moveSpeed = 2f;
    [SerializeField]
    private PlayerUnit playerUnit;
    private List<Vector3> path;
    private Vector3Int currentPos;
    public static bool isMoving = false;

    private void Start()
    {
        playerUnit.OnPlayerMoved += PlayerMoved;
        currentPos = grid.WorldToCell(transform.position);
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);            // Added this to make sure the enemy blocks the tile after we're done reading from ObstacleDataSO
        BlockCell(currentPos);                          // Without this delay it would add a obstacle in enemy's cell
    }

    private void OnDestroy()
    {
        playerUnit.OnPlayerMoved -= PlayerMoved;
        UnblockCell(currentPos);
    }

    public void PlayerMoved(Vector3 playerPosition)
    {
        Vector3Int playerGridPosition = grid.WorldToCell(playerPosition);
        if (IsAdjacentToPlayer(playerGridPosition))
        {
            return; 
        }

        Vector3Int targetGridPosition = ClosestAdjacentTile(playerGridPosition);

        MoveTowardsTarget(targetGridPosition);
    }

    public bool IsAdjacentToPlayer(Vector3Int playerGridPosition)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>
        {
            new Vector3Int(playerGridPosition.x + 1, playerGridPosition.y, playerGridPosition.z),
            new Vector3Int(playerGridPosition.x - 1, playerGridPosition.y, playerGridPosition.z),
            new Vector3Int(playerGridPosition.x, playerGridPosition.y, playerGridPosition.z + 1),
            new Vector3Int(playerGridPosition.x, playerGridPosition.y, playerGridPosition.z - 1)
        };

        return adjacentTiles.Contains(currentPos);
    }

    private Vector3Int ClosestAdjacentTile(Vector3Int playerGridPosition)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>
        {
            new Vector3Int(playerGridPosition.x + 1, playerGridPosition.y, playerGridPosition.z),
            new Vector3Int(playerGridPosition.x - 1, playerGridPosition.y, playerGridPosition.z),
            new Vector3Int(playerGridPosition.x, playerGridPosition.y, playerGridPosition.z + 1),
            new Vector3Int(playerGridPosition.x, playerGridPosition.y, playerGridPosition.z - 1)
        };

        Vector3Int closestTile = playerGridPosition;
        float closestDistance = float.MaxValue;

        foreach (var tile in adjacentTiles)
        {
            if (IsValidMove(tile))
            {
                float distance = Vector3.Distance(transform.position, grid.CellToWorld(tile));
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTile = tile;
                }
            }
        }

        return closestTile;
    }

    private bool IsValidMove(Vector3Int gridPosition)
    {
        int index = (4 - gridPosition.z) * 10 + gridPosition.x + 5;

        if (index < 0 || index >= obstacleData.obstacleGrid.Length)
        {
            return false;
        }

        return !obstacleData.obstacleGrid[index];
    }

    public void MoveTowardsTarget(Vector3Int targetPos)
    {
        if(isMoving)
        {
            return;
        }

        AStarPathfinding pathfinding = new AStarPathfinding(obstacleData);
        List<Vector3Int> AstarPath = pathfinding.FindPath(grid.WorldToCell(transform.position), targetPos);

        if (AstarPath != null && AstarPath.Count > 0)
        {
            path = new List<Vector3>();
            foreach (Vector3Int pos in AstarPath)
            {
                Vector3 worldPos = grid.CellToWorld(pos);
                worldPos += grid.cellSize / 2;              // Move to the center of the cell
                path.Add(worldPos);
            }
            StartCoroutine(MoveAlongPath(AstarPath));
        }
    }

    private IEnumerator MoveAlongPath(List<Vector3Int> AstarPath)
    {
        isMoving = true;

        UnblockCell(currentPos); 

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 targetPosition = path[i];
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPosition; 
        }

        currentPos = AstarPath[AstarPath.Count - 1];
        BlockCell(currentPos);

        isMoving = false;
    }

    private void BlockCell(Vector3Int gridPosition)
    {
        int index = (4 - gridPosition.z) * 10 + gridPosition.x + 5;
        obstacleData.obstacleGrid[index] = true;
    }

    private void UnblockCell(Vector3Int gridPosition)
    {
        int index = (4 - gridPosition.z) * 10 + gridPosition.x + 5;
        obstacleData.obstacleGrid[index] = false;
    }
}