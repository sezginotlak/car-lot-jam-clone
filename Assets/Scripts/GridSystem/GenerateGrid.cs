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
    public List<RoadCell> roadCellList = new List<RoadCell>();
    public CinemachineTargetGroup targetGroup;

    [Header("Road Prefabs")]
    [SerializeField] GameObject exitCorner;
    [SerializeField] GameObject exitRoad;
    [SerializeField] GameObject corner;
    [SerializeField] GameObject straightRoad;
    [SerializeField] Transform roadParent;

    private const float CELL_GAP = 1.75f;

    int maxRoadID = 0;

    private void Start()
    {
        AdjustGridCells();
        AdjustRoadCells();
    }

    private void AdjustGridCells()
    {
        int childCount = transform.childCount;
        int j = -1;

        for (int i = 0; i < childCount; i++)
        {
            if (i % horizontalGridCount == 0)
                j++;

            GridCell cell = transform.GetChild(i).GetComponent<GridCell>();
            cell.Location = new Vector2Int(j, i % horizontalGridCount);

            generatedCellDictionary.Add(cell.Location, cell);
        }

        CameraManager.Instance.ChangeCamera(verticalGridCount);
    }

    public void GenerateGrids(int verticalCount, int horizontalCount)
    {
        targetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
        generatedCellDictionary.Clear();
        maxRoadID = (verticalCount * horizontalCount) + 3; // köþeler dahil yol sayýsý

        ClearParentObjects();
        Dictionary<Vector2Int, GridCell> gridCellList = new Dictionary<Vector2Int, GridCell>();
        for (int i = 0; i < verticalCount; i++)
        {
            float verticalPosition = transform.position.z - i * CELL_GAP;
            for (int j = 0; j < horizontalCount; j++)
            {
                float horizontalPosition = transform.position.x + j * CELL_GAP;
                Vector3 gridCellPosition = new Vector3(horizontalPosition, transform.position.y, verticalPosition);

                GridCell cell = Instantiate(gridCell, gridCellPosition, Quaternion.identity, transform);
                //GenerateRoadConnectedToCell(cell, i, j);

                // oluþturulacak yollar için kullanýlacak liste
                cell.Location = new Vector2Int(i, j);
                gridCellList.Add(cell.Location, cell);

                //generatedCellDictionary.Add(cell.Location, cell);
                targetGroup.AddMember(cell.transform, 1, 1.75f);
            }
        }

        GenerateRoads(gridCellList);
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

    public void GenerateRoads(Dictionary<Vector2Int, GridCell> gridCellList)
    {
        //top
        for(int i = 0; i < horizontalGridCount; i++)
        {
            Vector2Int cellLocation = new Vector2Int(0, i);
            GridCell cell = gridCellList[cellLocation];
            Vector3 cellPosition = cell.transform.position;

            Vector3 roadPosition = new Vector3(cellPosition.x, cellPosition.y, cellPosition.z + CELL_GAP);
            Instantiate(straightRoad, roadPosition, Quaternion.Euler(new Vector3(0, 270f, 0)), roadParent);

            if (i == horizontalGridCount - 1)
            {
                Vector3 cornerPosition = roadPosition;
                cornerPosition.x += CELL_GAP;
                Instantiate(corner, cornerPosition, Quaternion.Euler(new Vector3(0, 90f, 0)), roadParent);
            }
        }

        //right
        for (int i = 0; i < verticalGridCount; i++)
        {
            Vector2Int cellLocation = new Vector2Int(i, horizontalGridCount - 1);
            GridCell cell = gridCellList[cellLocation];
            Vector3 cellPosition = cell.transform.position;

            Vector3 roadPosition = new Vector3(cellPosition.x + CELL_GAP, cellPosition.y, cellPosition.z);
            Instantiate(straightRoad, roadPosition, Quaternion.identity, roadParent);

            if (i == verticalGridCount - 1)
            {
                Vector3 cornerPosition = roadPosition;
                cornerPosition.z -= CELL_GAP;
                Instantiate(corner, cornerPosition, Quaternion.Euler(new Vector3(0, -180, 0)), roadParent);
            }
        }

        //bottom
        for (int i = horizontalGridCount - 1; i >= 0; i--)
        {
            Vector2Int cellLocation = new Vector2Int(verticalGridCount - 1, i);
            GridCell cell = gridCellList[cellLocation];
            Vector3 cellPosition = cell.transform.position;

            Vector3 roadPosition = new Vector3(cellPosition.x, cellPosition.y, cellPosition.z - CELL_GAP);
            Instantiate(straightRoad, roadPosition, Quaternion.Euler(new Vector3(0, 270f, 0)), roadParent);

            if (i == 0)
            {
                Vector3 cornerPosition = roadPosition;
                cornerPosition.x -= CELL_GAP;
                Instantiate(corner, cornerPosition, Quaternion.Euler(new Vector3(0, -90f, 0)), roadParent);
            }
        }

        //left
        for (int i = verticalGridCount - 1; i >= 0; i--)
        {
            Vector2Int cellLocation = new Vector2Int(i, 0);
            GridCell cell = gridCellList[cellLocation];
            Vector3 cellPosition = cell.transform.position;

            Vector3 roadPosition = new Vector3(cellPosition.x - CELL_GAP, cellPosition.y, cellPosition.z);
            Instantiate(straightRoad, roadPosition, Quaternion.identity, roadParent);

            if (i == 0)
            {
                Vector3 cornerPosition = roadPosition;
                cornerPosition.z += CELL_GAP;
                Instantiate(exitCorner, cornerPosition, Quaternion.Euler(new Vector3(0, 0, 0)), roadParent);
                Instantiate(exitRoad, cornerPosition, Quaternion.Euler(new Vector3(0, 0, 0)), roadParent);
            }
        }
    }

    void AdjustRoadCells()
    {
        roadCellList = roadParent.GetComponentsInChildren<RoadCell>().ToList();
        int id = 0;
        foreach (RoadCell cell in roadCellList)
        {
            cell.roadId = id;
            id++;
        }
    }
}
