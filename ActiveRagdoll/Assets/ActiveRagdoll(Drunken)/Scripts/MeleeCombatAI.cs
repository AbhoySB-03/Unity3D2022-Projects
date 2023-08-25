using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeleeCombatAI : MonoBehaviour
{
    public Rigidbody meleeWeapon;
    public float attackCooldown;
    public GameObject target;
    public float turningTorque;
    public TargetPositioning tar;
    public Vector3 targetDir;
    public float attackRange;
    public float hitForce;
    public float moveSpeed;
    public Rigidbody arm;
    
    
    bool attacking,canAttack;
    Rigidbody myRB;
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        canAttack = true;
        attacking = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LookAtTarget();
        Attack();
    }

    void LookAtTarget()
    {
        targetDir = target.transform.position - transform.position;
        myRB.AddTorque(Vector3.Cross(transform.forward, Vector3.ProjectOnPlane(targetDir, Vector3.up)) * turningTorque);
        myRB.AddTorque(Vector3.Cross(transform.up, Vector3.up) * turningTorque);
    }

    void Attack()
    {
        if (Vector3.Distance(target.transform.position,transform.position) <= attackRange)
        {
            if (Vector3.Distance(target.transform.position, transform.position) <= attackRange * 2 / 3)  
            {

                tar.desVelocity = -Vector3.ProjectOnPlane((target.transform.position - transform.position).normalized, Vector3.up) * moveSpeed;
            }
            else
            {
                tar.desVelocity = Vector3.zero;
            }
            if (canAttack)
            {
                StartCoroutine(Hit());
            }
            
        }
        else
        {
            tar.desVelocity = Vector3.ProjectOnPlane((target.transform.position - transform.position).normalized, Vector3.up)*moveSpeed;
        }
    }

    IEnumerator Hit()
    {
        attacking = true;
        canAttack = false;
        Vector3 dir = targetDir.normalized;
        meleeWeapon.AddForce(transform.up * hitForce / 2+dir*hitForce/4);
        yield return new WaitForSeconds(0.2f);
        /*
        meleeWeapon.AddForce(- dir * hitForce / 4);
        yield return new WaitForSeconds(0.4f);*/
        dir = Vector3.ProjectOnPlane((target.transform.position - transform.position).normalized, Vector3.up);
        meleeWeapon.AddForce(dir * hitForce);
        arm.AddForce(-dir * hitForce / 2);
        yield return new WaitForSeconds(1);
        
        StartCoroutine(AttackCooldown());
        attacking = false;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
