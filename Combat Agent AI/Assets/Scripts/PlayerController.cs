using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Animator anim;
    NavMeshAgent nav;
    SwordShieldCombt com;
    public GameObject CamPivot,Cam,MyEye;
    public float TP_CamOffset, FP_CamHeight;
    bool FirstPerson=false;
    Vector3 camrot=Vector3.zero;
    float YRot;
    Vector3 lookat;
    void Start()
    {
        com = GetComponentInChildren<SwordShieldCombt>();
        nav = GetComponent<NavMeshAgent>();
        nav.angularSpeed = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        nav.velocity = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * nav.speed;

        lookat =CamPivot.transform.forward;
        lookat.y = 0;
        lookat.Normalize();

        ChangeCam();
        setcam();

        if (Input.GetMouseButton(1))
        {
            com.block(true);
        }
        else
        {
            com.block(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            com.Attack();
        }

    }

    void setcam()
    {
        
        if (FirstPerson)
        {
            CamPivot.transform.position =MyEye.transform.position;
            Cam.transform.position = CamPivot.transform.position;
           
        }
        else
        {
            CamPivot.transform.position = transform.position +transform.up * FP_CamHeight/2;
            Cam.transform.position =CamPivot.transform.position -CamPivot.transform.forward * TP_CamOffset;
            
        }
        camrot.x = Mathf.Clamp(camrot.x - Input.GetAxis("Mouse Y"), -80, 80);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookat), 0.5f);
        CamPivot.transform.eulerAngles = camrot;
        
        camrot.y += Input.GetAxis("Mouse X");

        

    }


    public void Attack()
    {
        if (CanAttack())
        {
            anim.SetTrigger("Attack");
        }
        else
        {
            anim.ResetTrigger("Attack");
        }

    }

    void ChangeCam()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FirstPerson = !FirstPerson;
        }
    }
    public bool CanAttack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            return true;
        }
        return false;
    }
}
