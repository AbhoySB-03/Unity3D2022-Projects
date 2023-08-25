using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class HipBalancer : MonoBehaviour
{
    [SerializeField] private float balanceStrength=1000;
    [SerializeField] private float maxVerticleAngle=30;
    [SerializeField] private float maxLegDistance=2;
    [SerializeField] private float maxHipToMidFootAngle=30;
    [SerializeField] private float GettingUpTime=2;
    [SerializeField] private float maxStableSpeed=20;
    [SerializeField] private float StabilityCheckTime=10;
    [SerializeField] private string GetUpBackStateName="GetUpBack", GetUpChestStateName="GetUpChest", idleStateName="idle";
    [SerializeField] private GameObject leftFoot, rightFoot;
    [SerializeField] private LayerMask notMe;
    [SerializeField] private Animator refAnimator;
    private enum BalanceState
    {
        Balanced, Unbalanced, GettingUp
    }

    private BalanceState _state;
    private Rigidbody _body;
    private float _gettingUpTimeLeft;
    private float _balanceStabilityCheckTime;
    private float _hipHeight;
    private CopyPose _pose;

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _hipHeight = Vector3.Distance(transform.position, MidFoot());
        _pose = GetComponent<CopyPose>();
        _pose.SetStrength(balanceStrength);
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case BalanceState.Balanced:
                BalancedBehavior();
                break;
            case BalanceState.Unbalanced:
                UnbalancedBehavior();
                break;
            case BalanceState.GettingUp:
                if (refAnimator.GetCurrentAnimatorStateInfo(0).IsName(idleStateName))
                {
                    _state = BalanceState.Balanced;
                }
                break;

        }

        Debug.Log(_state);
    }

    void BalancedBehavior()
    {        
        if (Vector3.Angle(Vector3.up, transform.up)>maxVerticleAngle ||
            Vector3.Distance(leftFoot.transform.position, rightFoot.transform.position) > maxLegDistance ||
            Vector3.Angle(Vector3.up, transform.position-MidFoot())>maxHipToMidFootAngle ||
            !CheckHipGrounded()) 
        {
            _gettingUpTimeLeft = GettingUpTime;
            SetUnBalanced();
            _state = BalanceState.Unbalanced;
        }
    }

    Vector3 MidFoot()
    {
        return (leftFoot.transform.position + rightFoot.transform.position) / 2; 
    }

    void UnbalancedBehavior()
    {
        if (_gettingUpTimeLeft <= 0)
        {
            GetUp();
            SetBalanced();
            _state= BalanceState.GettingUp;
        }
        else if (CheckHipGrounded())
        {
            _gettingUpTimeLeft -= Time.deltaTime;
        }        
       
    }

    void SetUnBalanced()
    {
        foreach (CopyPose p in GetComponentsInChildren<CopyPose>())
        {
            p.SetStrength(0);
        }
        refAnimator.enabled = false;
    }

    void SetBalanced()
    {
        foreach (CopyPose p in GetComponentsInChildren<CopyPose>())
        {
            p.RestoreStrengthSmooth(0.1f);
        }
        refAnimator.enabled = true;
    }

    void GetUp()
    {
        string animState;
        Vector3 forward;
        if (Vector3.Dot(transform.forward, Vector3.up) > 0)
        {
            animState=GetUpBackStateName;
            forward=-transform.up;
        }
        else
        {
            animState=GetUpChestStateName;
            forward=transform.up;
        }
        refAnimator.Play(animState);
        
    }

    bool CheckHipGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, (_state == BalanceState.Balanced) ? _hipHeight*1.5f : _hipHeight / 2) && _body.velocity.magnitude < maxStableSpeed;
        
    }
}
