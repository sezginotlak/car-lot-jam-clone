using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FindShortestPath : MonoBehaviour
{
    public GenerateGrid generateGrid;

    Queue<GridCell> frontier = new Queue<GridCell>();
    Dictionary<Vector2Int, GridCell> reachedCells = new Dictionary<Vector2Int, GridCell>();
    List<Vector2Int> directionList = new List<Vector2Int>()
        {
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
        };

    GridCell startCell;
    GridCell destinationCell;
    GridCell currentSearchingCell;

    bool isReachedDestination;

    public GridCell StartCell { get => startCell; set => startCell = value; }
    public GridCell DestinationCell { get => destinationCell; set => destinationCell = value; }

    private void Start()
    {
        GameController.Instance.FindShortestPath = this;
    }

    void FindNeighbours(Dictionary<Vector2Int, GridCell> cellDictionary)
    {
        List<GridCell> neighbourList = new List<GridCell>();
        foreach (Vector2Int direction in directionList)
        {
            Vector2Int neighbourCoordinates = currentSearchingCell.Location + direction;
            GridCell neighbourCell = null;

            if (cellDictionary.ContainsKey(neighbourCoordinates))
                neighbourCell = cellDictionary[neighbourCoordinates];

            if (neighbourCell != null && !neighbourList.Contains(neighbourCell))
                neighbourList.Add(neighbourCell);

        }

        foreach(GridCell neighbour in neighbourList)
        {
            if(!reachedCells.ContainsKey(neighbour.Location) && !neighbour.IsBlocked) 
            {
                neighbour.connectedTo = currentSearchingCell;
                reachedCells.Add(neighbour.Location, neighbour);
                frontier.Enqueue(neighbour);
            }
        }
    }

    public void SearchShortestPath()
    {
        bool isRunning = true;
        isReachedDestination = false;
        reachedCells.Clear();
        frontier.Clear();

        frontier.Enqueue(StartCell);
        reachedCells.Add(StartCell.Location, StartCell);

        while(frontier.Count > 0 && isRunning)
        {
            currentSearchingCell = frontier.Dequeue();
            FindNeighbours(generateGrid.generatedCellDictionary);

            if(currentSearchingCell.Location == DestinationCell.Location)
            {
                isReachedDestination = true;
                isRunning = false;
            }
        }
    }

    // SearchShortestPath'ten hemen sonra çaðrýlacak
    public List<GridCell> BuildPath()
    {
        if (!isReachedDestination)
            return null;

        List<GridCell> path = new List<GridCell>();
        GridCell currentCell = DestinationCell;

        path.Add(currentCell);

        while(currentCell.connectedTo != null)
        {
            currentCell = currentCell.connectedTo;
            path.Add(currentCell);
        }

        path.Reverse();

        return path;
    }
}
