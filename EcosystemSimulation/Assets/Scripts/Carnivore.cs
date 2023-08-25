using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Carnivore : Animal
{
    [Header("Parameters for Carnivore")]
    public LayerMask food;
    public LayerMask animals;
    [SerializeField] private float  huntSpeed;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackDamage;
   

    private bool attacking;
    private Meat curFood;
    private Hervivores[] preys;
    private Carnivore[] fellows;
    
    public enum AI_State
    {
        wandering, finding_food, hunting

    }

    [HideInInspector] public AI_State myState;
    new void Start()
    {
        base.Start();
        myState = AI_State.wandering;
      
    }

    new void Update()
    {
        eating = false;   
        base.Update();
        SenseAnimals();
        if (myState == AI_State.finding_food)
        {
            agent.speed = moveSpeed;
            attacking = false;
            Meat f = GetFood();
            if (f)
            {
                eating = false;
                agent.stoppingDistance = bodyLength / 2;
                agent.SetDestination(f.transform.position);
                if (Vector3.Distance(transform.position, f.transform.position) <= bodyLength/2)
                {
                    curFood = f;
                    agent.velocity = Vector3.zero;                    
                    eating = true;
                }
            }
            else
            {
                agent.stoppingDistance = 0;
                if (FoundPrey())
                {
                    myState = AI_State.hunting;
                }

                Wander();
            }
            if (!hungry)
            {
                eating = false;
                myState = AI_State.wandering;
            }
        }        
        else
        {
            eating = false;
            agent.stoppingDistance = 0;
            if (myState == AI_State.wandering)
            {
                agent.speed = moveSpeed;
                attacking = false;
                if (FoundPrey())
                {
                    myState = AI_State.hunting;
                }
                if (hungry)
                {
                    myState = AI_State.finding_food;
                }
               

                Wander();
            }
            else if (myState == AI_State.hunting)
            {
                agent.speed = huntSpeed;
                if (hungry && GetFood())
                {
                    myState = AI_State.finding_food;
                }
                if (preys.Length == 0)
                {
                    myState = AI_State.wandering;
                    attacking = false;
                }
                else
                {
                    Hervivores prey = TargetPrey();
                    if (Vector3.Distance(prey.transform.position, transform.position) < attackDistance)
                    {

                        attacking = true;
                    }
                    else
                    {

                        attacking = false;
                        agent.SetDestination(prey.transform.position);
                    }
                }

            }

        }
    }

    new void LateUpdate()
    {
        base.LateUpdate();
        if (!eating)
        {
            curFood = null;
        }
    }

    protected override void Eat(int i)
    {
        anim.SetFloat("FoodValue", curFood.Consume());
        base.Eat(i);
       
    }
    private Meat GetFood()
    {
        Meat[] a = (from b in Physics.OverlapSphere(transform.position, farDetectionRange, food)
                     where (b.GetComponent<Meat>() && b.GetComponent<Meat>().IsConsumable())
                     select b.GetComponent<Meat>()).ToArray();
        if (a.Length == 0)
            return null;
        Meat min = a[0];
        for (int i = 1; i < a.Length; i++)
        {
            if (Vector3.Distance(transform.position, a[i].transform.position) < Vector3.Distance(transform.position, min.transform.position)) min = a[i];
        }
        return min;
    }

    private void SenseAnimals()
    {
        fellows = (from b in Physics.OverlapSphere(transform.position, farDetectionRange, animals)
                   where (b.GetComponent<Carnivore>() )
                   select b.GetComponent<Carnivore>()).ToArray();

        preys = (from b in Physics.OverlapSphere(transform.position, farDetectionRange, animals)
                   where (b.GetComponent<Hervivores>() && InSight(b))
                   select b.GetComponent<Hervivores>()).ToArray();

    }


    protected override void Animate()
    {
        base.Animate();
        anim.SetBool("Attacking", attacking);
    }
    private Hervivores TargetPrey()
    {
        if (preys.Length == 0)
            return null;
        Hervivores tPrey;
        tPrey = preys[0];
        for (int i=1; i < preys.Length; i++)
        {
            if (Vector3.Distance(transform.position, preys[i].transform.position) < Vector3.Distance(transform.position, tPrey.transform.position)) tPrey = preys[i];
            
        }
        return tPrey;
    }

    private bool FoundPrey()
    {        
        return preys.Length > 0;
    }


    void Attack(int i)
    {
        Hervivores t = TargetPrey();
        if (!t)
            return;
        
        if (Vector3.Angle(transform.forward, t.transform.position - transform.position) < 180)
        {
            t.TakeDamage(attackDamage);
        }
    }
    

}
