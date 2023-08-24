using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vehicle : Object
{
    public List<Transform> raycastPoints = new List<Transform>();
    public Transform overlapBoxPoint;
    public Transform seatPoint;
    public Transform shakeTransform, leftDoor, rightDoor;
    public LayerMask layerMaskForBlocks;
    public LayerMask layerMaskForRoad;
    public ParticleSystem smokeParticle, smokeTrailParticle;
    public Color greenColor, redColor;

    bool isPlayerIn;
    bool isFront;
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
        Invoke(nameof(AddToVehicleList), 0.25f);

    }

    void AddToVehicleList()
    {
        gameController.vehicleList.Add(this);
    }

    void RemoveFromVehicleList()
    {
        gameController.vehicleList.Remove(this);

        if (!gameController.vehicleList.Any())
            UIManager.Instance.OpenWinPanel();
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

    public void LeaveArea()
    {
        StartCoroutine(IELeaveArea());
    }

    public void StartCar()
    {
        StartCoroutine(IEStartCar());
        LeaveArea();
    }

    IEnumerator IEStartCar()
    {
        isPlayerIn = false;
        smokeParticle.Play();
        shakeTransform.DOLocalRotate(new Vector3(0, 0, 4f), 0.15f).OnComplete(() =>
        {
            shakeTransform.DOLocalRotate(new Vector3(0, 0, -4f), 0.3f).OnComplete(() => shakeTransform.DOLocalRotate(Vector3.zero, 0.15f));
        });
        yield return new WaitForSeconds(0.6f);
        isPlayerIn = true;
    }

    IEnumerator IELeaveArea()
    {
        yield return new WaitUntil(() => isPlayerIn);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(shakeTransform.DOLocalRotate(new Vector3(2f, 0, 0), 0.2f)).Append(shakeTransform.DOLocalRotate(new Vector3(-2f, 0, 0), 0.2f));
        sequence.SetLoops(-1);
        sequence.Play();
        yield return new WaitUntil(() => HasPathToLeave());
        smokeParticle.Stop();
        smokeTrailParticle.Play();
        shakeTransform.DOKill();
        List<RoadCell> path = DecidePath();
        MoveToDestination(path);
    }

    public void DoorAnim(int isLeft)
    {
        StartCoroutine(IEDoorAnim(isLeft));
    }

    IEnumerator IEDoorAnim(int isLeft)
    {
        Transform door = null;
        if (isLeft == 0)
        {
            door = leftDoor;
            door.DOLocalRotate(new Vector3(0, 75f, 0), 0.3f);
        }
        else
        {
            door = rightDoor;
            door.DOLocalRotate(new Vector3(0, -75f, 0), 0.3f);
        }
        yield return new WaitForSeconds(1.5f);
        door.DOLocalRotate(Vector3.zero, 0.3f);
    }

    public void SetOutlineWidth() 
    {
        DOTween.To(() => outline.OutlineWidth, x => outline.OutlineWidth = x, outline.OutlineWidth + 1, 1f).OnComplete(() =>
        {
            outline.OutlineWidth--;
            outline.enabled = false;
        });
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
            Debug.Log(hit1.transform.name, hit.transform.gameObject);
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
            isFront = false;
            return cell1;
        } 

        if (cell1 == null)
        {
            Debug.Log(cell.name, cell.gameObject);
            isFront = true;
            return cell;
        }

        if (Vector3.Distance(transform.position, cell.transform.position) <= Vector3.Distance(transform.position, cell1.transform.position))
        {
            isFront = true;
            return cell;
        }
        else
        {
            isFront = false;
            return cell1;
        }
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
        if (gameController.CurrentDriver == null) return;

        if(gameController.CurrentDriver.id != id)
        {
            gameController.CurrentDriver.vehicle = null;
            gameController.OnCantReach();
            OutlineActs(redColor);
        }
        else if(IsReachable() == false)
        {
            gameController.CurrentDriver.vehicle = null;
            gameController.OnCantReach();
            OutlineActs(redColor);
        }
        else
        {
            gameController.CurrentDriver.vehicle = this;
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
                

                if (path != null && tempPath != null && path.Count >= tempPath.Count)
                {
                    destCell = cell;
                    path = tempPath;
                }
                count++;
            }
            OutlineActs(greenColor);
            gameController.OnCanReach(destCell);
        }
    }

    private void OutlineActs(Color color)
    {
        outline.enabled = true;
        outline.OutlineColor = color;
        SetOutlineWidth();
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
        if(isFront)
        {
            shakeTransform.DOLocalRotate(new Vector3(-3f, 0, 0), 0.2f).OnComplete(() => shakeTransform.DOLocalRotate(Vector3.zero, 0.1f));
        }
        else
        {
            shakeTransform.DOLocalRotate(new Vector3(3f, 0, 0), 0.2f).OnComplete(() => shakeTransform.DOLocalRotate(Vector3.zero, 0.1f));
        }
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 destination;
            if (i > 0)
            {
                Vector3 fwRotation = path[i].transform.position - transform.position;
                transform.DOLookAt(path[i].transform.position, 0.1f);

                destination = path[i].transform.position;
                transform.DOMove(destination, 0.15f).SetEase(Ease.Linear);
            }
            else
            {
                destination = path[i].transform.position;
                transform.DOMove(destination, 0.5f).SetEase(Ease.Linear);
            }
            yield return new WaitUntil(() => IsReachedDestination(destination));
        }

        gameController.ExitBarrier.BarrierAct();
        yield return new WaitForSeconds(0.2f);
        OpenHappyEmoji();
        RemoveFromVehicleList();
        Vector3 newPosition = transform.position + Vector3.forward * 30f;
        transform.DOLookAt(newPosition, 0.1f);
        transform.DOMove(newPosition, 3f).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject));
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

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 3)
        {
            other.GetComponent<GridCell>().IsBlocked = false;
        }
    }
}
