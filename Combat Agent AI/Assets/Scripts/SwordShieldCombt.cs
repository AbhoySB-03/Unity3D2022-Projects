﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordShieldCombt : MonoBehaviour
{
    public float CombatRange, AttackRange;
    public float  AttackCost, BlockCost,regen_rate;
    public HealthScript h;

    public Transform hitpoint;
    public LayerMask hitlayer;

    public bool blocking=false;
    
    Animator anim;
    void Start()
    {
        
        anim = transform.GetComponentInChildren<Animator>();
    }

    
    void FixedUpdate()
    {
        h.combatstamina = Mathf.Clamp(h.combatstamina, 0, h.MaxCombatStamina);
        
        if (blocking)
        {
            h.combatstamina -= Time.deltaTime * BlockCost;
        }

        anim.SetBool("Blocking", blocking);
    }

    void regenstamina()
    {
        if (h.combatstamina < h.MaxCombatStamina / 4)
        {
            h.combatstamina += regen_rate;
        }
        else
        {
            h.combatstamina += regen_rate/4;
        }
    }

    public void block(bool t)
    {
        if (t)
        {
            if (CanBlock())
            {
                blocking = true;
            }
            else
            {
                blocking = false;
            }
        }
        else
        {
            blocking = false;
        }

        

    }

    public void Attack()
    {
        if (CanAttack())
        {
            if (blocking)
            {
                blocking = false;
                
            }
            anim.SetTrigger("Attack");
            
        }
        else
        {
            anim.ResetTrigger("Attack");
        }
        

    }

    public bool CanAttack()
    {
        if (h.combatstamina>AttackCost && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            return true;
        }
        return false;
    }

    public bool CanBlock()
    {
        if (h.combatstamina >= h.MaxCombatStamina / 2)
        {
            return true;
        }
        return false;

    }


    public void hit(float m)
    {
        
        h.combatstamina -= AttackCost;
        RaycastHit hitt;
        if (Physics.Raycast(hitpoint.position, hitpoint.forward,out hitt, 2))
        {
            if (hitt.collider.transform.root.gameObject.tag == "Enemy")
            {
                if (hitt.collider.tag != "Shield")
                {
                    h.combatstamina = h.MaxCombatStamina;
                    hitt.collider.transform.root.gameObject.GetComponent<HealthScript>().TakeDamage(10);
                    hitpoint.GetComponent<AudioSource>().Play();
                }
                else
                {
                    hitt.collider.GetComponent<AudioSource>().Play();
                }
            }
           
        }
        
    }

    
}
