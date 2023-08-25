using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegScript : MonoBehaviour
{
    public GameObject foot;
    public float minStepDis;
    public float ThighHeight;
    public Transform desStepPoint;
    public GameObject IK;
    public LayerMask ground;
    public float groundForce;

    Vector3 groundUP;
    public bool canStep, shouldStep;
    

    FootStep FS;
    FootIKSolver fIK;
    Vector3 targetPos, currentPos;

    void Start()
    {
        groundUP = Vector3.up;
        ThighHeight = Mathf.Abs(IK.transform.position.y - desStepPoint.position.y)*1.1f;
        FS = IK.GetComponent<FootStep>();
        currentPos = GroundOffset(transform, ThighHeight);
        fIK=foot.GetComponent<FootIKSolver>();  
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        GetPositions();
        if (!fIK.CanPlaceFoot())
        {
            return;
        }

        if (Vector3.Distance(currentPos, targetPos) > minStepDis && canStep)
        {
            shouldStep = true;
            FS.MoveLeg(currentPos, targetPos);
            currentPos = targetPos;
        }
        else
        {
            shouldStep = false;
            if (FS.Stepping == false)
            {
                IK.transform.position = currentPos;
                foot.GetComponent<Rigidbody>().AddForce(Vector3.down * groundForce);
            }
        }
        IK.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(desStepPoint.forward,groundUP),groundUP);
    }

    
    void GetPositions()
    {
        targetPos = GroundOffset(desStepPoint, ThighHeight);
    }

    public Vector3 GroundOffset(Transform t,float height)
    {
        RaycastHit h;
        if (Physics.Raycast(t.position,Vector3.down,out h, height, ground))
        {
            groundUP = h.normal;
            return h.point;
        }
        groundUP = Vector3.up;
        return t.position  + Vector3.down * height;
    }


}
