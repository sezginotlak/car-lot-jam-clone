using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera perspectiveCamera, ortographicCamera;

    public static CameraManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ChangeCamera(int verticalGridCount)
    {
        if (verticalGridCount > 5)
        {
            perspectiveCamera.Priority = 5;
            ortographicCamera.Priority = 15;
        }
        else
        {
            perspectiveCamera.Priority = 15;
            ortographicCamera.Priority = 5;
        }
    }
}
