using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PhysicsIK : MonoBehaviour
{
    [Header("Inverse Kinematics")]
    [SerializeField] private int chainLength=2;
    [SerializeField] private int iterations=10;
    [SerializeField] private float delta = 0.001f;
    [SerializeField] private Transform pole;
    [Range(0f,1f)] public float IKStrength = 1f;
    [SerializeField] private float springForce=20, damping=100;
    [SerializeField] private float maxSnapForce = 500;
    [SerializeField] private bool useTransformTarget;
    [SerializeField] private Transform target;
    [Header("Stepping")]
    [SerializeField] private float maxMoveTime = 0.25f;
    [SerializeField] private float stepHeight;
    [SerializeField] private AnimationCurve stepCurve;
    

    public float completeLength;
    public Vector3 IKGoalPosition;

    private Vector3[] bonePositions;
    private Rigidbody[] boneRigidbodies;
    private float[] bonesLength;
    private Vector3 stepStartPos, stepEndPos;
    public bool movingGoal;
    private float moveTime;

    private void Awake()
    {
        moveTime = maxMoveTime;        
        Init();
        stepStartPos = IKGoalPosition;
        stepEndPos = IKGoalPosition;

    }
    private void Update()
    {
        MovingGoal();      

    }


    private void FixedUpdate()
    {
        ResolveIK();
        SetForces();
    }
    private void Init()
    {
        // Initilize Arrays
        bonePositions= new Vector3[chainLength+1];
        boneRigidbodies= new Rigidbody[chainLength+1];
        bonesLength= new float[chainLength];

        completeLength = 0;

        // Populate the arrays
        var current = transform;
        for (int i =chainLength; i >= 0; i--)
        {
            boneRigidbodies[i]=current.GetComponent<Rigidbody>();
            bonePositions[i]=current.position;
            if (i<boneRigidbodies.Length-1)
            {
                bonesLength[i] = Vector3.Distance(bonePositions[i + 1], bonePositions[i]);
                completeLength += bonesLength[i];
            }

            current=current.parent;
        }
        
        // Default IKGoalPosition is set to the Foot Position
        IKGoalPosition=transform.position;
        if (useTransformTarget)
            IKGoalPosition=target.position;

        // Stepping Parameters
        

    }

    // Playing the Step Animation
    private void MovingGoal()
    {
        if (moveTime <= maxMoveTime)
        {
            moveTime += Time.deltaTime;
            var Pos = Vector3.Lerp(stepStartPos, stepEndPos, moveTime / maxMoveTime)+Vector3.up*stepHeight*stepCurve.Evaluate(moveTime/maxMoveTime);
            if (useTransformTarget)
                target.position= Pos;
            IKGoalPosition=Pos;
        }
        else
        {
            movingGoal= false;
        }

    }
    private void ResolveIK()
    {
        // Use Transform Object as the IKGoal. Usefull for Debuging
        if (useTransformTarget)
            IKGoalPosition=target.position;

        // If chainLength is changed in Between, Readjust all
        if (bonesLength.Length != chainLength)
            Init();

        var current = transform;
        for (int i = chainLength; i >= 0; i--)
        {
            bonePositions[i] = current.position;            
            current = current.parent;
        }

        // Calculating Positions of each Bones using IK

        Vector3 targetDisplacement = IKGoalPosition - bonePositions[0];
        if (targetDisplacement.sqrMagnitude >= chainLength * chainLength)
        {
            var dir = targetDisplacement.normalized;
            bonePositions[bonePositions.Length-1] = dir*completeLength;
            for (int i = 1; i < bonePositions.Length; i++)
                bonePositions[i] = bonePositions[i - 1] + dir * bonesLength[i - 1];
        }
        else
        {
            // Iteration used for FABRIK Algorithm
            for (int j = 0; j < iterations; j++)
            {
                Vector3 rootPos = bonePositions[0];
                bonePositions[bonePositions.Length - 1] = IKGoalPosition;
                // Back
                for (int i = bonePositions.Length - 2; i >= 0; i--)
                {
                    Vector3 dir = (bonePositions[i] - bonePositions[i + 1]).normalized;
                    bonePositions[i] = bonePositions[i+1]+dir * bonesLength[i];
                }

                // Forward
                for (int i = 0; i<bonePositions.Length; i++)
                {
                    if (i == 0)
                    {
                        bonePositions[i] = rootPos;
                        continue;
                    }
                    
                    Vector3 dir = (bonePositions[i]-bonePositions[i-1]).normalized;

                    // Leaf bone doesn't need to be Adjusted for the Pole
                    if (i == bonePositions.Length - 1) {
                        bonePositions[i] = bonePositions[i - 1] + dir * bonesLength[i - 1];
                        continue;
                    }

                    //Pole Adjust
                    Vector3 polePlaneNormal = (IKGoalPosition - bonePositions[0]).normalized;
                    Vector3 boneDir =Vector3.ProjectOnPlane(bonePositions[i]-bonePositions[0],polePlaneNormal);
                    Vector3 poleDir = Vector3.ProjectOnPlane(pole.position - bonePositions[0], polePlaneNormal);

                    Quaternion poleAdjustRotation = Quaternion.FromToRotation(boneDir, poleDir);
                    bonePositions[i] =bonePositions[i - 1] + (poleAdjustRotation* dir).normalized * bonesLength[i - 1];
                    
                }

                
                
            }

            for (int i=0; i<bonePositions.Length-1; i++) 
            {
                Debug.DrawLine(bonePositions[i], bonePositions[i+1], Color.red);
            }
        }


    }

    private void SetForces()
    {
        // Move the Bone Joints to the Calculated Positions
        for (int i=0; i < boneRigidbodies.Length; i++)
        {
            Vector3 desVel =(bonePositions[i]- boneRigidbodies[i].transform.position)*springForce;
            boneRigidbodies[i].AddForce(Vector3.ClampMagnitude(damping * IKStrength * (desVel - boneRigidbodies[i].velocity),maxSnapForce));
        }
    }

    // Called to Set IKGoal
    public void SetIKGoalPosition(Vector3 Pos)
    {
        if (movingGoal)
            return;
        if (useTransformTarget)
        {
            target.position= Pos;
        }
        IKGoalPosition = Pos;
    }

    // Called when the Foot Step Animation will be played
    public void MoveTarget(Vector3 startPos,Vector3 endPos)
    {
        
        stepStartPos = startPos;
        stepEndPos = endPos;
        movingGoal = true;
        moveTime= 0;
    }

}
