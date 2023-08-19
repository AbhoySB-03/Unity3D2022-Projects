using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balancer : MonoBehaviour
{
    public float headBalanceSpring, headBalanceDamping;
    public float headBalanceAngularSpring, headBalanceAngularDamping;
    public float maxHeadDisplacement=1.5f;
    public LayerMask notMe;
    public GameObject head, lFoot, rFoot;

    [SerializeField] private ProceduralFootPlacement lFootPlacement, rFootPlacement;

    private Rigidbody _headRB;
    private PhysicsIK lFootIK, rFootIK;
    private float height;
    private Rigidbody body;

    private int stepIndex;

    enum BalanceState
    {
        Balanced, Unbalanced
    }

    BalanceState _state;
    private Vector3 footCentre
    {
        get
        {
            return (lFoot.transform.position+rFoot.transform.position)/2;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        body= GetComponent<Rigidbody>();
        lFootIK=lFoot.GetComponent<PhysicsIK>();
        rFootIK=rFoot.GetComponent<PhysicsIK>();
        _headRB=head.GetComponent<Rigidbody>();
        Init();
        stepIndex=0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (_state)
        {
            case BalanceState.Balanced:
                BalancedBehavior();
                break;
            case BalanceState.Unbalanced:
                UnbalancedBehavior(); break;

        }
    }

    void Init()
    {
        height=Vector3.Distance(head.transform.position, footCentre);
    }

    void BalancedBehavior()
    {
        if (!Physics.Raycast(head.transform.position, Vector3.down, out RaycastHit hit,height*1.1f, notMe))
        {
            _state = BalanceState.Unbalanced;
            lFootIK.IKStrength= 0;
            rFootIK.IKStrength = 0;            
            return;
        }

        Vector3 headDisplacement = head.transform.position - footCentre;
        headDisplacement.y = 0;
        if (headDisplacement.sqrMagnitude > maxHeadDisplacement * maxHeadDisplacement)
        {
            _state= BalanceState.Unbalanced;
            lFootIK.IKStrength = 0;
            rFootIK.IKStrength = 0;
            return;
        }
        Vector3 desHeadPos = footCentre + Vector3.up * height;
        Vector3 desHeadVel = headBalanceSpring * (desHeadPos - head.transform.position);
        _headRB.AddForce(headBalanceDamping*(desHeadVel-_headRB.velocity));
        _headRB.angularVelocity = headBalanceAngularSpring * Vector3.Cross(head.transform.up, Vector3.up);
        
        
    }

    void UnbalancedBehavior()
    {

    }
}
