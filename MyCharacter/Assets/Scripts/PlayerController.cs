using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed, runSpeed, sprintSpeed, crouchWalkSpeed, jumpSpeed;
    [SerializeField] private float accelerationFactor, accelAnimFactor;
    [SerializeField] private Transform cam;
    [SerializeField] private bool useDynamicCC;
    private AnimationRagdollBlender animRagdol;
    private bool walk, crouch;
    private CharacterController body;
    private Vector3 currentVelocity,desiredVelocity;
    private float yVel;
    private Vector3 acceleration;
    private Animator anim;
    private float defaultCCHeight,defaultCCYpos;

    void Start()
    {
        anim = GetComponent<Animator>();
        animRagdol=GetComponent<AnimationRagdollBlender>();
        body= GetComponent<CharacterController>();
        walk = true;
        defaultCCHeight = body.height;
        defaultCCYpos = body.center.y;
    }

    // Update is called once per frame
    void Update()
    {
        
        GetInput();
        Animate();
        Gravity();
        MoveBody();
        
        
    }

    

    private void LateUpdate()
    {
        if (animRagdol.currentState == AnimationRagdollBlender.HumanState.Animation)
            SetRotation();
        if (useDynamicCC)
            DynamicCharacterController();
    }

    
    void GetInput()
    {
        if (body.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                anim.SetTrigger("Jump");
            }

            desiredVelocity = cam.transform.forward * Input.GetAxisRaw("Vertical") + cam.transform.right * Input.GetAxisRaw("Horizontal");
            desiredVelocity.y = 0;
            desiredVelocity.Normalize();

            float speed = walk ? walkSpeed : runSpeed;
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                walk = !walk;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                crouch = !crouch;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = sprintSpeed;
            }
            if (crouch)
            {
                speed = crouchWalkSpeed;
            }
            desiredVelocity *= speed;
        }
        

    }

    private void DynamicCharacterController()
    {
        if (!body.isGrounded)
        {
            body.center=new Vector3(0, defaultCCYpos, 0);
            body.height = defaultCCHeight;
            return;
        }
        Transform head=anim.GetBoneTransform(HumanBodyBones.Head);
        Transform lFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        Transform rFoot=anim.GetBoneTransform(HumanBodyBones.RightFoot);

        float shortHeight=head.position.y-Mathf.Max(lFoot.position.y,rFoot.position.y);
        //body.height = shortHeight;
        body.center = new Vector3(0, defaultCCYpos + Mathf.Abs(lFoot.position.y - rFoot.position.y));


    }
    void MoveBody()
    {
        
        acceleration = (desiredVelocity - currentVelocity)/Time.deltaTime;
        currentVelocity += acceleration * Time.deltaTime;
        body.Move((currentVelocity+ Vector3.up * yVel) * Time.deltaTime);        
        
    }


    void Jump(int i)
    {
        yVel = jumpSpeed;
    }
    void Gravity()
    {
        if (!body.isGrounded)
        {
            yVel += Physics.gravity.y * Time.deltaTime;
        }
    }

    void SetRotation()
    {
        Vector3 forward=transform.forward;
        if (desiredVelocity.magnitude != 0)            
            forward = desiredVelocity.normalized;
        Vector3 up = Vector3.up +acceleration*accelAnimFactor;
        forward = Vector3.ProjectOnPlane(forward, up);
        Quaternion desiredRotation= Quaternion.LookRotation(forward, up);
        transform.rotation=Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime*6f);

    }

    void Animate()
    {
        
        anim.SetFloat("Speed", currentVelocity.magnitude, 0.1f, Time.deltaTime);
        anim.SetBool("Crouch", crouch);
        anim.SetBool("IsGrounded", body.isGrounded);
    }

    
}
