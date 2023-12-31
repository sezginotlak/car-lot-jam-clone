using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    bool isOnEnable;

    // Oyun başlangıcında hangi hücrelerin dolu olduğunu belirtmek için kullanılıyor
    private IEnumerator MarkOccupiedCell(Collider other)
    {
        yield return new WaitForSeconds(0.2f);
        other.transform.GetComponent<GridCell>().IsBlocked = true;
        isOnEnable = true;
    }

    public virtual void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 3)
        {
            if (isOnEnable) return;

            StartCoroutine(MarkOccupiedCell(other));
        }

    }
}
