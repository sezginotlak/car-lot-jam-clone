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

    [SerializeField] GridCell startCell;
    [SerializeField] GridCell destinationCell;
    GridCell currentSearchingCell;

    bool isReachedDestination;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SearchShortestPath();
            BuildPath();
        }
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

    void SearchShortestPath()
    {
        bool isRunning = true;
        isReachedDestination = false;
        reachedCells.Clear();

        frontier.Enqueue(startCell);
        reachedCells.Add(startCell.Location, startCell);

        while(frontier.Count > 0 && isRunning)
        {
            currentSearchingCell = frontier.Dequeue();
            FindNeighbours(generateGrid.generatedCellDictionary);

            if(currentSearchingCell.Location == destinationCell.Location)
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
        {
            Debug.Log("CAN'T REACH THE END");
            return null;
        }

        List<GridCell> path = new List<GridCell>();
        GridCell currentCell = destinationCell;

        path.Add(currentCell);

        while(currentCell.connectedTo != null)
        {
            currentCell = currentCell.connectedTo;
            path.Add(currentCell);
        }

        path.Reverse();

        foreach (GridCell neighbour in path)
        {
            Debug.Log("Coordinates: " + neighbour.Location, neighbour.gameObject);
        }

        return path;
    }
}
