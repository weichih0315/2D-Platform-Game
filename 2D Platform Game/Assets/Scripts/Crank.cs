using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Crank : MonoBehaviour {

    public GameObject effectImage;
    public Animator animator;
    public Flowchart flowchart;
    public UnityEvent OpenCrank = new UnityEvent();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            effectImage.SetActive(true);
            if (!FlowchartManager.instance.isTalking && Input.GetKeyDown(KeyCode.Z))
            {
                flowchart.ExecuteBlock("Crank");
                animator.SetTrigger("Close");
                OpenCrank.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            effectImage.SetActive(false);
            FlowchartManager.instance.isTalking = false;
            flowchart.StopBlock("Crank");
        }
    }
}
