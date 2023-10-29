using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] GameObject cam;

    float speed;
    CharacterController body;
    Vector3 moveDir, camRot;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        body= GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        GetMoveDirection();
        CameraMovement();
        Movement();
    }

    void GetMoveDirection()
    {
        if (body.isGrounded)
        {
            
        }

    }

    void CameraMovement()
    {
        camRot.x -= Input.GetAxis("Mouse Y");
        camRot.x = Mathf.Clamp(camRot.x, -80, 80);
        transform.Rotate(0, Input.GetAxisRaw("Mouse X"), 0);

        cam.transform.localEulerAngles= camRot;
    }

    void Movement()
    {
        
        if (!body.isGrounded)
        {            
            moveDir.y += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            moveDir = cam.transform.forward * Input.GetAxisRaw("Vertical") + cam.transform.right * Input.GetAxisRaw("Horizontal");
            moveDir.y = 0;
            moveDir.Normalize();

            speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;
            moveDir *= speed;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDir.y=jumpSpeed;
            }
            else
            {
                moveDir.y = 0;
            }
        }
        body.Move(moveDir*Time.deltaTime);

    }
}
