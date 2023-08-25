using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mjolnir : MonoBehaviour
{
    [SerializeField] private bool detached=false;
    [SerializeField] private float searchRange, searchDistance;
    [SerializeField] private float attachDistance;
    [SerializeField] private float returnSpeed,returnSpeedKP, moveAccFactor;
    [SerializeField] private float moveSpeed, hitSpeed;
    [SerializeField] private float minSearchTime, hitWaitTime;
    [SerializeField] private int maxTargets;

    [SerializeField] private Transform pivot;
    [SerializeField] private LayerMask enemyLayer;


    


    private float currentHitWaitTime;
    private float searchTimeBeforeHit;
    private ConfigurableJoint myJoint;
    public enum MjolnirState
    {
        Held, Searching, Returning, Stable, Hitting, AfterHit
    }
    public MjolnirState state;
    private Rigidbody rbody;
    private Vector3 moveDir;
    private int targetCounts;
    private Queue<GameObject> targetsQueue;
    private List<GameObject> toHitList;
    private GameObject currentTarget;

    

    private class JointInfos {
     
        private Rigidbody connectedBody;
        private JointDrive angXDrive;
        private JointDrive angYZDrive;
        private ConfigurableJointMotion xMotion,yMotion,zMotion, angXMotion,angYMotion,angZMotion;
        
        public JointInfos(ConfigurableJoint confJ)
        {
            StoreInfos(confJ);
        }
        public void StoreInfos(ConfigurableJoint confJ)
        {
            connectedBody = confJ.connectedBody;
            angXDrive = confJ.angularXDrive;
            angYZDrive= confJ.angularYZDrive;
            xMotion= confJ.xMotion;
            yMotion= confJ.yMotion;
            zMotion= confJ.zMotion;
            angXMotion = confJ.angularXMotion;
            angYMotion= confJ.angularYMotion;
            angZMotion= confJ.angularZMotion;

        }

        public ConfigurableJoint SetInfos(ConfigurableJoint confJ)
        {
            confJ.connectedBody = connectedBody;
            confJ.angularXDrive = angXDrive;
            confJ.angularYZDrive = angYZDrive;
            confJ.xMotion = xMotion;
            confJ.yMotion = yMotion;
            confJ.zMotion = zMotion;
            confJ.angularXMotion = angXMotion;
            confJ.angularYMotion = angYMotion;
            confJ.angularZMotion = angZMotion;

            return confJ;
        }
    }

    JointInfos myJointInfos;

    
    // Start is called before the first frame update
    void Awake()
    {
        
        rbody=GetComponent<Rigidbody>();   
        myJoint=GetComponent<ConfigurableJoint>();
        targetsQueue = new Queue<GameObject>();
        if (myJoint != null)
        {
            myJointInfos = new JointInfos(myJoint);
            state = MjolnirState.Held;
        }
        currentHitWaitTime = hitWaitTime;
        targetCounts = maxTargets;
        toHitList= new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state==MjolnirState.AfterHit)
        {
            if (currentHitWaitTime>0)
            {
                currentHitWaitTime-=Time.deltaTime;
            }
            else
            {
                state = MjolnirState.Searching;
                
            }
        }
        
        
    }

    
    private void FixedUpdate()
    {
        switch (state)
        {
            case MjolnirState.Held:

                break;
            case MjolnirState.Searching:
                SearchingBehavior();
                break;
            case MjolnirState.Hitting:
                Hitting();
                break;
            case MjolnirState.Returning:
                Returning();
                break;
            case MjolnirState.AfterHit:
                AfterHit();
                break;
        }
        if (state ==MjolnirState.Searching || state==MjolnirState.Hitting || state==MjolnirState.AfterHit)
        {
            if (rbody.velocity.magnitude > 0.1f && detached)
            {
                rbody.MoveRotation(Quaternion.LookRotation(rbody.velocity));
            }

        }

    }

    public void TakeHold()
    {
        rbody.interpolation = RigidbodyInterpolation.None;
        transform.SetParent(pivot);
        transform.localPosition = Vector3.zero; transform.localRotation=Quaternion.identity;
        myJoint = myJointInfos.SetInfos(myJoint);
        detached = false;
        targetCounts= maxTargets;
        toHitList.Clear();
        targetsQueue.Clear();
        state = MjolnirState.Held;
        
    }

    void SearchingBehavior()
    {        
        Vector3 towardsDir = pivot.position - transform.position;
        if (Vector3.Distance(pivot.position, transform.position) >= searchDistance)
        {
            rbody.useGravity = true;
        }
        else
        {
            rbody.useGravity = false;
            rbody.velocity = moveDir * moveSpeed;
        }

        if (targetCounts <= 0)
        {
            targetsQueue.Clear();
            return;
        }
        
            Collider[] targetColiders = Physics.OverlapSphere(transform.position, searchRange, enemyLayer);
            GameObject[] targets = (from c in targetColiders select c.gameObject).ToArray();
            foreach (GameObject g in targets)
            {
                
                if (!targetsQueue.Contains(g) && !toHitList.Contains(currentTarget))
                {
                    targetsQueue.Enqueue(g);
                }
            }
        
        if (searchTimeBeforeHit > 0)
        {
            searchTimeBeforeHit -= Time.deltaTime;
            return;
        }
       

        if (targetsQueue.Count > 0)
        {
            currentTarget= targetsQueue.Dequeue();
            toHitList.Add(currentTarget);
            targetCounts--;
            state = MjolnirState.Hitting;
        }
    }

    private void AfterHit()
    {
        rbody.velocity = moveDir * moveSpeed;
    }
    public void Detach()
    {
        myJointInfos.StoreInfos(myJoint);
        myJoint.connectedBody= null;
        myJoint.angularXDrive = new JointDrive();
        myJoint.angularYZDrive= new JointDrive();
        myJoint.angularXMotion = myJoint.angularYMotion = myJoint.angularZMotion = ConfigurableJointMotion.Free;
        myJoint.xMotion = myJoint.yMotion = myJoint.zMotion = ConfigurableJointMotion.Free;
        
        transform.parent = null;
        rbody.interpolation = RigidbodyInterpolation.Interpolate;
        detached = true;
    }

    private void Hitting()
    {
        moveDir=currentTarget.transform.position-transform.position;
        moveDir.Normalize();
        rbody.velocity = moveDir * hitSpeed;
    }

    public void Throw(Vector3 dir)
    {
        Detach();
        moveDir = dir;
        searchTimeBeforeHit = minSearchTime;
        state = MjolnirState.Searching;
    }

    public void Call()
    {
        state = MjolnirState.Returning;
    }

    void Returning()
    {
        if (Vector3.Distance(pivot.position, transform.position) > attachDistance)
        {
            Vector3 velocity = (pivot.position - transform.position) * returnSpeedKP;
            rbody.velocity= Vector3.ClampMagnitude(velocity, returnSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, pivot.rotation, 0.1f);
        }
        else
        {
            rbody.velocity = Vector3.zero;
            TakeHold();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
      if (collision.gameObject.layer==LayerMask.NameToLayer("Ground") && state != MjolnirState.Returning)
        {
            rbody.useGravity = true;
            rbody.velocity= Vector3.zero;
            state = MjolnirState.Stable;
        }  
        if (collision.collider.gameObject.layer==LayerMask.NameToLayer("Enemy") && state==MjolnirState.Hitting){
            currentHitWaitTime = hitWaitTime;
            state = MjolnirState.AfterHit;
        }
    }
}


