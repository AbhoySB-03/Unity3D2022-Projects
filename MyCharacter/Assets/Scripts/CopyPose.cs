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
        if (maxLerpRestoreTime > lerpRestoreTime)
        {
            SetStrength(Mathf.Lerp(0, Strength, lerpRestoreTime / maxLerpRestoreTime));
            lerpRestoreTime += Time.deltaTime;
        }
    }

    public void SetStrength(float strength)
    {
        confJoint.angularXDrive = SetAngularSpring(confJoint.angularXDrive,strength);   
        confJoint.angularYZDrive = SetAngularSpring(confJoint.angularYZDrive,strength);        
    }

    JointDrive SetAngularSpring(JointDrive jointDrive, float spring)
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
