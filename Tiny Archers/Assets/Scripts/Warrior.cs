using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Warrior : Character
{
    [SerializeField] private float shieldHealth;
    [SerializeField] private GameObject handShield;
    [SerializeField] protected float attackCoolDownTime;
    [SerializeField] protected Transform hitEmitter;
    [SerializeField] protected float hitRange=1f;
    [SerializeField] protected float swordHitDamage=15;
    [SerializeField] protected LayerMask enemy, ally;
    [SerializeField] protected AudioClip slashSound, shieldHitSound;
    [SerializeField] protected AudioClip battlecry;

    protected GameObject target;
    protected bool shielding;
    protected float attackCoolDown;
    
    protected bool idleState
    {
        get
        {
            return anim.GetCurrentAnimatorStateInfo(1).IsName("Idle");
        }
    }
    protected bool canAttack
    {
        get
        {
            return attackCoolDown <= 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        shielding= false;
        attackCoolDown= 0;
    }
    
    protected void Attack()
    {
        int attackType = Random.Range(0, 2);
        anim.SetBool("Block", false);
        if (attackType==0 )
        {
            anim.SetTrigger("Attack1");
        }
        else
        {
            anim.SetTrigger("Attack2");
        }
        
    }

    void SwordHit(int _)
    {
        if (!Physics.Raycast(hitEmitter.transform.position, hitEmitter.transform.forward, out RaycastHit hit, hitRange, enemy))
            return;
        if (hit.collider.gameObject.GetComponent<Character>())
        {
            hit.collider.gameObject.SendMessage("TakeDamage", swordHitDamage);
        }
    }

    protected override void Animate()
    {
        base.Animate();
        anim.SetBool("Block", shielding && (shieldHealth> 0));
        handShield.SetActive(shieldHealth> 0);
    }

    protected void LockTarget(GameObject t) {
        target = t;
        nav.angularSpeed= 0;
        shielding= true;
    }

    protected void UnlockTarget()
    {
        target = null;
        nav.angularSpeed= 120;
        shielding= false;
    }

    public override void TakeDamage(float Damage)
    {
        if (shieldHealth > 0 && shielding)
        {
            audioSource.clip=shieldHitSound; audioSource.Play();
            shieldHealth -= Damage;
            anim.SetTrigger("Hit");
            return;
        }
        base.TakeDamage(Damage);
    }

    void Slash(int _)
    {
        audioSource.clip=slashSound; audioSource.Play();
    }
}
