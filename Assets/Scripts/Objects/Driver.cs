using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Driver : Object
{
    const string WAVE_HAND_PARAMETER = "wave";
    const string ENTER_CAR_PARAMETER = "enter";
    const string MOVE_SPEED_PARAMETER = "speed";
    const string ENTERING_IDENTIFIER_PARAMETER = "entering";

    public Animator animator;
    public Vehicle vehicle;
    bool isRight;

    private void OnMouseDown()
    {
        if(gameController.CurrentDriver != null)
            gameController.CurrentDriver.outline.enabled = false;

        gameController.CurrentDriver = this;
        findShortestPath.StartCell = currentOccupiedCell;
        outline.enabled = true;
        animator.SetTrigger(WAVE_HAND_PARAMETER);
    }

    private IEnumerator IEMoveToDestination()
    {
        if (pathToDestination == null)
        {
            findShortestPath.DestinationCell.SetColor(findShortestPath.DestinationCell.greenColor);
            GameController.Instance.OnCantReach();
        }
        else
        {
            animator.SetFloat(MOVE_SPEED_PARAMETER, 1f);
            for (int i = 1; i < pathToDestination.Count; i++)
            {
                Vector3 destination = pathToDestination[i].transform.position;
                transform.DOLookAt(destination, 0.1f);
                transform.DOMove(destination, 0.15f).SetEase(Ease.Linear);
                yield return new WaitUntil(() => IsReachedDestination(destination));
                pathToDestination[i].connectedTo = null;
            }
            int enteringIdentifier = 0;
            if(isRight)
                enteringIdentifier = 1;
            animator.SetFloat(MOVE_SPEED_PARAMETER, 0);
            pathToDestination.Clear();

            GetInCar(enteringIdentifier);
        }
    }

    void GetInCar(int enteringIdentifier)
    {
        if(vehicle != null)
            StartCoroutine(IEGetInCar(enteringIdentifier));
    }

    IEnumerator IEGetInCar(int enteringIdentifier)
    {
        transform.parent = vehicle.seatPoint;
        Vector3 newPos = Vector3.zero;
        Vector3 lookPos = new Vector3(vehicle.seatPoint.position.x, transform.position.y, vehicle.seatPoint.position.z);
        if (enteringIdentifier == 0)
        {
            newPos = Vector3.zero - Vector3.right * 1.3f;
            transform.DOLookAt(lookPos, 0.1f);
        }
        else
        {
            newPos = Vector3.zero + Vector3.right * 1.3f;
            transform.DOLookAt(lookPos, 0.1f);
        }

        animator.SetFloat(MOVE_SPEED_PARAMETER, 1);
        transform.DOLocalMove(newPos, 0.15f).OnComplete(() =>
        {
            animator.SetFloat(MOVE_SPEED_PARAMETER, 0);
            transform.DOLookAt(lookPos, 0.1f);
        });
        yield return new WaitForSeconds(0.5f);
        

        animator.SetTrigger(ENTER_CAR_PARAMETER);
        animator.SetFloat(ENTERING_IDENTIFIER_PARAMETER, enteringIdentifier);
        vehicle.DoorAnim(enteringIdentifier);
        yield return new WaitForSeconds(0.5f);
        transform.DOLocalMove(Vector3.zero, 2f);
        transform.DORotate(vehicle.seatPoint.eulerAngles, 2f);
        transform.DOScale(Vector3.one * 0.2f, 2f);
        yield return new WaitForSeconds(1.5f);
        vehicle.StartCar();
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

        if (other.CompareTag("Left"))
        {
            isRight = false;
        }
        else if (other.CompareTag("Right"))
        {
            isRight = true;
        }
    }
}
