using UnityEngine;

public interface IAI
{
    public void MoveTowardsTarget(Vector3Int targetPos);
    public void PlayerMoved(Vector3 playerPosition);
    public bool IsAdjacentToPlayer(Vector3Int playerGridPosition);
}