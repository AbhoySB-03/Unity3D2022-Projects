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
    [SerializeField] private GameObject hip,leftFoot, rightFoot;
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
    private Rigidbody _mainBody;
    // Start is called before the first frame update
    void Start()
    {
        _body = hip.GetComponent<Rigidbody>();
        _hipHeight = Vector3.Distance(hip.transform.position, MidFoot());
        _pose = hip.GetComponent<CopyPose>();
        _pose.SetStrength(balanceStrength);
        _mainBody=GetComponent<Rigidbody>();
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
        _mainBody.isKinematic = (_state!=BalanceState.Unbalanced);
        Debug.Log(_state);
    }

    void BalancedBehavior()
    {        
        if (Vector3.Angle(Vector3.up, hip.transform.up)>maxVerticleAngle ||
            Vector3.Distance(leftFoot.transform.position, rightFoot.transform.position) > maxLegDistance ||
            Vector3.Angle(Vector3.up, hip.transform.position-MidFoot())>maxHipToMidFootAngle ||
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
            _state = BalanceState.GettingUp;
            _mainBody.isKinematic = true;
            GetUp();
            SetGetUpAnimation();
            StartCoroutine(GettingUp(0.1f));     
            
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

    void GetUp(int _=0)
    {
        Vector3 forward;
        if (Vector3.Dot(hip.transform.forward, Vector3.up) > 0)
        {
            forward=-hip.transform.up;
        }
        else
        {            
            forward=hip.transform.up;
        }
         
        forward.y = 0;
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        if (Physics.Raycast(hip.transform.position, Vector3.down, out RaycastHit hit, _hipHeight, notMe))
        {
            Vector3 hipPos=hip.transform.position;
            Quaternion hipRot = hip.transform.rotation;

            transform.position = hit.point;

            hip.transform.rotation = hipRot;
            hip.transform.position = hipPos;
        }
        
    }
    void SetGetUpAnimation()
    {
        string animState;
        if (Vector3.Dot(hip.transform.forward, Vector3.up) > 0)
        {
            animState = GetUpBackStateName;
        }
        else
        {
            animState = GetUpChestStateName;
        }
        refAnimator.Play(animState);

    }

    IEnumerator GettingUp(float sec)
    {
        yield return new WaitForSeconds(sec);
        SetBalanced();
    }
    bool CheckHipGrounded()
    {
        return Physics.Raycast(hip.transform.position, Vector3.down, (_state == BalanceState.Balanced) ? _hipHeight * 1.5f : _hipHeight / 2, notMe) 
            && _body.velocity.magnitude < maxStableSpeed;       
        
    }
}
