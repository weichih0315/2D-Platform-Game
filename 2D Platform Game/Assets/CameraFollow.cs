using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    
    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetFollowTarget(Transform target)
    {
        cinemachineVirtualCamera.Follow = target;
    }
}
