using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Hervivores : Animal
{
    [Header("Parameters for Hervivore")]
    public LayerMask food;
    public LayerMask animals;
    [SerializeField] private float fleeDistance, fleeSpeed;
    private Hervivores[] fellows;
    private Carnivore[] enemies;
    private int no_of_Enemies;
    private Plant currFood;
    private float digestionRate;
    
    public enum HervivoreFood
    {
        grass, plant
    }

    public enum AI_State
    {
        wandering, finding_food, fleeing           
    }

    public HervivoreFood foodType;
    [HideInInspector] public AI_State myState;
    new void Start()
    {
        digestionRate = Random.Range(5, 10);
        base.Start();
        myState = AI_State.wandering;
        no_of_Enemies = 0;
        
    }

    new void Update()
    {
       
        base.Update();
        SenseAnimals();
        eating = false;
        if (myState == AI_State.fleeing)
        {            
            agent.stoppingDistance = 0;
            agent.speed = (stamina > 0) ? fleeSpeed : moveSpeed;
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (enemies.Length == 0)
                {
                    myState = AI_State.wandering;
                }
                else
                {
                    Flee();
                }
            }
        }
        else
        {
            agent.stoppingDistance = 0;
            agent.speed = moveSpeed;
            if (myState == AI_State.wandering)
            {
                
                if (hungry)
                {
                    myState = AI_State.finding_food;
                }
                Wander();
            }
            else if (myState == AI_State.finding_food)
            {
                
                if (foodType == HervivoreFood.grass)
                {
                    eating = true;
                    agent.SetDestination(transform.position);
                    anim.SetFloat("FoodValue", digestionRate);
                }
                else if (foodType == HervivoreFood.plant)
                {
                    agent.stoppingDistance = bodyLength/2;
                    
                    Plant f = GetFood();
                    if (f)
                    {
                        agent.SetDestination(f.transform.position);
                        if (Vector3.Distance(transform.position, f.transform.position) <= bodyLength/2+eatRange)
                        {
                            agent.velocity = Vector3.zero;
                            currFood = f;
                            eating = true;
                        }
                    }
                    else
                    {
                        Wander();
                    }
                }
                if (!hungry)
                {
                    eating = false;
                    myState = AI_State.wandering;
                }

            }

            if (SensedDanger())
            {
                Flee();
                myState = AI_State.fleeing;
            }
        }

        
    }

    new void LateUpdate()
    {
        base.LateUpdate();        
    }

    protected override void Eat(int i)
    {
        if (foodType == HervivoreFood.plant)
        anim.SetFloat("FoodValue", eating?currFood.Consume():0);
        base.Eat(0);        

    }


    private Plant GetFood()
    {
        Plant[] a = (from b in Physics.OverlapSphere(transform.position, farDetectionRange, food)
                     where (b.GetComponent<Plant>() && b.GetComponent<Plant>().IsConsumable())
                    select b.GetComponent<Plant>()).ToArray();
        if (a.Length == 0)
            return null;
        Plant min=a[0];
        for (int i=1; i < a.Length; i++)
        {
            if (Vector3.Distance(transform.position, a[i].transform.position) < Vector3.Distance(transform.position, min.transform.position)) min = a[i];
        }
        return min;
    }

    private void SenseAnimals()
    {
        enemies=(from b in Physics.OverlapSphere(transform.position, farDetectionRange, animals)
         where (b.GetComponent<Carnivore>() && InSight(b))
         select b.GetComponent<Carnivore>()).ToArray();

        fellows= (from b in Physics.OverlapSphere(transform.position, farDetectionRange, animals)
                            where (b.GetComponent<Hervivores>())
                            select b.GetComponent<Hervivores>()).ToArray();
        
    }

    private bool SensedDanger()
    {
        bool v = false;
        if (enemies.Length != no_of_Enemies)
        {
            if (enemies.Length > no_of_Enemies)
                v = true;
            no_of_Enemies = enemies.Length;
        }
        for (int i = 0; i < fellows.Length; i++)
        {
            if (fellows[i].myState == AI_State.fleeing)
            {
                v = true;
                break;
            }                
        }
        return v;
    }
    
    private void Flee()
    {
        agent.SetDestination(new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100)).normalized*fleeDistance);
    }

    protected override void Die(int i)
    {
        base.Die(0);
        Destroy(GetComponent<Hervivores>());
        agent.isStopped = true;
        Destroy(agent);
    }

}
