using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalMarker : MonoBehaviour
{
    public Transform botChassis;
    MeshRenderer myMesh;
    void Start()
    {
     myMesh=GetComponent<MeshRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        myMesh.enabled=Vector3.Distance(transform.position, botChassis.position)>0.8f;
    }
}
