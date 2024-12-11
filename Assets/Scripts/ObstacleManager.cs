using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField]
    private ObstacleDataSO obstacleData;
    [SerializeField]
    private GameObject obstaclePrefab;
    [SerializeField]
    private Grid grid;

    private void Start()
    {
        GenerateObstacles();
    }

    private void GenerateObstacles()
    {
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                int index = y * 10 + x;
                if (obstacleData.obstacleGrid[index])
                {
                    Vector3Int cellPosition = new Vector3Int(x - 5, 0, 4 - y);              // Start from -5,4
                    Vector3 worldPosition = grid.CellToWorld(cellPosition);
                    worldPosition += grid.cellSize / 2;
                    Instantiate(obstaclePrefab, worldPosition, Quaternion.identity);
                }
            }
        }
    }
}