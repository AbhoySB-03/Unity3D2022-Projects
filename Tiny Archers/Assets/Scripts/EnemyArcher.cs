using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyArcher:Archer
{
    [SerializeField] private GameObject player;
    [SerializeField] private float detectionRange, attackRange;
    [SerializeField] private float aimTime;
    [SerializeField] private float searchingTime;
    [SerializeField] private bool guard;
    [SerializeField] private LayerMask ally;
    private float searchTimeLeft;
    private float aimTimeLeft;
    private Vector3 guardPosition;

    private enum enemyState
    {
        idle, pursue, search, attack
    }

    private enemyState state;

    // Start is called before the first frame update
    void Start()
    {
        guardPosition=transform.position;
        state = enemyState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case enemyState.idle:
                IdleBehavior();
                break;
            case enemyState.pursue:
                PursueBehavior();
                break;
            case enemyState.search:
                SearchBehavior();
                break;
            case enemyState.attack:
                AttackBehavior();
                break;
        }
        Animate();
    }

    void IdleBehavior()
    {
        if (guard)
        {
            nav.SetDestination(guardPosition);
        }
        if (player == null)
            return;
        if (Vector3.Distance(player.transform.position, transform.position) <= detectionRange)
        {            
            state = enemyState.pursue;
            AlertOthers();
        }
    }
    void PursueBehavior()
    {
        if (player == null)
        {
            state = enemyState.idle;
            if (Aimed)
            {
                Aimed = false;
                anim.SetTrigger("Cancel");
            }
            target = null;
            return;
        }
        nav.stoppingDistance = attackRange;
        nav.SetDestination(player.transform.position);
        if (Vector3.Distance(player.transform.position, transform.position) > detectionRange)
        {
            if (Aimed)
            {
                anim.SetTrigger("Cancel");
                Aimed= false;
            }
            state = enemyState.search;
        }
        else if(Vector3.Distance(player.transform.position, transform.position) <= attackRange)
        {
            state= enemyState.attack;
        }
        
    }

    void SearchBehavior()
    {
        if (player == null)
        {
            state = enemyState.idle;
            if (Aimed)
            {
                Aimed = false;
                anim.SetTrigger("Cancel");
            }
            target = null;
            return;
        }
        nav.stoppingDistance = 0;
        if (Vector3.Distance(player.transform.position, transform.position) <= detectionRange)
        {
            state = enemyState.pursue;
        }
        if (searchTimeLeft > 0)
        {
            searchTimeLeft -= Time.deltaTime;
        }
        else
        {
            state = enemyState.idle;
        }
    }

    void AttackBehavior()
    {
        if (player == null)
        {
            state = enemyState.idle;
            if (Aimed)
            {
                Aimed = false;
                anim.SetTrigger("Cancel");
            }
            target = null;
            return;
        }
        nav.stoppingDistance = attackRange;
        if (Vector3.Distance(player.transform.position, transform.position) > attackRange)
        {
            target = null;
            state = enemyState.pursue;
            nav.angularSpeed = 120;
            
            return;
        }
        target = player;
        nav.angularSpeed = 0;
        LookAtTarget();
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Aim") && Aimed)
        {
            if (aimTimeLeft <= 0)
            {
                Aimed = false;
                aimTimeLeft = aimTime;
            }
            else
            {
                aimTimeLeft -= Time.deltaTime;
            }
        }
        else
        {
            Aimed = true;
        }        
    }

    protected override void Death(int _)
    {
        base.Death(_);
    }
    public override void TakeDamage(float Damage)
    {
        base.TakeDamage(Damage);
        AlertOthers();
    }

    void AlertOthers()
    {
        foreach (Collider c in Physics.OverlapSphere(transform.position, detectionRange, ally))
        {
            if (!c.gameObject.GetComponent<Character>())
                continue;
            c.gameObject.SendMessage("CheckPlayer");
        }

    }
    void CheckPlayer()
    {
        if (state != enemyState.idle && state !=enemyState.search)
            return;

        nav.SetDestination(player.transform.position);
        state = enemyState.search;
        searchTimeLeft = searchingTime;
    }
}
