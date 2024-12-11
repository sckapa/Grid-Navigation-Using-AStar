using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "ScriptableObjects/ObstacleDataSO")]
public class ObstacleDataSO : ScriptableObject
{
    public bool[] obstacleGrid = new bool[100]; 
}