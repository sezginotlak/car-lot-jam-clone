using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Object : MonoBehaviour
{
    public List<GridCell> pathToDestination = new List<GridCell>();
    public GridCell currentOccupiedCell;
    public LayerMask layerMaskForStart;
    public int id;

    internal GameController gameController;

    bool isOnEnable;

    public virtual void Start()
    {
        gameController = GameController.Instance;    
    }

    // Oyun baþlangýcýnda hangi hücrelerin dolu olduðunu belirtmek için kullanýlýyor
    private IEnumerator MarkOccupiedCell(Collider other)
    {
        yield return new WaitForSeconds(0.2f);
        currentOccupiedCell = other.transform.GetComponent<GridCell>();
        currentOccupiedCell.IsBlocked = true;
        isOnEnable = true;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (isOnEnable) return;

        if(other.gameObject.layer == 3)
            StartCoroutine(MarkOccupiedCell(other));

    }
}
