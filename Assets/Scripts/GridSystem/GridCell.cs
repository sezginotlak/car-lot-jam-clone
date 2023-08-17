using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private bool isBlocked;

    public bool IsBlocked { get => isBlocked; set => isBlocked = value; }
}
