using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    
    Driver currentDriver;
    FindShortestPath findShortestPath;

    public Driver CurrentDriver { get => currentDriver; set => currentDriver = value; }
    public FindShortestPath FindShortestPath { get => findShortestPath; set => findShortestPath = value; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OnCanReach(GridCell cell)
    {
        CurrentDriver.currentOccupiedCell.IsBlocked = false;
        CurrentDriver.currentOccupiedCell = cell;
        FindShortestPath.DestinationCell = cell;
        FindShortestPath.SearchShortestPath();
        CurrentDriver.pathToDestination = FindShortestPath.BuildPath();
        CurrentDriver.MoveToDestination();
        cell.IsBlocked = true;
    }

    public void OnCantReach()
    {
        FindShortestPath.StartCell = null;
        FindShortestPath.DestinationCell = null;
        CurrentDriver = null;
    }
}
