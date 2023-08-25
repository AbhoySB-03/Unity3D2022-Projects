using UnityEngine;

public class ThorCombatScript : MonoBehaviour
{
    [SerializeField] private float throwSpeed;
    [SerializeField] private string IdleStateName="Armature|Idle";
    [SerializeField] private Mjolnir hammer;
    [SerializeField] private MjolnirCharged mjolnirCharged;

    private Animator anim;
    private float attackButtonTime, secondAttackTime;
    private bool haveHammer, throwHammerAim, block, canAttack;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        haveHammer = true;
    }

    // Update is called once per frame
    void Update()
    {
        combat();
    }

    private void combat()
    {
        canAttack = anim.GetCurrentAnimatorStateInfo(0).IsName(IdleStateName);
        if (secondAttackTime>0)
            secondAttackTime-=Time.deltaTime;
        if (throwHammerAim)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                throwHammerAim = false;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                anim.SetTrigger("Throw");
                throwHammerAim = false;
            }
        }
        anim.SetBool("ThrowingHammer", throwHammerAim);
        

        if (!haveHammer)
        {
            if (Input.GetMouseButtonDown(1) && !anim.GetBool("CallingHammer"))
            {
                anim.SetBool("CallingHammer", true);
                hammer.Call();
            }
            if (Input.GetMouseButtonDown(0) && canAttack)
            {
                anim.SetTrigger("AttackH");
            }
            if (anim.GetBool("CallingHammer") && hammer.state==Mjolnir.MjolnirState.Held)
            {
                anim.SetTrigger("Catch");
                anim.SetBool("CallingHammer", false);
                haveHammer= true;
                
            }           
            return;
        }

        if (!mjolnirCharged.charged|| (mjolnirCharged.charged && (!Input.GetMouseButton(0) || !Input.GetMouseButton(1)))){
            mjolnirCharged.StopArcAttacking();
            anim.SetBool("ArcAttacking", false);
        } 
        
        if (!canAttack)
            return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            anim.SetTrigger("Block");
        }
        if (Input.GetMouseButton(0))
        {
            if (mjolnirCharged.charged && Input.GetMouseButton(1))
            {
                anim.SetBool("ArcAttacking", true);
            }
            else
            {
                anim.SetBool("ArcAttacking", false);
                attackButtonTime += Time.deltaTime;
            }
            
        }

        if (attackButtonTime > 0.5f)
        {
            anim.SetTrigger("PowerAttack");
            attackButtonTime = 0f;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (secondAttackTime > 0)
            {
                anim.SetTrigger("Attack2");
                secondAttackTime= 0f;
            }
            else
            {
                anim.SetTrigger("Attack1");
                secondAttackTime = 1.5f;
            }
            attackButtonTime= 0f;

        }
        
        if (Input.GetMouseButtonDown(1))
        {
            throwHammerAim = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Charge");
        }
        
    }

    void ThrowHammer(int i)
    {
        hammer.Throw(transform.forward);
        haveHammer= false;

    }

    void ChargeHammer(int i)
    {
        if (!haveHammer)
            return;

        mjolnirCharged.ChargeHammer();
    }


    void ArcAttack(int i)
    {
        mjolnirCharged.StartArcAttack();
    }
    
}
