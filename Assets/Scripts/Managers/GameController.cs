using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    
    Driver currentDriver;
    FindShortestPath findShortestPath;
    ExitBarrier exitBarrier;

    public List<Vehicle> vehicleList = new List<Vehicle>();

    public Driver CurrentDriver { get => currentDriver; set => currentDriver = value; }
    public FindShortestPath FindShortestPath { get => findShortestPath; set => findShortestPath = value; }
    public ExitBarrier ExitBarrier { get => exitBarrier; set => exitBarrier = value; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OnCanReach(GridCell cell)
    {
        if (CurrentDriver.vehicle != null)
            CurrentDriver.OpenHappyEmoji();

        CurrentDriver.outline.enabled = false;
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
        if(currentDriver != null)
            CurrentDriver.outline.enabled = false;

        CurrentDriver.OpenAngryEmoji();
        FindShortestPath.StartCell = null;
        FindShortestPath.DestinationCell = null;
        CurrentDriver = null;
    }
}
