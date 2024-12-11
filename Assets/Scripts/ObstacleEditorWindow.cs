using UnityEditor;
using UnityEngine;

public class ObstacleEditorWindow : EditorWindow
{
    private ObstacleDataSO obstacleData;

    [MenuItem("Tools/Obstacle Editor")]
    public static void ShowWindow()
    {
        GetWindow<ObstacleEditorWindow>("Obstacle Editor");
    }

    private void OnGUI()
    {
        obstacleData = (ObstacleDataSO)EditorGUILayout.ObjectField("Obstacle Data", obstacleData, typeof(ObstacleDataSO), false);

        if (obstacleData == null)
        {
            EditorGUILayout.HelpBox("Please assign an Obstacle Data ScriptableObject.", MessageType.Warning);
            return;
        }

        for (int y = 0; y < 10; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < 10; x++)
            {
                int index = y * 10 + x;
                obstacleData.obstacleGrid[index] = GUILayout.Toggle(obstacleData.obstacleGrid[index], GUIContent.none, GUILayout.Width(20), GUILayout.Height(20));
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(obstacleData);
            AssetDatabase.SaveAssets();
        }
    }
}