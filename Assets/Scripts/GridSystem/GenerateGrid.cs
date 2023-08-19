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

    [Header("Road Prefabs")]
    [SerializeField] GameObject leftTopCorner;
    [SerializeField] GameObject otherCorners;
    [SerializeField] GameObject straightRoad;
    [SerializeField] Transform roadParent;

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

        ClearParentObjects();

        for (int i = 0; i < verticalCount; i++)
        {
            float verticalPosition = transform.position.z - i * CELL_GAP;
            for (int j = 0; j < horizontalCount; j++)
            {
                float horizontalPosition = transform.position.x + j * CELL_GAP;
                Vector3 gridCellPosition = new Vector3(horizontalPosition, transform.position.y, verticalPosition);

                GridCell cell = Instantiate(gridCell, gridCellPosition, Quaternion.identity, transform);
                GenerateRoadConnectedToCell(cell, i, j);
                //cell.Location = new Vector2Int(i, j);
                //generatedCellDictionary.Add(cell.Location, cell);
                targetGroup.AddMember(cell.transform, 1, 1);
            }
        }
    }

    public void ClearParentObjects()
    {
        if (transform.childCount > 0)
        {
            int count = transform.childCount;

            for (int i = 0; i < count; i++)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        if (roadParent.childCount > 0)
        {
            int count = roadParent.childCount;

            for (int i = 0; i < count; i++)
            {
                DestroyImmediate(roadParent.GetChild(0).gameObject);
            }
        }
    }

    public void GenerateRoadConnectedToCell(GridCell cell, int cellVertical, int cellHorizontal)
    {
        Vector3 cellPosition = cell.transform.position;
        // if its left top corner
        if(cellVertical == 0 && cellHorizontal == 0)
        {
            Vector3 roadPosition = new Vector3(cellPosition.x - CELL_GAP, cellPosition.y, cellPosition.z + CELL_GAP);
            Instantiate(leftTopCorner, roadPosition, Quaternion.identity, roadParent);
        }
        // if its right top corner
        else if(cellVertical == 0 && cellHorizontal == horizontalGridCount - 1) 
        {
            Vector3 roadPosition = new Vector3(cellPosition.x + CELL_GAP, cellPosition.y, cellPosition.z + CELL_GAP);
            Instantiate(otherCorners, roadPosition, Quaternion.Euler(new Vector3(0, 90f, 0)), roadParent);
        }
        // if its left bottom corner
        else if (cellVertical == verticalGridCount - 1 && cellHorizontal == 0)
        {
            Vector3 roadPosition = new Vector3(cellPosition.x - CELL_GAP, cellPosition.y, cellPosition.z - CELL_GAP);
            Instantiate(otherCorners, roadPosition, Quaternion.Euler(new Vector3(0, -90f, 0)), roadParent);
        }
        // if its right bottom corner
        else if (cellVertical == verticalGridCount - 1 && cellHorizontal == horizontalGridCount - 1)
        {
            Vector3 roadPosition = new Vector3(cellPosition.x + CELL_GAP, cellPosition.y, cellPosition.z - CELL_GAP);
            Instantiate(otherCorners, roadPosition, Quaternion.Euler(new Vector3(0, 180f, 0)), roadParent);
        }
        // if its left side
        else if (cellVertical != 0 && cellVertical != verticalGridCount - 1 && cellHorizontal == 0)
        {
            Vector3 roadPosition = new Vector3(cellPosition.x - CELL_GAP, cellPosition.y, cellPosition.z);
            Instantiate(straightRoad, roadPosition, Quaternion.identity, roadParent);
        }
        // if its right side
        else if (cellVertical != 0 && cellVertical != verticalGridCount - 1 && cellHorizontal == horizontalGridCount - 1)
        {
            Vector3 roadPosition = new Vector3(cellPosition.x + CELL_GAP, cellPosition.y, cellPosition.z);
            Instantiate(straightRoad, roadPosition, Quaternion.identity, roadParent);
        }
        // if its top
        else if (cellVertical == 0 && cellHorizontal != 0 && cellHorizontal != horizontalGridCount - 1)
        {
            Vector3 roadPosition = new Vector3(cellPosition.x, cellPosition.y, cellPosition.z + CELL_GAP);
            Instantiate(straightRoad, roadPosition, Quaternion.Euler(new Vector3(0, 90f, 0)), roadParent);
        }
        // if its bottom
        else if (cellVertical == verticalGridCount - 1 && cellHorizontal != 0 && cellHorizontal != horizontalGridCount - 1)
        {
            Vector3 roadPosition = new Vector3(cellPosition.x, cellPosition.y, cellPosition.z - CELL_GAP);
            Instantiate(straightRoad, roadPosition, Quaternion.Euler(new Vector3(0, 90f, 0)), roadParent);
        }
    }
}
