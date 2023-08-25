using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    public Transform holdPoint;
    public float castRange, castRadius;
    public float forceConstant, forceDamping;
    public float throwForce;
    public float pullpushSpeed;
    public Transform castPoint;
    public LayerMask telekineticObjects;
    bool holding;
    bool release;
    Rigidbody teleObject;
    float objDistance;

    bool hold, throwObject, pull, push;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        if (hold)
        {
            StartHolding();
        }
        if (throwObject && holding)
        {
            Throw();
        }
        if (release)
        {
            StopHolding();
        }
        
    }

    private void FixedUpdate()
    {
        if (holding) { Holding(); }
    }


    void GetInputs()
    {
        push = holding && Input.GetMouseButton(0);
        pull = holding && Input.GetMouseButton(1);
        hold=!holding && Input.GetMouseButtonDown(1);
        release=holding && Input.GetKeyDown(KeyCode.R);
        throwObject=holding && Input.GetKeyDown(KeyCode.F);

    }
    void Holding()
    {
        objDistance=Mathf.Clamp(objDistance,5,castRange);
        holdPoint.position=castPoint.position+castPoint.forward*objDistance;
        teleObject.AddForce(((holdPoint.position-teleObject.transform.position)*forceConstant-teleObject.velocity)*forceDamping);
        if (pull)
        {
            objDistance -= pullpushSpeed * Time.deltaTime;
        }
        else if (push)
        {
            objDistance += pullpushSpeed * Time.deltaTime;
        }
        
    }

    void Throw()
    {
        holding= false;
        teleObject.AddForce((holdPoint.position - teleObject.position) * throwForce);
        teleObject = null;       

    }
    void StopHolding()
    {
        teleObject = null;
        holding = false;
    }
    void StartHolding()
    {
        if (Physics.SphereCast(new Ray(castPoint.position, castPoint.forward),  castRadius, out RaycastHit hit, castRange, telekineticObjects))
        {
            holding = true;
            teleObject = hit.rigidbody;
            objDistance = Vector3.Distance(hit.point, castPoint.position);           
        }

    }
}
