using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Animal : MonoBehaviour
{
   
    [Header("Basic Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxStamina;
    [SerializeField] private float healthRecRate;
    [SerializeField] private float staminaRecRate;
    [SerializeField] private float hungerRate;
    [SerializeField] protected float moveSpeed;
    [Header("Detection Parameters")]
    [SerializeField] private Transform eye;
    [SerializeField] protected float farDetectionRange, closeDetectionRange, FOV;
    [Header("Wander Parameters")]
    [SerializeField] protected float maxWanderRange, maxWanderWaitTime, minWanderWaitTime;
    [Header("Body Allignment with Ground Parameters")]
    [SerializeField] private LayerMask ground;
    [SerializeField] protected float bodyLength, eatRange;
    [SerializeField] private float deathFoodValue;
    protected float health;
    protected float hunger;
    protected float stamina;
    protected NavMeshAgent agent;
    protected bool eating, hungry;
    protected Animator anim;

    private bool waiting, moving;


    protected void Start()
    {
        health = maxHealth;
        stamina = maxStamina;
        hunger = 0;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        waiting = false;
        moving = false;
        anim = GetComponent<Animator>();
    }

    protected void Update()
    {
        if (IsDead())
        {
            agent.isStopped = true;
        }
        if (agent.velocity.sqrMagnitude==0)
        stamina = Mathf.Clamp(stamina + Time.deltaTime * staminaRecRate, 0, maxStamina);
        ConsumeStamina(agent.velocity.magnitude*Time.deltaTime);
        if (!hungry && hunger >50)
        {
            hungry = true;
        }
        if (hungry && hunger <= 0)
        {
            hungry = false;
        }
    }

    protected void LateUpdate()
    {
        if (!eating)
            hunger = Mathf.Clamp(hunger + Time.deltaTime * hungerRate * (stamina > maxStamina / 2 ? 1 : 2), 0, 100);
        Animate();
        BodyAlignment();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead())
            return;
        health -= damage;
    }

    protected void ConsumeStamina(float staminaConsumption)
    {
        stamina -= staminaConsumption;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
    }

    protected virtual void Eat(int i)
    {
        if (IsDead())
            return;
        health = Mathf.Clamp(health + anim.GetFloat("FoodValue"), 0, maxHealth);
        hunger -= anim.GetFloat("FoodValue");
    }

    public bool IsDead()
    {
        return health <= 0;
    }

   
    protected virtual void Animate()
    {
        if (!anim)
            return;
        anim.SetFloat("Speed", agent.velocity.magnitude);
        anim.SetBool("Eating", eating);
        anim.SetBool("Dead", IsDead());
    }

    protected bool InSight(Collider c)
    {
        if (Vector3.Distance(c.transform.position, transform.position) < closeDetectionRange)
            return true;
        if (Vector3.Distance(c.transform.position, transform.position) < farDetectionRange)
        {
            if (Vector3.Angle(eye.forward, c.transform.position - eye.transform.position) <= FOV / 2)
            {
                if (Physics.Raycast(eye.transform.position, c.transform.position - eye.transform.position, out RaycastHit hit, farDetectionRange))
                {
                    return hit.collider.gameObject.transform.root.gameObject == c.transform.root.gameObject;
                }
            }
        }
        return false;
    }

    protected void Wander()
    {
        if (!moving && !waiting)
        {
            StartCoroutine(Move());
        }

        
    }

    private IEnumerator Move()
    {
        moving = true;
        agent.SetDestination(transform.position + new Vector3(Random.Range(-maxWanderRange, maxWanderRange), 0, Random.Range(-maxWanderRange, maxWanderRange)));
        yield return new WaitForSeconds(0.05f);
        yield return new WaitUntil(() => (agent.remainingDistance <= agent.stoppingDistance));
        moving = false;
        waiting = true;
        yield return new WaitForSeconds(Random.Range(minWanderWaitTime, maxWanderWaitTime));
        waiting = false;
    }

    private void BodyAlignment()
    {
        Physics.Raycast(transform.position + transform.forward*bodyLength/2+transform.up, Vector3.down, out RaycastHit hf, 5,ground);
        Physics.Raycast(transform.position - transform.forward*bodyLength /2+transform.up, Vector3.down, out RaycastHit hr, 5, ground);
        Vector3 forward = hf.point - hr.point;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward, Vector3.ProjectOnPlane(Vector3.up, forward)),0.5f);
    }

    protected virtual void Die(int i)
    {
        gameObject.layer = LayerMask.NameToLayer("Food");
        Meat m = gameObject.AddComponent<Meat>();
        m.maxFoodValue = deathFoodValue;
        m.decomposeRate = 1;       
        Destroy(anim);
        
    }
}
