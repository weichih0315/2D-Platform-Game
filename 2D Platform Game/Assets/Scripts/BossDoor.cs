using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossDoor : MonoBehaviour {

    public string name;
    public GameObject door;
    public Vector3 closeMovement;
    public float speed;

    public Flowchart flowchart;

    private bool isClose;

    public UnityEvent OnClose = new UnityEvent();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isClose && collision.tag == "Player")
        {
            StartCoroutine(CloseDoor());
        }
    }

    public void StartFlowchat()
    {
        flowchart.ExecuteBlock("CloseDoor");
    }

    IEnumerator CloseDoor()
    {
        FlowchartManager.instance.isTalking = true;
        isClose = true;
        float percent = 0;
        Vector3 startPoint = door.transform.position;
        Vector3 endPoint = door.transform.position + closeMovement;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            door.transform.position = Vector3.Lerp(startPoint, endPoint, percent);
            yield return null;
        }

        OnClose.Invoke();
    }
}
