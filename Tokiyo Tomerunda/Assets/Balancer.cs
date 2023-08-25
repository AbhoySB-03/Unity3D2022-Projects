using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balancer : MonoBehaviour
{
    public Rigidbody head, footl, footr, thighl,thighr, kneel, kneer;
    public float unbalanceAngle;
    public float hforce, fforce;
    public float getupTime;
    public float maxBalanceDirMagn, maximumLegDistance;

    TimeManager t;
    float fallRecovery;
    bool stun, recovering;
    // Start is called before the first frame update
    void Start()
    {
        t = FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (t.timeStopped) return;
        
        if (Vector3.Angle(transform.position-(footl.transform.position+footr.transform.position)/2, Vector3.up) < unbalanceAngle)
        {           
            head.AddForce(Vector3.up * hforce);
            footl.AddForce(Vector3.down * fforce);
            footr.AddForce(Vector3.down * fforce);
        }
        else
        {
            Vector3 balanceDir = Vector3.ProjectOnPlane(transform.position - (footl.transform.position + footr.transform.position) / 2, Vector3.up);
            
            
                head.AddForce(Vector3.up * hforce);
                if (Vector3.Angle(balanceDir, -transform.up) < 90)
                {
                    
                thighr.AddTorque(transform.right * fforce*4);
                kneer.AddTorque(transform.right * -fforce*4);
                footr.AddForce(balanceDir * fforce);
            }
                else
                {
                   
                thighl.AddTorque(transform.right * fforce * 4);
                kneel.AddTorque(transform.right * -fforce * 4);
                footl.AddForce(balanceDir * fforce);
            }
            
        }
    }
}
