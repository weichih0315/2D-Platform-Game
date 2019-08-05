using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowchartManager : MonoBehaviour {

    public bool isTalking;

    public static FlowchartManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void SetIsTalking(bool boolean)
    {
        isTalking = boolean;
    }
}
