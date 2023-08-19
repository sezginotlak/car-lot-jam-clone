using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Driver : Object
{
    private void OnMouseDown()
    {
        GameController.Instance.CurrentDriver = this;
        GameController.Instance.FindShortestPath.StartCell = currentOccupiedCell;
    }

    private IEnumerator IEMoveToDestination()
    {
        for(int i = 1; i < pathToDestination.Count; i++)
        {
            Vector3 destination = pathToDestination[i].transform.position;
            transform.DOMove(destination, 0.5f).SetEase(Ease.Linear);
            yield return new WaitUntil(() => IsReachedDestination(destination));
            pathToDestination[i].connectedTo = null;
        }

        pathToDestination.Clear();
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
