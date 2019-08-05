using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDialogInput : MonoBehaviour {

    private DialogInput dialogInput;

    private void Awake()
    {
        dialogInput = GetComponent<DialogInput>();
    }

    private void FixedUpdate()
    {
        if (FlowchartManager.instance.isTalking && Input.GetKeyDown(KeyCode.Z))
            dialogInput.SetDialogClickedFlag();
    }
}
