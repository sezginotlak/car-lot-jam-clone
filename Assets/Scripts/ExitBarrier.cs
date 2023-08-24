using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBarrier : MonoBehaviour
{
    public Transform barrier;

    private void Start()
    {
        GameController.Instance.ExitBarrier = this;    
    }

    public void BarrierAct()
    {
        StartCoroutine(IEBarrierAct());
    }

    IEnumerator IEBarrierAct()
    {
        barrier.DOLocalRotate(new Vector3(0, 0, 60f), 0.2f);
        yield return new WaitForSeconds(1f);
        barrier.DOLocalRotate(Vector3.zero, 0.2f);
    }
}
