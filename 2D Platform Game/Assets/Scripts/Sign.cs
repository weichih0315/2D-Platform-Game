using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour {

    public GameObject effectImage;
    public Flowchart flowchart;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            effectImage.SetActive(true);
            if (!FlowchartManager.instance.isTalking && Input.GetKeyDown(KeyCode.Z))
            {
                flowchart.ExecuteBlock("Sign");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            effectImage.SetActive(false);
            FlowchartManager.instance.isTalking = false;
            flowchart.StopBlock("Sign");
        }  
    }
}
