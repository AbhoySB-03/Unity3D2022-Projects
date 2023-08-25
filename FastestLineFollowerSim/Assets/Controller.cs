using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Controller : MonoBehaviour
{
    [SerializeField] private float fastSpeed, slowSpeed;
    [SerializeField] private float Kp,Kd;
    [SerializeField] private float maxWaitTime;
    [SerializeField] private int middleValue;
    [SerializeField] private float errorTolerance;
    [SerializeField] private ArticulationBody lWheel, rWheel;
    [SerializeField] private IRArray front, back;

    float leftVel, rightVel;
    private float waitTime;
    private float error, lastError;
    private float forwardSpeed;
    // Start is called before the first frame update
    void Start()
    {
        lastError = 0;
        leftVel=fastSpeed; rightVel=fastSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {
        forwardSpeed = fastSpeed;
        error = CalculateError(back);
        Debug.Log(error);
        if (Mathf.Abs(error) < errorTolerance)
        {
            leftVel = rightVel = forwardSpeed;
        }
        else if (Mathf.Abs(CalculateError(front)) < error)
        {
            leftVel = rightVel = forwardSpeed;
        }
        else
        {
            leftVel = forwardSpeed - error * Kp - (error - lastError) * Kd/Time.deltaTime;
            rightVel = forwardSpeed + error * Kp + (error - lastError) * Kd/Time.deltaTime;


        }
        

        lastError = error;
    }


    void FixedUpdate()
    {
        lWheel.yDrive = SetVelocity(lWheel, leftVel);
        rWheel.yDrive = SetVelocity(rWheel, rightVel);

    }
    ArticulationDrive SetVelocity(ArticulationBody wheel,float vel)
    {
        ArticulationDrive xD=wheel.yDrive;
        xD.targetVelocity = vel;
        xD.damping = 10000;
        return xD;
    }

    float CalculateError(IRArray sensorArray)
    {
        return sensorArray.readLine() - middleValue;
    }
}
