using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed, runSpeed, sprintSpeed, jumpSpeed, aimedMoveSpeed;
    public float accTime, tiltFactor;
    public Transform cameraPoint;
    public float cameraShift;
    Vector3 acceleration, defaultCamPivotPos, defaultCamPointPos;
    public Transform camPivot;
    bool walk, sprint, jump;
    CharacterController body;
    Animator anim;
    Vector3 gravityVelocity;
    Vector3 moveDir;
    Vector3 forward;
    Vector3 targetDir;
    PlayerCombat pc;

    void Start()
    {
        body = GetComponent<CharacterController>();   
        anim = GetComponent<Animator>();
        pc=GetComponent<PlayerCombat>();
        forward=transform.forward;
        defaultCamPivotPos = camPivot.GetComponent<TPSCamFollow>().CamOffset;
        defaultCamPointPos = cameraPoint.position-transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();        
        Animate();    
        
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void LateUpdate()
    {
        Look();
        DynamicCameraMovement();
    }
    void GetInputs()
    {
        if (body.isGrounded)
        moveDir = Vector3.ProjectOnPlane(camPivot.forward * Input.GetAxis("Vertical") + camPivot.right * Input.GetAxis("Horizontal"),transform.up).normalized;
        sprint = Input.GetKey(KeyCode.LeftShift);
        walk = Input.GetKeyDown(KeyCode.LeftAlt)?(!walk):walk;
        jump = Input.GetKeyDown(KeyCode.Space);   
    }
    void Movement()
    {
        float speed;
        if (body.isGrounded)
        {
            if (jump)
            {
                gravityVelocity = Vector3.up * jumpSpeed;
                anim.SetTrigger("Jump");
            }
            else
                gravityVelocity = Vector3.down;
        }
        else
        {
            gravityVelocity+=Physics.gravity* Time.deltaTime;
        }
        if (pc.targeting || pc.shielding)
        {
            speed = aimedMoveSpeed;
        }
        else if (sprint)
        {
            speed = sprintSpeed;
            
        }
        else if (walk)
        {
            speed = walkSpeed;
        }
        else
        {
            speed = runSpeed;
        }
        acceleration= ((moveDir * speed)-Vector3.ProjectOnPlane(body.velocity,Vector3.up))/accTime;
        body.Move((Vector3.ProjectOnPlane((body.velocity+acceleration*Time.deltaTime),Vector3.up)+gravityVelocity)*Time.deltaTime);
    }

    void Animate()
    {        
        anim.SetFloat("Speed", Vector3.ProjectOnPlane(body.velocity, Vector3.up).magnitude);
        anim.SetBool("InAir", !body.isGrounded);
        anim.SetFloat("x", Vector3.Dot(transform.right, body.velocity));
        anim.SetFloat("z", Vector3.Dot(transform.forward, body.velocity));

        targetDir=camPivot.forward;
        if (pc.shielding && pc.target)
        {
            targetDir = (pc.target.transform.position - transform.position).normalized;
        }
        anim.SetFloat("Angle", Vector3.SignedAngle(transform.forward,Vector3.ProjectOnPlane(targetDir,transform.right), transform.right));
        
    }

    void Look()
    {
        if (pc.shielding)
        {
            if (pc.target != null)
            {
                forward = (pc.target.transform.position - transform.position).normalized;
            }
            else
            {
                forward = camPivot.forward;
            }
        }
        else if (pc.targeting)
        {
            forward = camPivot.forward;
        }
        else if (moveDir.sqrMagnitude > 0)
        {
            forward = body.velocity.normalized;
        }
        Vector3 up = Vector3.up + Vector3.ProjectOnPlane(acceleration * tiltFactor,Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(forward,up),up),10*Time.deltaTime);
    }

    void DynamicCameraMovement()
    {
        if (pc.combatType == 1)
        {
            cameraPoint.position =Vector3.Lerp(cameraPoint.position,transform.position+defaultCamPointPos-body.velocity * cameraShift,Time.deltaTime);
        }
        else
        {
            cameraPoint.position = transform.position+defaultCamPointPos;
        }
        if (pc.combatType == 2 && pc.targeting)
        {
            camPivot.GetComponent<TPSCamFollow>().CamOffset = new Vector3(0, 0.8f, -3);
        }
        else
        {
            camPivot.GetComponent<TPSCamFollow>().CamOffset =defaultCamPivotPos;
        }
    }

    
}
