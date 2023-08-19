using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    // VARIABLES
    public bool isBlocked;
    private Vector2Int location;
    GameController gameController;
    public List<GridCell> neighbourCells = new List<GridCell>();
    public GridCell connectedTo;

    // PROPS
    public bool IsBlocked { get => isBlocked; set => isBlocked = value; }
    public Vector2Int Location { get => location; set => location = value; }

    private void Start()
    {
        gameController = GameController.Instance;
    }

    private void OnMouseDown()
    {
        if (IsBlocked)
        {
            // feedbackler
            gameController.FindShortestPath.StartCell = null;
            gameController.FindShortestPath.DestinationCell = null;
            gameController.CurrentDriver = null;
        }
        else
        {
            gameController.CurrentDriver.currentOccupiedCell.IsBlocked = false;
            gameController.CurrentDriver.currentOccupiedCell = this;
            gameController.FindShortestPath.DestinationCell = this;
            gameController.FindShortestPath.SearchShortestPath();
            gameController.CurrentDriver.pathToDestination = gameController.FindShortestPath.BuildPath();
            gameController.CurrentDriver.MoveToDestination();
            IsBlocked = true;
        }
    }
}
