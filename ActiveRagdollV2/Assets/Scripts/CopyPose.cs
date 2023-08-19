using UnityEngine;


public class CopyPose : MonoBehaviour
{
    [SerializeField]private float Strength=1500;
    [SerializeField] private Transform copyTransform;
    private ConfigurableJoint confJoint;   
    public Quaternion StartRot;
    private float lerpRestoreTime, maxLerpRestoreTime;
    
    // Start is called before the first frame update
    void Awake()
    {
        confJoint= GetComponent<ConfigurableJoint>();
        StartRot = copyTransform.localRotation;
        SetStrength(Strength);
        
    }

    
    // Update is called once per frame
    

    private void FixedUpdate()
    {
        ConfigurableJointExtensions.SetTargetRotationLocal(confJoint, copyTransform.localRotation, StartRot);
        confJoint.targetPosition=copyTransform.localPosition;
        Debug.DrawLine(transform.TransformPoint(confJoint.targetPosition), copyTransform.position);
        if (maxLerpRestoreTime > lerpRestoreTime)
        {
            SetStrength(Mathf.Lerp(0, Strength, lerpRestoreTime / maxLerpRestoreTime));
            lerpRestoreTime += Time.deltaTime;
        }
    }

    public void SetStrength(float strength)
    {
        confJoint.angularXDrive = SetSpring(confJoint.angularXDrive,strength);   
        confJoint.angularYZDrive = SetSpring(confJoint.angularYZDrive,strength);
        confJoint.yDrive = SetSpring(confJoint.yDrive,strength);
    }

    JointDrive SetSpring(JointDrive jointDrive, float spring)
    {
        jointDrive.positionSpring = spring;
        return jointDrive;
    }

    public void RestoreStrength()
    {
        SetStrength(Strength);
    }

    public void RestoreStrengthSmooth(float restoreTime)
    {
        maxLerpRestoreTime= restoreTime;
        lerpRestoreTime = 0;
    }

    

}
