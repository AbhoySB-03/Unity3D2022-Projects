using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimationRagdollBlender : MonoBehaviour
{
    
    private class BoneInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }


    // Start is called before the first frame update
    private bool isRagdoll;
    [SerializeField] private LayerMask notMe;
    private Rigidbody[] rigidbodies;
    [SerializeField] private string getUpBackStateName = "GetUpBack";
    [SerializeField] private string getUpBackClipName = "GetUpBack";
    [SerializeField] private string getUpFrontStateName = "GetUpFront";
    [SerializeField] private string getUpFrontClipName = "GetUpFront";
    [SerializeField] private float getUpTime, stableHipSpeed, maxCollisionRelVelBeforeRagdoll;

    private Transform[] bones;
    private int getUpDir;
    private float curGetUpTime;
    private Collider myCol;

    public enum HumanState
    {
        Animation, Ragdoll, Switching, GettingUp
    }

    public HumanState currentState;
    private Transform hipBone;
    private Rigidbody hipRigidBody;
    private CharacterController myBody;
    private Animator anim;
    private BoneInfo[] animationBonesBack;
    private BoneInfo[] animationBonesFront;
    private BoneInfo[] ragdollBones;
    [SerializeField] private float boneResetTime = 0.5f;
    private float currentBoneResetTime;
    private float hipToTransformDis;
    void Awake()
    {
        anim = GetComponent<Animator>();
        rigidbodies= GetComponentsInChildren<Rigidbody>();
        hipBone = anim.GetBoneTransform(HumanBodyBones.Hips);
        bones=hipBone.GetComponentsInChildren<Transform>();
        hipToTransformDis=Vector3.Distance(transform.position, hipBone.position);
        hipRigidBody= hipBone.GetComponent<Rigidbody>();
        animationBonesBack=new BoneInfo[bones.Length];
        animationBonesFront=new BoneInfo[bones.Length];
        ragdollBones=new BoneInfo[bones.Length];
        myBody=GetComponent<CharacterController>();
        for (int i=0; i<bones.Length; i++)
        {
            animationBonesBack[i]=new BoneInfo();
            animationBonesFront[i]=new BoneInfo();
            ragdollBones[i]=new BoneInfo();
        }

        PopulateAnimationStartBoneInfos(getUpBackClipName, animationBonesBack);
        PopulateAnimationStartBoneInfos(getUpFrontClipName, animationBonesFront);
        currentBoneResetTime= 0;
        currentState = HumanState.Animation;
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case HumanState.Animation:                
                anim.enabled = true;
                myBody.enabled = true;
                break;
            case HumanState.Ragdoll:
                if (!isRagdoll)
                    StartRagdoll();
                if (Physics.Raycast(hipBone.position, Vector3.down, hipToTransformDis, notMe))
                {
                    Debug.Log("Grounded");
                    if (hipRigidBody.velocity.magnitude <= stableHipSpeed)
                    {
                        curGetUpTime += Time.deltaTime;
                    }
                    if (curGetUpTime >= getUpTime)
                    {
                        currentState = HumanState.Switching;
                    }

                }
                else
                {
                    curGetUpTime = 0;
                }
                break;
            case HumanState.Switching:
                Switching();
                break;
            case HumanState.GettingUp:
                if (!(anim.GetCurrentAnimatorStateInfo(0).IsName(getUpBackStateName) || anim.GetCurrentAnimatorStateInfo(0).IsName(getUpFrontStateName)))
                {
                    currentState= HumanState.Animation;
                }
                break;


        }
        
    }

    void DisableRagdoll()
    {
        foreach (Rigidbody rb in rigidbodies)
            if (rb.gameObject != gameObject)
                rb.isKinematic = true;
    }

    void StopRagdoll()
    {
        AlignHipRotation();
        AlignHipPosition();
        populateBoneInfos(ragdollBones);
        DisableRagdoll();
        isRagdoll = false;
    }


    void StartRagdoll()
    {
        myBody.enabled= false;
        anim.enabled = false;
        foreach (Rigidbody rb in rigidbodies) 
            if (rb.gameObject!=gameObject)
                rb.isKinematic= false;
        isRagdoll= true;
        curGetUpTime= 0;
        
    }

    void AlignHipRotation()
    {
        Vector3 originalHipPosition=hipBone.position;
        Quaternion originalHipRotation=hipBone.rotation;
        getUpDir = (int)Mathf.Sign(Vector3.Dot(hipBone.forward, Vector3.down));
        Vector3 desireForwardDirection = hipBone.up * getUpDir;
        if (Physics.Raycast(originalHipPosition, Vector3.down, out RaycastHit hit, 1f, notMe))
        {
            desireForwardDirection = Vector3.ProjectOnPlane(desireForwardDirection, hit.normal);
        }
        transform.rotation =Quaternion.LookRotation(desireForwardDirection);
        hipBone.rotation = originalHipRotation;
        hipBone.position = originalHipPosition;
    }

    void AlignHipPosition()
    {
        Vector3 desiredPosition = hipBone.position;
        transform.position = desiredPosition;
        if (Physics.Raycast(desiredPosition, Vector3.down, out RaycastHit hit, 1f, notMe))
        {
            transform.position = hit.point;

        }
        hipBone.position = desiredPosition;
        
    }
    void populateBoneInfos(BoneInfo[] boneInfos)
    {
        for (int i = 0; i < boneInfos.Length; i++)
        {
            boneInfos[i].position = bones[i].localPosition;
            boneInfos[i].rotation = bones[i].localRotation;
        }

    }

    void PopulateAnimationStartBoneInfos(string clipName, BoneInfo[] boneInfos)
    {
        Vector3 originalPosition=transform.position;
        Quaternion originalRotation=transform.rotation;
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 0);
                populateBoneInfos(boneInfos);
                break;
            }
        }

        transform.position= originalPosition;
        transform.rotation= originalRotation;

    }

    void Switching()
    {
        if (isRagdoll)
            StopRagdoll();
        currentBoneResetTime += Time.deltaTime;
        float resetPercentage = currentBoneResetTime / boneResetTime;
        if (getUpDir == 1)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].localPosition = Vector3.Lerp(ragdollBones[i].position, animationBonesFront[i].position, resetPercentage);
                bones[i].localRotation = Quaternion.Lerp(ragdollBones[i].rotation, animationBonesFront[i].rotation, resetPercentage);

            }
        }
        else
        {
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].localPosition = Vector3.Lerp(ragdollBones[i].position, animationBonesBack[i].position, resetPercentage);
                bones[i].localRotation = Quaternion.Lerp(ragdollBones[i].rotation, animationBonesBack[i].rotation, resetPercentage);

            }
        }
        if (resetPercentage >= 1)
        {
            currentState = HumanState.GettingUp;
            currentBoneResetTime= 0;
            anim.enabled = true;
            if (getUpDir == 1)
                anim.Play(getUpFrontStateName);
            else
                anim.Play(getUpBackStateName);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        Transform LeftFoot, RightFoot;
        LeftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        RightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);

        if (Physics.Raycast(LeftFoot.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit lhit, 1, notMe))
        {
            if (Vector3.Dot(Vector3.up, LeftFoot.position - lhit.point) < 0.1f)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
                anim.SetIKPosition(AvatarIKGoal.LeftFoot, Vector3.Lerp(LeftFoot.position,lhit.point + Vector3.up * 0.1f,0.5f));
                anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.Slerp(anim.GetIKRotation(AvatarIKGoal.LeftFoot),
                    Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, lhit.normal), lhit.normal),0.5f));
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
            }
        }
        if (Physics.Raycast(RightFoot.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit rhit, 1,notMe))
        {
            if (Vector3.Dot(Vector3.up, RightFoot.position - rhit.point) < 0.1f)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
                anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
                anim.SetIKPosition(AvatarIKGoal.RightFoot, Vector3.Lerp(RightFoot.position, rhit.point + Vector3.up * 0.1f, 0.5f));
                anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.Slerp(anim.GetIKRotation(AvatarIKGoal.RightFoot),
                    Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, rhit.normal),rhit.normal),0.5f));
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
            }
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > maxCollisionRelVelBeforeRagdoll)
        {
            currentState = HumanState.Ragdoll;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Objects"))
        {
            currentState = HumanState.Ragdoll;
        }
    }

}
