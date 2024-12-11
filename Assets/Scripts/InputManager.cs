using System;
using TMPro;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private new Camera camera;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private TextMeshProUGUI positionText;
    private Vector3 hitPos;
    public event Action OnClicked, OnExit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }
    }

    public Vector3 HoveredTile()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            hitPos = hit.point;
            int x = Mathf.FloorToInt(hit.point.x);
            int z = Mathf.FloorToInt(hit.point.z);
            positionText.text = $"Tile Coordinates: {x},{z}";
        }
        return hitPos;
    }
}