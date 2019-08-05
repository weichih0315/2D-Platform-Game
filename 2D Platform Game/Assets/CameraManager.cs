using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public List<CameraFollow> cameraFollows;

    public static CameraManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void InitialFollowTarget(Transform target)
    {
        foreach (CameraFollow cameraFollow in cameraFollows)
        {
            cameraFollow.gameObject.SetActive(true);
            cameraFollow.SetFollowTarget(target);
            cameraFollow.gameObject.SetActive(false);
        }

        cameraFollows[0].gameObject.SetActive(true);
    }
}
