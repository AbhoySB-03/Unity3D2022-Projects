using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public bool stand;
    Transform[] guardpos;

    public LayerMask NotAlly,GuardPost;
    public float guardtime;
    public Transform MyEye;
    public float SeeDistance,AwareRange,FOV;
    public float GuardRange;
    Vector3 PlayerLastPos;
    public GameObject player;

    NavMeshAgent Nav;
    int currentguardpos;
    Quaternion targetrotation;
    bool OnGuard = false;
    bool searching = false;
    
    EnemyCombat com;
    GameObject[] Allies;
    

    public int AiState=0; //0=idle/OnGuard 1=alert 2=combat 3=searching 4=pursuing


    
    void Start()
    {
        GetGuardPoints();
        com = GetComponentInChildren<EnemyCombat>();
        Nav = GetComponent<NavMeshAgent>();
        targetrotation = transform.rotation;
        
        
    }
    void LateUpdate()
    {
        if (Nav.velocity == Vector3.zero)
        {
            SetRotations();
        }
    }

    void GetGuardPoints()
    {
        Collider[] col;
        col = Physics.OverlapSphere(transform.position, 30,GuardPost);
        
        
        if (col!=null)
        {
            guardpos = new Transform[col.Length];
            for (int i=0; i < col.Length; i++)
            {
                guardpos[i] = col[i].transform;
            }
        }
        else
        {
            guardpos = new Transform[1];
            guardpos.SetValue(transform.position, 0);
        }
        
    }
    private void Update()
    {
        Allies = GameObject.FindGameObjectsWithTag("Enemy");
        if (AiState == 0) //Guarding
        {
            Nav.stoppingDistance = 0;
            if (guardpos.Length != 0)

            {
                LookForPlayer();
                Guard();
            }
        }
        if (AiState == 1) //Keeping an Eye on Player but not Attacking
        {
            EyeOnPlayer();
            LookAtPlayer();
             
        }
        if (AiState == 4) //Attacking player
        {
            
            AlertOthers();
            PursuePlayer();
            if (!InView(player))
            {
                PlayerLastPos = player.transform.position;
                AiState = 3;
            }
            LookAtPlayer();
        }
        if (AiState == 3) //Search the player once lost his sight
        {
            Nav.stoppingDistance = 0;
            if (InView(player))
            {
                AiState = 4;
            }
            if (Nav.velocity.magnitude==0)
            {
                if (!searching)
                {
                    StartCoroutine(WaitAndSearch(2));
                }
            }
        }
        if (AiState == 2) //In Combat with player
        {
            Attack();
        }
        
    }

    IEnumerator WaitAndSearch(float time) //searching procedure
    {
        searching = true;
        while (time > 0 || InView(player))
        {
            time -= Time.deltaTime;
            yield return Time.deltaTime;
               
        }
        
        if (InView(player))
        {
            AiState = 4;
        }
        else
        {
            AiState = 0;
        }
        searching = false;
    }

    
    
    void PursuePlayer() //procedure to charge on player
    {
        Nav.stoppingDistance = com.AttackRange;
        if (InView(player) || AllyAlert())
        {
            Nav.SetDestination(player.transform.position);
            
        }
        
        if (Vector3.Distance(transform.position, player.transform.position) <= com.AttackRange)
        {
            AiState = 2;
        }
        else
        {
            AiState = 4;
        }


    }

    bool AllyAlert() //checcks if any other ally is seeing the player
    {
        foreach (GameObject a in Allies)
        {
            if (!a.GetComponent<HealthScript>().dead())
            {
                if (a.GetComponent<EnemyAI>().InView(player))
                {
                    return true;
                }
            }
            
        }
        return false;
    }

   

    void SetRotations() //set the rotation based on particular target direction
    {
        transform.eulerAngles =new Vector3(0, Quaternion.Slerp(transform.rotation, targetrotation, 0.5f).eulerAngles.y,0);
    }

    void Guard()   //procedure to Guard based on waypoints
    {
       
        if (Vector3.Distance(transform.position, guardpos[currentguardpos].position) > 1)
        {

            Nav.SetDestination(guardpos[currentguardpos].position);
            return;

        }
        else if(!OnGuard)
        {
            StartCoroutine(RemainOnGuard(guardtime));
        }
        

    }

    void LookForPlayer() //Looking if Player is in View
    {
        if (InView(player))
        {
            AiState = 1;
        }
        
        
    }
    void LookAtPlayer() //Set target on player
    {
        targetrotation = Quaternion.LookRotation(player.transform.position - transform.position);
    }

    void EyeOnPlayer()  //Keep an Eye on the Player
    {
        
        if (ShouldAttack())
        {
            ChargeOnPlayer();
        }
        else
        {
            if (!InView(player))
            {
                AiState = 0;
            }
        }
        
    }

    void AlertOthers() //Alerts allies about player's location
    {
        foreach (GameObject a in Allies)
        {
            if (Vector3.Distance(a.transform.position, transform.position) <= SeeDistance)
            {
                if (!a.GetComponent<HealthScript>().dead()) { 
                    if (a.GetComponent<EnemyAI>().AiState != 4)
                    {
                        if (InView(player))
                            a.GetComponent<EnemyAI>().AiState = 4;
                    }
                }
            
            }
        }
    }
    bool ShouldAttack() //Checks if Player Should Be Attacked
    {
        if (InView(player))
        {
            return true;
        }
        return false;
    }

    void ChargeOnPlayer() //sets the AiState in order to attack the Player
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= com.AttackRange)
        {
            AiState = 2;
        }
        else
        {
            AiState = 4;
        }
       

    }

    void Attack() //Attacking Procedure
    {
        LookAtPlayer();
        Nav.stoppingDistance = com.AttackRange; 
        if (Vector3.Distance(transform.position, player.transform.position) <= com.CombatRange + 1)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= com.AttackRange + 1)
            {
                com.Attack();
            }
            else
            {
                AiState = 4;
            }
            
                com.block();
            
        }
        else
        {
            AiState = 4;
        }

    }

    void changeguard()  //changing guard position
    {
        currentguardpos = (currentguardpos + 1) % guardpos.Length;
        
    }

    IEnumerator RemainOnGuard(float duration)  //to remain on guard and watch for a duration
    {
        OnGuard = true;
        if (!InView(player))
        {
            targetrotation = guardpos[currentguardpos].rotation;
        }
        yield return new WaitForSeconds(duration);
        
        OnGuard = false;
        
        changeguard();
        
        
    }

    int GetNearest(Transform[] positions)  //finds the nearest position ot of the given set of position
    {
        int x = 0;

        for (int m = positions.Length-1; m >0 ; m--)
        {
           if ((transform.position -positions[m].position).sqrMagnitude < (transform.position - positions[m - 1].position).sqrMagnitude)
            {
                x = m;
            }
        }
        return x;

    }

    public bool InView(GameObject o)  //checks whether  gameobject is in view of the Eye
    {
        if (Vector3.Distance(transform.position, o.transform.position) < SeeDistance)
        {
            if (Vector3.Angle(transform.forward, o.transform.position - transform.position) <= FOV / 2)
            {
                
                RaycastHit hit;
                if (Physics.Raycast(MyEye.position, o.transform.position - MyEye.position, out hit, SeeDistance,NotAlly))
                {
                   
                    if (hit.collider.transform.root.tag ==o.tag)
                    {
                        return true;
                    }
                }
            }
            else if (Vector3.Distance(transform.position, o.transform.position) < AwareRange)
            {
                return true;
            }
        }
        return false;
    }
}
