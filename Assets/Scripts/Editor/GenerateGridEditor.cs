using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateGrid))]
public class GenerateGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GenerateGrid generateGrid = (GenerateGrid)target;

        if(GUILayout.Button("Generate Grid"))
        {
            generateGrid.GenerateGrids(generateGrid.verticalGridCount, generateGrid.horizontalGridCount);
        }
    }
}
