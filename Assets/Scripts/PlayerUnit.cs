using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private ObstacleDataSO obstacleData;
    [SerializeField]
    private float moveSpeed = 2f;
    private List<Vector3> path;
    public delegate void PlayerMoved(Vector3 position);
    public event PlayerMoved OnPlayerMoved;

    private void Update()
    {
        if (EnemyAI.isMoving)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 mousePos = hit.point;
                Vector3Int gridPosition = grid.WorldToCell(mousePos);

                Debug.Log($"{gridPosition.x},{gridPosition.z}");

                if (IsValidMove(gridPosition))
                {
                    AStarPathfinding pathfinding = new AStarPathfinding(obstacleData);
                    List<Vector3Int> pathGridPositions = pathfinding.FindPath(grid.WorldToCell(transform.position), gridPosition);

                    if (pathGridPositions != null && pathGridPositions.Count > 0)
                    {
                        path = new List<Vector3>();
                        foreach (Vector3Int pos in pathGridPositions)
                        {
                            Vector3 worldPos = grid.CellToWorld(pos);
                            worldPos += grid.cellSize / 2;
                            path.Add(worldPos);
                        }
                        StartCoroutine(MoveAlongPath());
                    }
                    else
                    {
                        Debug.Log("No path found.");
                    }
                }
                else
                {
                    Debug.Log("Invalid move.");
                }
            }
        }
    }

    private bool IsValidMove(Vector3Int gridPosition)
    {
        int index = (4 - gridPosition.z) * 10 + gridPosition.x + 5;
        return !obstacleData.obstacleGrid[index];
    }

    private IEnumerator MoveAlongPath()
    {
        EnemyAI.isMoving = true;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 targetPosition = path[i];
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPosition;
        }

        EnemyAI.isMoving = false;
        OnPlayerMoved?.Invoke(transform.position);
    }
}