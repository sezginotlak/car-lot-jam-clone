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
}
