using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class GenerateGrid : MonoBehaviour
{
    [Range(1, 5)]
    [Tooltip("Vertical grid count can be between 1 and 10.")]
    public int horizontalGridCount = 3;

    [Range(1, 10)]
    [Tooltip("Vertical grid count can be between 1 and 5.")]
    public int verticalGridCount = 5;

    public GridCell gridCell;
    public Dictionary<Vector2Int, GridCell> generatedCellDictionary = new Dictionary<Vector2Int, GridCell>();
    public CinemachineTargetGroup targetGroup;

    private const float CELL_GAP = 1.75f;

    private void Start()
    {
        int childCount = transform.childCount;
        int j = -1;

        for(int i = 0; i < childCount; i++)
        {
            if (i % horizontalGridCount == 0)
                j++;

            GridCell cell = transform.GetChild(i).GetComponent<GridCell>();
            cell.Location = new Vector2Int(j, i % horizontalGridCount);

            generatedCellDictionary.Add(cell.Location, cell);
        }
    }

    public void GenerateGrids(int verticalCount, int horizontalCount)
    {
        targetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
        generatedCellDictionary.Clear();

        if(transform.childCount > 0)
        {
            int count = transform.childCount;

            for(int i = 0; i < count; i++)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        for(int i = 0; i < verticalCount; i++)
        {
            float verticalPosition = transform.position.z - i * CELL_GAP;
            for(int j = 0; j < horizontalCount; j++)
            {
                float horizontalPosition = transform.position.x + j * CELL_GAP;
                Vector3 gridCellPosition = new Vector3(horizontalPosition, transform.position.y, verticalPosition);

                GridCell cell = Instantiate(gridCell, gridCellPosition, Quaternion.identity, transform);
                //cell.Location = new Vector2Int(i, j);
                //generatedCellDictionary.Add(cell.Location, cell);
                targetGroup.AddMember(cell.transform, 1, 1);
            }
        }
    }
}
