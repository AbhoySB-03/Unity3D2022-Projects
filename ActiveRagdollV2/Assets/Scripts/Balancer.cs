using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balancer : MonoBehaviour
{
    [Header("Body Parts Assignment")]
    public GameObject head;
    public GameObject lFoot;
    public GameObject rFoot;
    private Rigidbody _headRB;
    private Rigidbody body;

    [Header("Balance Settings")]
    public float balanceSpring;
    public float balanceDamping;
    public float headBalanceAngularSpring, headBalanceAngularDamping;
    public float maxHeadDisplacement = 1.5f;
    private float height;
    private float hipHeight;
    public LayerMask notMe;

    //[Header("Foot Placement")]
    private PhysicsIK lFootIK;
    private PhysicsIK rFootIK;

    [Header("Getting Up Settings")]
    [SerializeField] private float getUpTime=4;
    private float _gettingUpTimeLeft; 


    private bool gettingUp
    {
        get
        {
            return _gettingUpTimeLeft < getUpTime;
        }
    }

    enum BalanceState
    {
        Balanced, Unbalanced, GettingUp
    }

    BalanceState _state;
    private Vector3 footCentre
    {
        get
        {
            Vector3 lPos = lFoot.transform.position;
            Vector3 rPos = rFoot.transform.position;
            float yVal = Mathf.Min(lPos.y, rPos.y);
            lPos.y = yVal;
            rPos.y = yVal;
            return (lPos + rPos) / 2;
        }
    }

    private Vector3 footIKCentre
    {
        get
        {
            Vector3 lPos = lFootIK.IKGoalPosition;
            Vector3 rPos = rFootIK.IKGoalPosition;
            float yVal=Mathf.Min(lPos.y,rPos.y);
            lPos.y=yVal;
            rPos.y=yVal;
            return (lPos+rPos)/2;
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
            case BalanceState.GettingUp:
                GettingUpBehavior(); break;

        }
    }

    void Init()
    {
        height=Vector3.Distance(head.transform.position, footCentre);
        hipHeight=Vector3.Distance(transform.position, footCentre);
    }

    void BalancedBehavior()
    {
        if (!Physics.Raycast(head.transform.position, Vector3.down, out RaycastHit hit,height*1.1f, notMe))
        {
            MakeUnbalanced();           
            return;
        }

        Vector3 headDisplacement = head.transform.position - footCentre;
        headDisplacement.y = 0;
        if (headDisplacement.sqrMagnitude > maxHeadDisplacement * maxHeadDisplacement && !gettingUp)
        {
            MakeUnbalanced();
            return;
        }
        Vector3 desHeadPos = footIKCentre + Vector3.up * height*1.05f;
        Vector3 desHeadVel = balanceSpring * (desHeadPos - head.transform.position);
        _headRB.AddForce(balanceDamping*(desHeadVel-_headRB.velocity));
        _headRB.angularVelocity = headBalanceAngularSpring * Vector3.Cross(head.transform.up, Vector3.up);

        /*
        Vector3 desHipPos = footIKCentre + Vector3.up * hipHeight;
        Vector3 desHipVel = balanceSpring * (desHipPos - transform.position);
        body.AddForce(balanceDamping * (desHipVel - body.velocity));
        */
    }

    void UnbalancedBehavior()
    {
        if (!Physics.Raycast(head.transform.position, Vector3.down, height * 1.1f, notMe))
        {
            _gettingUpTimeLeft = 0;
            return;
        }
        if (_gettingUpTimeLeft >= getUpTime / 2)
        {
            _state = BalanceState.GettingUp;
            return;
        }
        _gettingUpTimeLeft += Time.deltaTime;
    }

    void GettingUpBehavior()
    {
        if (!Physics.Raycast(head.transform.position, Vector3.down, out RaycastHit hit, height * 1.1f, notMe))
        {
            MakeUnbalanced();
            return;
        }
        if (!gettingUp)
        {
            _state = BalanceState.Balanced;
            return;
        }
        _gettingUpTimeLeft += Time.deltaTime;
        
        Vector3 desHeadPos;
        if (_gettingUpTimeLeft < 0.75 * getUpTime)
        {
            desHeadPos = hit.point + Vector3.up * height/2;
            
        }
        else
        {
            desHeadPos = hit.point + Vector3.up * height;
            lFootIK.IKStrength = 2 * (_gettingUpTimeLeft / getUpTime) - 1;
            rFootIK.IKStrength = 2 * (_gettingUpTimeLeft / getUpTime) - 1;
        }
        Vector3 desHeadVel = balanceSpring * (desHeadPos - head.transform.position);
        _headRB.AddForce(balanceDamping * (desHeadVel - _headRB.velocity));
        _headRB.angularVelocity = headBalanceAngularSpring * Vector3.Cross(head.transform.up, Vector3.up);
    }

    void MakeUnbalanced()
    {
        _state = BalanceState.Unbalanced;
        lFootIK.IKStrength = 0;
        rFootIK.IKStrength = 0;
        _gettingUpTimeLeft = 0;
    }
}
