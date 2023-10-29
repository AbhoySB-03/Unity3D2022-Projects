using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBalancer : MonoBehaviour
{
    public GameObject Head;
    public FootIKSolver LFoot, RFoot;
    public float handThrust;
    public Rigidbody rH, lH;
    public float bodyMass;
    public float gettingUpTime;
    public float balanceForce;
    public float maxComDistance;
    public float stableVelocityMag;
    public bool stun;
    Rigidbody myRGB,head;
    float headHeight;
    bool gettingUp,recovering;

    TimeManager t;
   

    private void Start()
    {
        LFoot.ikActive = true;
        RFoot.ikActive = true;
        gettingUp = false;
        headHeight = Vector3.Dot(Head.transform.position - GetFootCentre(), Vector3.up);
        myRGB = GetComponent<Rigidbody>();
        head = Head.GetComponent<Rigidbody>();
        t=FindObjectOfType<TimeManager>();
    }

    private void FixedUpdate()
    {
        if (t.timeStopped)
        {
            return;
        }
        Vector3 footCentre = (LFoot.transform.position + RFoot.transform.position) / 2;
        Vector3 comDir = new Vector3(footCentre.x - head.position.x, 0, footCentre.z - head.position.z);

        if (Grounded())
        {
            if (recovering)
            {
                lH.AddForce(Vector3.down * handThrust);
                rH.AddForce(Vector3.down * handThrust);
            }
            if (!stun || gettingUp)
            {
                if (Mathf.Abs(Vector3.Dot(Head.transform.position - GetFootCentre(), Vector3.up)) > headHeight * 0.85f && comDir.magnitude < maxComDistance / 2)
                {
                   if (head.velocity.magnitude <= stableVelocityMag  && Vector3.Angle(myRGB.transform.up, Vector3.up) <= 60)
                    {
                        gettingUp = false;
                    }
                    
                    head.AddForce(-Physics.gravity * bodyMass);
                    head.AddForce(new Vector3(footCentre.x - head.position.x, 0, footCentre.z - head.position.z) * balanceForce);

                }
                else
                {
                    if (gettingUp)
                    {
                        if (recovering)
                        {
                            head.AddForce(-Physics.gravity * bodyMass / 2);
                        }
                        else
                        {
                            head.AddForce(-Physics.gravity * bodyMass);
                        }
                    }
                }
                

            }
            if (stun)
            {

                if (!recovering)
                {
                    StartCoroutine(StartGettingUp());
                }

            }
            Debug.Log(myRGB.velocity.magnitude);
            if (Vector3.Angle(myRGB.transform.up, Vector3.up) > 70 && !gettingUp)
            {              
                    stun = true;                
            }

        }
        LFoot.ikActive = RFoot.ikActive = !stun && !gettingUp;
    }

    Vector3 GetFootCentre()
    {
        return (LFoot.transform.position + RFoot.transform.position) / 2; 
    }
    bool Grounded() {
        return LFoot.CanPlaceFoot() && RFoot.CanPlaceFoot();
    }

    IEnumerator StartGettingUp()
    {
        
        recovering = true;        
        yield return new WaitForSeconds(gettingUpTime);
        
        gettingUp = true;
        yield return new WaitForSeconds(1f);
        stun = false;
        recovering = false;
    }

    

    

}
