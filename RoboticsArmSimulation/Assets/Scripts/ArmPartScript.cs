using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmPartScript : MonoBehaviour
{
    [SerializeField] private Vector3 disassembleDirection;
    [SerializeField] private float disassembleDistance;
    [SerializeField] private float assembleTime=0.25f;
    private Vector3 assembledPos;
    private Vector3 disassembledPos;
    private ArticulationBody body;
    private ArticulationDrive drive;

    private float startAngle, currAngle;
    public enum AssemblyState
    {
        Working , Assembling, Disassembling, Disassembled
    }
    [SerializeField, Range(0f, 1f)]
    private float disassmblePerc = 0;
    public AssemblyState state;

    // Start is called before the first frame update
    void Awake()
    {
        state= AssemblyState.Working;
        assembledPos=transform.position-transform.parent.position;
        disassembledPos = assembledPos + (transform.right * disassembleDirection.x + transform.up * disassembleDirection.y + transform.forward * disassembleDirection.z).normalized * disassembleDistance;
        body= GetComponent<ArticulationBody>();
        drive = body.xDrive;
        startAngle = GetRotation();
        currAngle=startAngle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case AssemblyState.Assembling:
                Assembling();
                break;
            case AssemblyState.Disassembling:
                Disassembling();
                break;
        }       
        
    }

    public void Disassemble()
    {
        if (body)
        {
            startAngle = currAngle;
            body.enabled = false;
        }
        if (state != AssemblyState.Working)
            return;
        state = AssemblyState.Disassembling;
        assembledPos=transform.position-transform.parent.position;
        disassembledPos = assembledPos + (transform.right * disassembleDirection.x + transform.up * disassembleDirection.y + transform.forward * disassembleDirection.z).normalized * disassembleDistance;
        
    }

    public void Assemble()
    {
        if (state != AssemblyState.Disassembled)
            return;
        state = AssemblyState.Assembling;        
    }

    public void SetRotation(float value)
    {
        if (state!=AssemblyState.Working)
            return;
        currAngle = value;
        drive.target = value-startAngle;
        body.xDrive = drive;
        
    }

    public float GetRotation()
    {
        return body.xDrive.target;
    }
    void Assembling()
    {
        if (disassmblePerc > 0)
            disassmblePerc -= Time.deltaTime / assembleTime;
        else
        {
            state = AssemblyState.Working;
            if (body)
            {
                body.enabled = true;
                SetRotation(currAngle);
            }
        }
        
            disassmblePerc = Mathf.Clamp01(disassmblePerc);
        transform.position = Vector3.Lerp(transform.parent.position + assembledPos, transform.parent.position + disassembledPos, disassmblePerc);
    }

    void Disassembling()
    {
        if (disassmblePerc < 1)
            disassmblePerc += Time.deltaTime / assembleTime;
        else
            state = AssemblyState.Disassembled;
        disassmblePerc = Mathf.Clamp01(disassmblePerc);
        transform.position = Vector3.Lerp(transform.parent.position + assembledPos, transform.parent.position + disassembledPos, disassmblePerc);
    }

}
