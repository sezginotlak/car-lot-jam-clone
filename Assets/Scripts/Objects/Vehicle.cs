using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : Object
{
    public List<Transform> raycastPoints = new List<Transform>();
    public Transform overlapBoxPoint;
    public LayerMask layerMaskForBlocks;
    public LayerMask layerMaskForRoad;
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

    IEnumerator LeaveArea()
    {
        yield return new WaitUntil(() => HasPathToLeave());
        List<RoadCell> path = DecidePath();
        MoveToDestination(path);
    }

    List<RoadCell> DecidePath()
    {
        RaycastHit hit, hit1;
        RoadCell roadCell = null, roadCell1 = null;
        if (Physics.Raycast(overlapBoxPoint.position, transform.forward, out hit, Mathf.Infinity, layerMaskForRoad))
        { 
            if(hit.transform.TryGetComponent(out roadCell))
            {
                roadCell = hit.transform.GetComponent<RoadCell>();
            }
        }

        if (Physics.Raycast(overlapBoxPoint.position, -transform.forward, out hit1, Mathf.Infinity, layerMaskForRoad))
        {
            if (hit1.transform.TryGetComponent(out roadCell1))
            {
                roadCell1 = hit1.transform.GetComponent<RoadCell>();
            }
        }

        return MakePath(FindTheNearestRoadToExit(roadCell, roadCell1), generateGrid.roadCellList);
    }

    RoadCell FindTheNearestRoadToExit(RoadCell cell, RoadCell cell1) 
    {
        RoadCell result = null;
        if(cell == null)
        {
            Debug.Log(cell1.name, cell1.gameObject);
            return cell1;
        } 

        if (cell1 == null)
        {
            Debug.Log(cell.name, cell.gameObject);
            return cell;
        }

        if (Vector3.Distance(transform.position, cell.transform.position) <= Vector3.Distance(transform.position, cell1.transform.position))
            return cell;
        else
            return cell1;
    }

    List<RoadCell> MakePath(RoadCell cell, List<RoadCell> roadCellList)
    {
        int maxRoadCount = generateGrid.roadCellList.Count;
        int cellIndex = generateGrid.roadCellList.IndexOf(cell);
        int midIndex = (maxRoadCount / 2) - 1;
        List<RoadCell> path = new List<RoadCell>();
        if(cellIndex <= midIndex)
        {
            for(int i = cellIndex; i >= 0; i--)
                path.Add(roadCellList[i]);

            path.Add(roadCellList[^1]);
        }
        else
        {
            for (int i = cellIndex; i < maxRoadCount; i++)
                path.Add(roadCellList[i]);
        }

        return path;
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
        StartCoroutine(LeaveArea());
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

    private IEnumerator IEMoveToDestination(List<RoadCell> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 destination;
            if (i > 0)
            {
                Vector3 fwRotation = path[i].transform.position - transform.position;
                //transform.DORotate(fwRotation, 0.1f);
                transform.DOLookAt(path[i].transform.position, 0.1f);

                destination = path[i].transform.position;
                transform.DOMove(destination, 0.15f).SetEase(Ease.Linear);
            }
            else
            {
                destination = path[i].transform.position;
                transform.DOMove(destination, 0.3f).SetEase(Ease.Linear);
            }
            yield return new WaitUntil(() => IsReachedDestination(destination));
        }

        transform.DOMove(transform.position + Vector3.forward * 30f, 3f).SetEase(Ease.Linear);
    }

    public void MoveToDestination(List<RoadCell> path)
    {
        StartCoroutine(IEMoveToDestination(path));
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
