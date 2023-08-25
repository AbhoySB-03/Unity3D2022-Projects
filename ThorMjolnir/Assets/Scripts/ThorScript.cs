using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThorScript : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField] private float runSpeed, sprintSpeed, jumpSpeed;
    [SerializeField] private float camRotSpeed;
    [SerializeField] private GameObject mainBody;
    [SerializeField] private float minCamRotX, maxCamRotX;
    private Animator anim;
    private Vector3 moveDir;
    private Vector3 velocity;
    private float camRotX;
    private float yVel;
    private CharacterController myBody;
    private bool jump, sprint;
    
    void Start()
    {
        anim=mainBody.GetComponent<Animator>();
        myBody=GetComponent<CharacterController>();   
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        Gravity();
        Movement();
    }

    private void LateUpdate()
    {
        CameraMovement();
    }


    private void GetInputs()
    {
        moveDir=mainBody.transform.forward*Input.GetAxis("Vertical")+mainBody.transform.right*Input.GetAxis("Horizontal");
        sprint = Input.GetKey(KeyCode.LeftShift);
        jump= Input.GetKey(KeyCode.Space);
        camRotX -= Input.GetAxisRaw("Mouse Y")*camRotSpeed*Time.deltaTime;
        camRotX = Mathf.Clamp(camRotX, minCamRotX, maxCamRotX);
    }

    private void Movement()
    {
        float speed = sprint ? sprintSpeed : runSpeed;
        
        if (myBody.isGrounded)
        {
            if (jump)
            {
                yVel = jumpSpeed;
            }
            velocity = speed * moveDir;
            
        }
        myBody.Move((velocity + Vector3.up * yVel) * Time.deltaTime);

    }
    private void Gravity()
    {
        if (!myBody.isGrounded) {
            yVel += Physics.gravity.y*Time.deltaTime;
        }
    }

    private void CameraMovement()
    {
        mainBody.transform.localEulerAngles = new Vector3(camRotX, 0, 0);
        transform.Rotate(0, Input.GetAxis("Mouse X") * camRotSpeed * Time.deltaTime, 0);

    }


}
