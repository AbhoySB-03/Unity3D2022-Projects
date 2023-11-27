using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform followPoint;
    [SerializeField] private float maxCamRot=80, minCamRot=-80;
    [SerializeField] private float camSpeed;

    float camRot;
    void Start()
    {
        camRot = 0;   
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        camRot=Mathf.Clamp(camRot-Input.GetAxisRaw("Mouse Y")*camSpeed*Time.deltaTime, minCamRot, maxCamRot);
        _camera.localEulerAngles = new Vector3(camRot, 0, 0);
    }

    private void LateUpdate()
    {
        transform.position=Vector3.Lerp(transform.position, followPoint.position, Time.deltaTime*40);
        transform.rotation = Quaternion.Slerp(transform.rotation, followPoint.rotation, Time.deltaTime * 40);
    }
}
