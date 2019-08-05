using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public Vector3 smoothVector;
    public float speed = 5.0f;
    private Vector3 startPosition;
    private Vector3 newPosition;

    private void Start()
    {
        startPosition = transform.position;
        newPosition = transform.position;
    }

    private void FixedUpdate()
    {
        newPosition = startPosition + (smoothVector * Mathf.Sin(Time.time * speed));
        transform.position = newPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Transform playerT = collision.gameObject.transform;
            playerT.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Transform playerT = collision.gameObject.transform;
            playerT.SetParent(null);
        }
    }
}
