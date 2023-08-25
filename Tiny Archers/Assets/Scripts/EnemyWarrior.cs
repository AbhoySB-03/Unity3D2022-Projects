using UnityEngine;

public class EnemyWarrior : Warrior
{
    [SerializeField] private GameObject player;
    [SerializeField] private float detectionRange, attackRange;
    [SerializeField] private float searchingTime;
    [SerializeField] private bool guard;

    private float searchTimeLeft;
    private Vector3 guardPosition;

    private enum enemyState
    {
        idle, pursue, search, attack
    }

    private enemyState state;

    // Start is called before the first frame update
    void Start()
    {
        guardPosition = transform.position;
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
        Look();
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
            UnlockTarget();
            return;
        }
        nav.stoppingDistance = attackRange;
        nav.SetDestination(player.transform.position);
        if (target==null)
        LockTarget(player);
        if (Vector3.Distance(player.transform.position, transform.position) > detectionRange)
        {
            UnlockTarget();
            if (shielding)
            {
                shielding= false;
            }
            state = enemyState.search;
            searchTimeLeft = searchingTime;
        }
        else if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
        {
            state = enemyState.attack;
        }

    }

    void SearchBehavior()
    {
        if (player == null)
        {
            state = enemyState.idle;
            UnlockTarget();
            return;
        }
        nav.stoppingDistance = 0;
        if (Vector3.Distance(player.transform.position, transform.position) <= detectionRange)
        {
            audioSource.clip = battlecry;
            audioSource.Play();
            state = enemyState.pursue;
        }
        if (searchTimeLeft> 0)
        {
            if (nav.velocity.magnitude<0.05f)
            searchTimeLeft-=Time.deltaTime;
        }
        else
        {
            state = enemyState.idle;
        }
    }

    protected virtual void AttackBehavior()
    {
        if (player == null)
        {
            state = enemyState.idle;
            UnlockTarget();
            return;
        }
        nav.stoppingDistance = attackRange;
        if (Vector3.Distance(player.transform.position, transform.position) > attackRange)
        {
            
            state = enemyState.pursue;            
            return;
        }
        
        if (canAttack)
        {
            Attack();
            attackCoolDown = attackCoolDownTime;
        }
        else
        {
            attackCoolDown-=Time.deltaTime;
        }
    }

    protected override void Death(int _)
    {
        base.Death(_);
    }

    void Look()
    {
        if (target!= null)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position-transform.position ), 10 * Time.deltaTime);
        }
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
        if (state != enemyState.idle || state==enemyState.search)
            return;
        
        nav.SetDestination(player.transform.position);
        state = enemyState.search;
        searchTimeLeft = searchingTime;
    }
}
