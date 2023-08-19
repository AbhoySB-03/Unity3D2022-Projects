using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralFootPlacement : MonoBehaviour
{
    [SerializeField] private Transform lRaycastPoint, rRaycastPoint;
    [SerializeField] private Transform rootRaycaster;
    [SerializeField] private float raycastHeight=2f;
    [SerializeField] private float stepFactor=1f;
    [SerializeField] private PhysicsIK lFootIK, rFootIK;
    [SerializeField] private float stepDistance=0.4f;
    [SerializeField] private LayerMask ground;

    private Vector3 curPosL, curPosR;
    private int _stepIndex=0;
    private Rigidbody _body;
    // Start is called before the first frame update
    void Start()
    {
        curPosL = lFootIK.transform.position;
        curPosR = rFootIK.transform.position;
        _body= GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        PositionRaycaster();
        curPosL = PerformPlacement(lFootIK, lRaycastPoint, 0, curPosL,rFootIK);
        curPosR = PerformPlacement(rFootIK, rRaycastPoint, 1, curPosR, lFootIK);        
    }

    void PositionRaycaster()
    {
        rootRaycaster.position=transform.position+_body.velocity*stepFactor;
        var _forward=transform.forward;
        _forward.y = 0;
        rootRaycaster.rotation = Quaternion.LookRotation(_forward, Vector3.up);
    }
    Vector3 PerformPlacement(PhysicsIK foot, Transform raycastPoint, int myStepIndex, Vector3 currPos, PhysicsIK OtherFoot)
    {

        if (!Physics.Raycast(raycastPoint.position, Vector3.down, out RaycastHit hit, raycastHeight, ground))
        {
            return currPos;
        }

        if ((hit.point - foot.IKGoalPosition).sqrMagnitude > stepDistance * stepDistance && !foot.movingGoal && _stepIndex==myStepIndex && !OtherFoot.movingGoal)
        {
            currPos = hit.point;
            foot.MoveTarget(foot.IKGoalPosition, currPos);
            _stepIndex = (_stepIndex + 1) % 2;
        }
        else
        {
            if (!foot.movingGoal)
                foot.SetIKGoalPosition(currPos);
        }

        Debug.DrawLine(hit.point, foot.IKGoalPosition, Color.blue);
        Debug.Log((hit.point - foot.IKGoalPosition).magnitude);
        return currPos;
    }
}
