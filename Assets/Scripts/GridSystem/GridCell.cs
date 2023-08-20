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
            gameController.OnCantReach();
        }
        else if(gameController.CurrentDriver != null)
        {
            // feedbackler
            gameController.OnCanReach(this);
        }

    }
}
