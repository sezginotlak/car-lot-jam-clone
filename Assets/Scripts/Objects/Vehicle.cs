using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : Object
{
    public List<Transform> raycastPoints = new List<Transform>();
    public Transform overlapBoxPoint;
    public LayerMask layerMaskForBlocks;
    List<GridCell> availableCells = new List<GridCell>(); //arabaya binilebilecek hücreleri belirlemek için

    public override void Start()
    {
        base.Start();
        
        foreach(Transform point in raycastPoints)
        {
            GridCell cell = StartRaycast(point);

            if(cell != null)
                availableCells.Add(cell);
        }
    }

    GridCell StartRaycast(Transform point)
    {
        RaycastHit hit;
        if(Physics.Raycast(point.position, -Vector3.up, out hit, Mathf.Infinity, layerMaskForStart))
        {
            return hit.transform.GetComponent<GridCell>();
        }

        return null;
    }

    IEnumerator WaitUntilHavePath()
    {
        yield return new WaitUntil(() => HasPathToLeave());
    }

    bool HasPathToLeave()
    {
        RaycastHit hit, hit1;
        if (Physics.Raycast(overlapBoxPoint.position, transform.forward, out hit, Mathf.Infinity, layerMaskForBlocks))
            { Debug.Log("False"); }
        else
        {
            Debug.Log("True");
            return true;
        }

        if (Physics.Raycast(overlapBoxPoint.position, -transform.forward, out hit1, Mathf.Infinity, layerMaskForBlocks))
            { Debug.Log("False"); }
        else
        {
            Debug.Log("True");
            return true;
        }

        return false;
    }

    private void OnMouseDown()
    {
        StartCoroutine(WaitUntilHavePath());
        if (gameController.CurrentDriver == null) return;

        if(gameController.CurrentDriver.id != id)
        {
            // feedback
            gameController.OnCantReach();
        }
        else if(IsReachable() == false)
        {
            // feedback
            gameController.OnCantReach();
        }
        else
        {
            // feedback
            List<GridCell> path = new List<GridCell>();
            GridCell destCell = availableCells[0];
            int count = 0;
            foreach(GridCell cell in availableCells)
            {
                if(cell.IsBlocked) continue;

                gameController.FindShortestPath.DestinationCell = cell;
                gameController.FindShortestPath.SearchShortestPath();
                List<GridCell> tempPath = new List<GridCell>();
                tempPath = gameController.FindShortestPath.BuildPath();
                if(count == 0)
                    path = tempPath;

                if (path.Count >= tempPath.Count)
                {
                    destCell = cell;
                    path = tempPath;
                }
                count++;
            }

            gameController.OnCanReach(destCell);
        }
    }

    bool IsReachable()
    {
        bool result = false;

        foreach(GridCell cell in availableCells)
        {
            if (!cell.IsBlocked)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    private IEnumerator IEMoveToDestination()
    {
        yield return new WaitForEndOfFrame();
    }

    public void MoveToDestination()
    {
        StartCoroutine(IEMoveToDestination());
    }

    private bool IsReachedDestination(Vector3 destination)
    {
        return Vector3.Distance(transform.position, destination) < 0.2f;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
