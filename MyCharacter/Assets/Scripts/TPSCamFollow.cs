using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCamFollow : MonoBehaviour
{
    public Transform Target,Cam;
    public float RotSpeed;
    public Vector3 CamOffset;
    public LayerMask notMe;

    Vector3 desPosition;
    Vector3 CamRot = Vector3.zero;
    RaycastHit camHit;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CamRot.y += Input.GetAxis("Mouse X")*RotSpeed*Time.deltaTime;
        CamRot.x = Mathf.Clamp(CamRot.x - Input.GetAxis("Mouse Y")*RotSpeed*Time.deltaTime, -45, 60);
        transform.position = Target.transform.position;
        transform.eulerAngles = CamRot;
        if (Physics.Raycast(transform.position,Cam.position-transform.position,out camHit, CamOffset.magnitude,notMe))
        {
            Cam.position = Vector3.Lerp(Cam.position,camHit.point+(transform.position-Cam.position).normalized*1,0.2f);
        }
        else
        {
            Cam.localPosition = Vector3.Lerp(Cam.localPosition,CamOffset,0.2f);
        }
    }
}
