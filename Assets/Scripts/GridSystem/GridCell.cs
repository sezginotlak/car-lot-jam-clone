using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    // VARIABLES
    public bool isBlocked;
    private Vector2Int location;
    GameController gameController;
    public List<GridCell> neighbourCells = new List<GridCell>();
    public GridCell connectedTo;
    public MeshRenderer cellRenderer;
    public Color startColor, redColor, greenColor;

    // PROPS
    public bool IsBlocked { get => isBlocked; set => isBlocked = value; }
    public Vector2Int Location { get => location; set => location = value; }

    private void Start()
    {
        gameController = GameController.Instance;
        startColor = cellRenderer.material.color;
    }

    public void SetColor(Color color)
    {
        Material cellMat = cellRenderer.material;
        cellMat.DOColor(color, 0.5f).OnComplete(() => cellMat.DOColor(startColor, 0.5f));
    }

    private void OnMouseDown()
    {
        if (IsBlocked)
        {
            // feedbackler
            SetColor(redColor);
            gameController.OnCantReach();
        }
        else if(gameController.CurrentDriver != null)
        {
            // feedbackler
            gameController.OnCanReach(this);
        }

    }
}
