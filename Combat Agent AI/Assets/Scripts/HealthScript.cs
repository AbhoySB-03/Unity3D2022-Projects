using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class HealthScript : MonoBehaviour
{
    public float MaxHealth;
    public float MaxCombatStamina;
    public float combatstamina;
    public float health;

    Rigidbody[] bodies;
    void Start()
    {
        health = MaxHealth;
        combatstamina = MaxCombatStamina;
        bodies = GetComponentsInChildren<Rigidbody>();
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        health = Mathf.Clamp(health, 0, MaxHealth);
        combatstamina = Mathf.Clamp(combatstamina, 0, MaxCombatStamina);

        if (dead())
        {
            die();
        }
    }

    public void RestoreHealth(float Health)
    {
        health += health;
    }

    public void TakeDamage(float DamageValue)
    {
        health -= DamageValue;
    }

    void die()
    {

        foreach (Rigidbody a in bodies)
        {
            a.isKinematic = false;
        }
        if (gameObject.tag == "Player")
        {

            SceneManager.LoadScene(0);
        }
        else
        {
            (GetComponent<EnemyAI>()).enabled = false;
        }
        GetComponentInChildren<Animator>().enabled=false;
        (GetComponent<NavMeshAgent>()).enabled=false;
        
        Destroy(gameObject, 6);
        (GetComponent<HealthScript>()).enabled=false;

    }
    public bool dead()
    {
        if (health > 0)
        {
            return false;
        }
        return true;
    }
}
