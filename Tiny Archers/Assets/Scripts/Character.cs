using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    [SerializeField] protected float maxHealth=100, moveSpeed=2;
    [SerializeField] protected AudioClip deathCry;

    protected float health;
    protected NavMeshAgent nav;
    protected Animator anim;
    protected AudioSource audioSource;

    public bool isDead
    {
        get
        {
            return health <= 0;
        }
    }
    // Start is called before the first frame update
    protected void Awake()
    {
        health = maxHealth;
        nav= GetComponent<NavMeshAgent>();
        anim= GetComponent<Animator>();
        nav.speed = moveSpeed;
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Animate()
    {
        if (anim == null)
            return;
        anim.SetFloat("Speed", nav.velocity.magnitude);
    }
    
    public virtual void TakeDamage(float Damage)
    {
        health-=Damage;
        anim.SetTrigger("Hit");
        RefreshHealth();
        if (isDead)
        {
            anim.Play("Dead");
            anim.SetBool("Dead", true);
        }
    }

    protected virtual void Death(int _)
    {
        audioSource.clip= deathCry;
        audioSource.Play();
        anim.enabled=false;
        nav.enabled= false;
        Destroy(gameObject, 10);
        Destroy(this);
    }
    void RefreshHealth()
    {
        health=Mathf.Clamp(health, 0, maxHealth);
    }
}
