using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float timeBetweenWaypoints;
    private Rigidbody Rigidbodyrb;
    private float curretTime, speed;
    private int currentPointInd;

    // Start is called before the first frame update
    void Start()
    {
        currentPointInd= 0;
        Rigidbodyrb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        speed = Vector3.Distance(wayPoints[currentPointInd].position, wayPoints[(currentPointInd + 1) % wayPoints.Length].position) / timeBetweenWaypoints;
        curretTime += Time.deltaTime;
        if (curretTime >= timeBetweenWaypoints)
        {
            currentPointInd = (currentPointInd + 1) % wayPoints.Length;
            curretTime= 0;
        }
        Rigidbodyrb.velocity=(wayPoints[(currentPointInd + 1) % wayPoints.Length].position- wayPoints[currentPointInd].position).normalized*speed;
        
    }
}
