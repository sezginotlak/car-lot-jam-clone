using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    [Range(1, 5)]
    [Tooltip("Vertical grid count can be between 1 and 10.")]
    public int horizontalGridCount = 3;

    [Range(1, 10)]
    [Tooltip("Vertical grid count can be between 1 and 5.")]
    public int verticalGridCount = 5;

    public GameObject gridCell;
    public CinemachineTargetGroup targetGroup;

    private const float CELL_GAP = 1.75f;

    public void GenerateGrids(int verticalCount, int horizontalCount)
    {
        targetGroup.m_Targets = new CinemachineTargetGroup.Target[0];

        for(int i = 0; i < verticalCount; i++)
        {
            float verticalPosition = transform.position.z - i * CELL_GAP;
            for(int j = 0; j < horizontalCount; j++)
            {
                float horizontalPosition = transform.position.x + j * CELL_GAP;
                Vector3 gridCellPosition = new Vector3(horizontalPosition, transform.position.y, verticalPosition);

                GameObject cell = Instantiate(gridCell, gridCellPosition, Quaternion.identity, transform);
                targetGroup.AddMember(cell.transform, 1, 1);
            }
        }
    }
}
