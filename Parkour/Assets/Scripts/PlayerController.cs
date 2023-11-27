using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform vaultStartCheckPoint, wallRunCheckPoint;
    [SerializeField] private Transform cam;
    [SerializeField] private LayerMask Ground, Vaultable, Wall;
    [SerializeField] private float wallRunTime = 2, wallRunJumpFactor=1.5f;
    [SerializeField] private float slideTime = 2, crouchJumpFactor=1.5f;
    [SerializeField] private float crouchHeightScale = 0.5f;
    
    [SerializeField] private float walkSpeed=2, runSpeed=6, jumpForce=10, slideSpeed=12, turnSpeed=150, vaultUpSpeed;


    private bool crouched;
    private Vector3 moveDir, slideDir;
    private bool runButton, jumpButton, crouchButton;
    private Rigidbody body;
    private CapsuleCollider capCol;
    private float wallRunTimeLeft, slideTimeLeft;
    private Vector3 defaultScale;
    
    public bool isGrounded
    {
        get
        {
            return Physics.SphereCast(new Ray(transform.position, -transform.up), capCol.radius, capCol.height/2 - capCol.radius+0.1f, Ground);
        }
    }


    bool canVault{ 
        get
        {
            if (moveDir == Vector3.zero)
                return false;

            return Physics.Raycast(transform.position + transform.up * (-capCol.height / 2 + 0.02f), moveDir, capCol.radius + 0.3f, Vaultable)
                && !Physics.Raycast(transform.position + transform.up * (capCol.height / 2 + 0.2f), moveDir, capCol.radius + 0.3f, Vaultable);
        }
    }


    bool canWallRun
    {
        get
        {
            
            if (IsWallPresent(out RaycastHit hit, (int)Input.GetAxisRaw("Horizontal")))
            {                
                return body.velocity.magnitude>walkSpeed;
            }
            return false;
        }
    }
    public enum PlayerState
    {
        Grounded, InAir, Vaulting, Sliding, WallRunning
    }

    public PlayerState state;
    // Start is called before the first frame update
    void Awake()
    {
        state = PlayerState.Grounded;
        capCol = GetComponent<CapsuleCollider>();
        body = GetComponent<Rigidbody>();
        defaultScale=transform.localScale;
    }

    private void Update()
    {
        GetInput();
        SetJump();
        SetCrouch();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.Grounded:
                GroundedMovement();
                break;
            case PlayerState.InAir:
                InAirMovement(); break;
            case PlayerState.Vaulting:
                VaultMovement(); break;
            case PlayerState.WallRunning:
                WallRunningMovement(); break;
            case PlayerState.Sliding:
                SlidingMovement(); break;
            
        }
        
    }

    private void LateUpdate()
    {
        SetRotation();
    }

    void GroundedMovement()
    {
        if (!isGrounded)
        {
            state = PlayerState.InAir;
        }
        Vector3 desVelocity = moveDir.normalized*(runButton? runSpeed: walkSpeed); 
        desVelocity.y=body.velocity.y;
        body.AddForce((desVelocity-body.velocity)/0.2f, ForceMode.Acceleration);

        
    }

    void VaultMovement()
    {
        body.velocity = transform.up * vaultUpSpeed + moveDir* (runButton ? runSpeed : walkSpeed);
        if (!canVault)
        {
            body.velocity = Vector3.ProjectOnPlane(body.velocity, transform.up);
            state= PlayerState.InAir;
        }
    }

    private void WallRunningMovement()
    {
        wallRunTimeLeft -= Time.deltaTime;
        if (wallRunTimeLeft < 0.5f*wallRunTime)
        {
            body.useGravity = true;
        }
        if (wallRunTimeLeft < 0)
        {
            state= PlayerState.InAir;
        }
        if (IsWallPresent(out RaycastHit hit,-1))
        {
            Vector3 forwardVel = Vector3.ProjectOnPlane(moveDir * (runButton ? runSpeed : walkSpeed), hit.normal);
            forwardVel.y = body.velocity.y;
            body.AddForce((forwardVel - body.velocity) / 0.2f, ForceMode.Acceleration);

        }
        else if (IsWallPresent(out RaycastHit hit2,1))
        {
            Vector3 forwardVel = Vector3.ProjectOnPlane(moveDir * (runButton ? runSpeed : walkSpeed), hit2.normal);
            forwardVel.y = body.velocity.y;
            body.AddForce((forwardVel - body.velocity) / 0.2f, ForceMode.Acceleration);
        }
        else
        {
            state=PlayerState.InAir;
            body.useGravity = true;
        }



    }
    void InAirMovement()
    {
        if (canWallRun)
        {
            state = PlayerState.WallRunning;
            body.useGravity = false;
            wallRunTimeLeft = wallRunTime;
            return;
        }
        if (canVault)
        {
            state = PlayerState.Vaulting;
            return;
        }
        if (isGrounded)
        {
            state= PlayerState.Grounded;
            return;
        }
        
    }


    void SlidingMovement()
    {
        slideTimeLeft -= Time.deltaTime;
        if (slideTimeLeft < 0.5f*slideTime)
        {
            body.useGravity= true;
        }
        if (slideTimeLeft < 0)
        {
            state = PlayerState.InAir;
        }
    }


    void GetInput()
    {
        moveDir = transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");

        runButton = Input.GetKey(KeyCode.LeftShift);
        jumpButton = Input.GetKeyDown(KeyCode.Space);
        crouchButton =Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C);

        transform.Rotate(0, Input.GetAxisRaw("Mouse X") * turnSpeed*Time.deltaTime, 0);

        
    }

    void SetJump() 
    {
        if (jumpButton)
        {
            switch (state)
            {
                case PlayerState.WallRunning:
                    Vector3 side = Vector3.ProjectOnPlane(transform.up, Vector3.up).normalized;
                    body.AddForce((transform.up + side * 0.9f).normalized * jumpForce * wallRunJumpFactor, ForceMode.Impulse);
                    break;
                case PlayerState.Grounded:
                    if (crouched)
                    {
                        body.AddForce(transform.up*jumpForce*crouchJumpFactor, ForceMode.Impulse);
                    }
                    else
                    body.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                    break;
            }
        }
    }


    void SetCrouch()
    {
        Vector3 scale= defaultScale;
        if (crouchButton)
        {
            var v = body.velocity;
            v.y = 0;
            if ( v.magnitude>walkSpeed && !crouched)
            {
                state = PlayerState.Sliding;
                body.AddForce(moveDir *slideSpeed, ForceMode.Impulse);
                body.useGravity = isGrounded;
                slideTimeLeft = slideTime;
            }

            if (isGrounded && !crouched)
            {
                body.AddForce(transform.up * -2f, ForceMode.Impulse);
            }
            scale.y = defaultScale.y*(crouched ? 1: crouchHeightScale);
            crouched = !crouched;
            transform.localScale = scale;
        }

        
    }
    void SetRotation()
    {
        Vector3 up = Vector3.up;
        if (state == PlayerState.WallRunning)
        {
            if (IsWallPresent(out RaycastHit hit, 1)) {
                up = hit.normal * 0.25f + Vector3.up;
            }
            else if (IsWallPresent(out RaycastHit hit2,-1))
            {
                up= hit2.normal * 0.25f+Vector3.up;
                 
            }


        }

        transform.rotation=Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.forward, up), 30*Time.deltaTime);
    }

    bool IsWallPresent(out RaycastHit hit, int dir = 1)
    {
        if (Physics.Raycast(transform.position-transform.up*(capCol.height/2-0.1f), dir*transform.right, out RaycastHit h, capCol.radius + 0.25f, Wall))
        {
            hit = h;
            return true;
        }
        hit=new RaycastHit();
        return false;
    }

}
