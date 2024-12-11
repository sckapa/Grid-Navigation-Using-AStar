using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject tileIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    private void Update()
    {
        Vector3 mousePos = inputManager.HoveredTile();
        Vector3Int tile = grid.WorldToCell(mousePos);

        tileIndicator.transform.position = grid.CellToWorld(tile);
    }
}