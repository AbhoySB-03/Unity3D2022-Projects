using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    
    public GameObject bow;
    public GameObject Pivot;
    public GameObject My_arrow;
    public GameObject arrow;
    public GameObject emitter;
    public float shot_force;

    CharacterController mychar;
    Animator anim;
    GameObject temp_arr;
    float zs=0;
    float xs=0;
    Vector3 moveVect;
    float rotx, roty;
    public float rotspeed;
    float g=9.8f * 0.01f;
    float ys;
    float angle;
    float speed;
    bool jumping;
    bool crouching;



    // Start is called before the first frame update
    void Start()
    {
        mychar = this.GetComponent<CharacterController>();
        anim = bow.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift) && mychar.isGrounded)
            {
                zs = 5;
                speed = 3;
            }
            else
            {

                if (crouching)
                {
                    speed = 0.5f;
                    zs = 1;
                }
                else
                {
                    speed = 1;
                    zs = 2;
                }
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (crouching)
            {
                zs = -1;
                speed = 0.5f;
            }
            else
            {
                zs = -2;
                speed = 1;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (crouching)
            {
                speed = 0.5f;
                xs = -1;
            }
            else
            {
                xs = -2;
                speed = 1;
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (crouching)
            {
                xs = 1;
                speed = 0.5f;
            }
            else
            {
                xs = 2;
                speed = 1;
            }
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            zs = 0;
            speed = 0;
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            xs = 0;
            speed=0;
        }

        if (mychar.isGrounded)
        {
            Pivot.GetComponent<Animator>().SetFloat("speed", speed);
        }


        if (Input.GetKeyDown(KeyCode.Space) && mychar.isGrounded )
        {
            ys = 5;
            jumping=true;
        }
        if (speed==0){
            if (crouching){
                speed=0.5f;
            }
            else{
                speed=0;
            }
        }

        if (Input.GetKeyDown(KeyCode.C) && mychar.isGrounded)
        {
            if (!crouching)
            {
                crouching = true;
            }
            else
            {
                crouching = false;
            }

        }
       

       
        
        if (mychar.isGrounded)
        {
           
        }
        else
        {
            ys -= g; 
        }

        moveVect = new Vector3(xs, ys, zs) * Time.deltaTime;
        moveVect = this.transform.TransformDirection(moveVect);

        mychar.Move(moveVect);

       
        roty = Input.GetAxis("Mouse X") * rotspeed;
        rotx = Input.GetAxis("Mouse Y") * rotspeed;

        
        this.transform.Rotate(0, roty * Time.deltaTime, 0);



        angle += rotx * Time.deltaTime;

        if (angle <= -35) angle = -35;
        if (angle >= 35) angle = 35;


        Pivot.GetComponent<Animator>().SetFloat("angle", angle);


        
                

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Charge");
        }
        if (Input.GetMouseButton(0))
        {
            anim.SetBool("Aiming", true);
        }
        else
        {
            anim.SetBool("Aiming", false);
        }

        if (Input.GetMouseButtonUp(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("aim"))
        {
            
            anim.SetTrigger("Shoot");
            StartCoroutine(shoot());
        }
            
        

              
    }

    IEnumerator shoot()
    {
        shot();
        My_arrow.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        My_arrow.SetActive(true);


    }

    void shot()
    {
        temp_arr = Instantiate(arrow, emitter.transform.position, emitter.transform.rotation);
        temp_arr.GetComponent<Rigidbody>().AddForce(emitter.transform.forward * shot_force);

        Destroy(temp_arr, 8);
    }







}
