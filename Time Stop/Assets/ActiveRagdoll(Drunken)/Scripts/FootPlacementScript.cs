using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPlacementScript : MonoBehaviour
{
    public Transform bodyCOMPoint;
    public StepController stc;
    public float maxBalanceDis;
    public LayerMask Ground;
    public GameObject LFoot,RFoot;
    public GameObject LIKTarget, RIKTarget;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        balanceLeg();
    }

    void balanceLeg()
    {
        if (!comStable())
        {
            StepLeg(stc.movLegIndex);
        }
      
    }

    void stickLeg()
    {
        LFoot.GetComponent<FootIKSolver>().Target.GetComponent<FootStep>().stickLeg(LFoot.transform.position);
        RFoot.GetComponent<FootIKSolver>().Target.GetComponent<FootStep>().stickLeg(RFoot.transform.position);
    }
    void StepLeg(int legIndex)
    {
        GameObject leg, otherLeg;
        if (legIndex == 1)
        {
            leg = RFoot;
            otherLeg = LFoot;
        }
        else
        {
            leg = LFoot;
            otherLeg = RFoot;
        }

        Vector3 comPos = new Vector3(bodyCOMPoint.position.x, otherLeg.transform.position.y, bodyCOMPoint.position.z);
        Vector3 desiredPoint = 2 * comPos - otherLeg.transform.position;
        float raycastDistance = LFoot.GetComponent<FootIKSolver>().CompleteLength;
        Vector3 raycastPosition = desiredPoint + Vector3.up * raycastDistance*9/10;

        if (Physics.Raycast(raycastPosition,Vector3.down,out RaycastHit hitinfo, raycastDistance, Ground))
        {
            leg.GetComponent<FootIKSolver>().Target.GetComponent<FootStep>().MoveLeg(leg.GetComponent<FootIKSolver>().Target.position, hitinfo.point);
        }
    }

    bool comStable()
    {
        Vector3 footMidpoint = (LFoot.transform.position + RFoot.transform.position) / 2;
        Vector3 comPos = bodyCOMPoint.position;
        float sqrDistance=Mathf.Pow((footMidpoint.x-comPos.x),2)+Mathf.Pow((footMidpoint.z-comPos.z),2);
        return sqrDistance <= maxBalanceDis*maxBalanceDis;
    }
}
