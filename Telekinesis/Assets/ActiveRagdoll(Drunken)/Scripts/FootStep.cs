using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour
{
    public float StepHeight, StepTime;
    public float maxStepDis,minStepDis;
    public AnimationCurve StepAnim;

    
    public bool Stepping;

    Vector3 lerpPos;
    Vector3 startPos, endPos;
    float steppingTime;
    void Start()
    {
        Stepping = false;
        steppingTime = 0;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Stepping)
        {
            Step();
        }        
    }

    public void stickLeg(Vector3  pos)
    {
        transform.position = pos;
    }

    public void MoveLeg(Vector3 from,Vector3 to)
    {
        startPos = from;
        endPos = startPos+Vector3.ClampMagnitude((to-startPos),maxStepDis);
        if (Vector3.Distance(startPos, endPos) > minStepDis)
        {
            Stepping = true;
        }
    }

    void Step()
    {
        lerpPos = Vector3.Lerp(startPos, endPos, steppingTime/StepTime);
        transform.position = lerpPos + Vector3.up * StepAnim.Evaluate(steppingTime/StepTime)*StepHeight ;
        if (steppingTime < StepTime)
        {
            steppingTime += Time.deltaTime;
        }
        else
        {
            Stepping = false;
            steppingTime = 0;

        }




    }
}
