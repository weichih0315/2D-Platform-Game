using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyAI1 : Enemy {
    
    public Transform pathHolder;

    public Vector2 damageKnockbackForce;
    public GameObject deadEffect;

    [Header("TopHurt")]
    public float knockbackForce = 5;
    public float scannerRadius = 10;
    [Range(0.0f, 360.0f)]
    public float scannerAngle = 90;

    private Vector2[] wayPoints;
    private int targetWayPointIndex;

    protected override void Start()
    {
        base.Start();
        wayPoints = new Vector2[pathHolder.childCount];

        float mixDistance = Vector2.Distance(transform.position, wayPoints[0]);
        for (int i = 0; i < wayPoints.Length; i++)
        {
            wayPoints[i] = pathHolder.GetChild(i).position;

            if (i > 0 && Vector2.Distance(transform.position, wayPoints[i]) < mixDistance)
            {
                mixDistance = Vector2.Distance(transform.position, wayPoints[i]);
                targetWayPointIndex = i;
            }
        }

        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        float percent = 0;
        Vector2 startPoint = transform.position;
        Vector2 endPoint = wayPoints[targetWayPointIndex];
        float distance = (startPoint - endPoint).magnitude;

        transform.localEulerAngles = (startPoint.x > endPoint.x) ? new Vector2(0, 0) : new Vector2(0, 180);

        do
        {
            percent += speed * Time.deltaTime / distance;
            percent = Mathf.Clamp01(percent);
            transform.position = Vector2.Lerp(startPoint, endPoint, percent);
            yield return null;
        } while (percent < 1);

        targetWayPointIndex = (targetWayPointIndex + 1) % wayPoints.Length;
        StartCoroutine(Move());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Vector2 dirToForward = (transform.up).normalized;
            Vector2 dirToTarget = (collision.transform.position - transform.position).normalized;

            if (Vector2.Angle(dirToForward, dirToTarget) < scannerAngle / 2)
            {

                Player player = collision.gameObject.GetComponent<Player>();
                Rigidbody2D rigidbody2D = player.GetComponent<Rigidbody2D>();
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.AddForce(Vector2.up * knockbackForce);
                Die();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Vector2 dirToForward = (transform.up).normalized;
            Vector2 dirToTarget = (collision.transform.position - transform.position).normalized;

            if (Vector2.Angle(dirToForward, dirToTarget) > scannerAngle / 2)
            {
                Player player = collision.gameObject.GetComponent<Player>();
                if (player.isInvincible)
                    return;

                player.TakeDamage(1);

                Rigidbody2D rigidbody2D = player.GetComponent<Rigidbody2D>();
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.AddForce(new Vector2(dirToTarget.x > 0? 1 : -1, 1) * damageKnockbackForce);
            }
        }
    }

    public override void Die()
    {
        Destroy(Instantiate(deadEffect, transform.position, Quaternion.identity), 1f);
        base.Die();
    }

    void OnDrawGizmos()
    {
        if (pathHolder == null)
            return;

        Vector2 startPosition = pathHolder.GetChild(0).position;
        Vector2 previousPosition = startPosition;

        foreach (Transform wayPoint in pathHolder)
        {
            Gizmos.DrawLine(previousPosition, wayPoint.position);
            previousPosition = wayPoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);


        //Test 用  產生遊戲檔 無法編譯
        //Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.2f);
        //Vector2 rotatedForward = Quaternion.Euler(0, 0, -scannerAngle * 0.5f) * transform.up;
        //Handles.DrawSolidArc(transform.position, Vector3.forward, rotatedForward, scannerAngle, scannerRadius);

        Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
