using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    public GameObject handSword, backSword, handBow, backBow, handShield, backShield, handArrow;
    public GameObject arrowPrefab;
    public LayerMask enemies;
    public float ShootSpeed;
    public Transform arrowEmitter;    

    public GameObject target;
    public bool targeting;
    public int combatType;
    bool attack;  
    public bool aiming;
    public bool shielding;

    [SerializeField] private GameObject[] quiverArrows;
    [SerializeField] private float targetRange;
    [SerializeField] private int quiverSize;
    [SerializeField] private Transform camPivot;
    [SerializeField] private Text arrowUI;
    private List<GameObject> targetList;
    private int attackType;
    private int arrowCount;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim=GetComponent<Animator>();
        targetList= new List<GameObject>();
        arrowCount = quiverSize;
        attackType = 1;
        arrowUI.text = "Quiver: " + arrowCount + " Arrows";
    }

    // Update is called once per frame
    void Update()
    {        
        GetInput();
        CheckTargeting();
        Animate();
    }

    void CheckTargeting()
    {
        if (target==null && shielding)
        {
            GetTarget();
        }
        else if (!shielding)
        {
            RemoveTarget();
        }
    }
    void Animate()
    {
        anim.ResetTrigger("Shoot");
        anim.ResetTrigger("Attack");
        anim.SetInteger("CombatType", combatType);
        if (attack)
        {
            if (attackType == 1)
            { anim.SetTrigger("Attack"); }
            else if (attackType == 2)
            {
                anim.SetTrigger("Attack2");
            }
        }

        anim.SetBool("BowAim", aiming);
        anim.SetBool("Shield", shielding);
        anim.SetBool("BowTargetting", targeting);
    }

    void ChangeAttackType(int _)
    {
        if (attackType == 1)
        {
            attackType = 2;
        }
        else if (attackType == 2)
        {
            attackType = 1;
        }
    }
    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            combatType = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            combatType = 2;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            combatType = 0;
        }

        attack = Input.GetMouseButtonDown(0) && combatType==1;
        aiming = Input.GetMouseButton(0) && targeting;
        
        if (combatType != 2)
        {
            aiming = false;
        }
        shielding = Input.GetMouseButton(1) && combatType == 1;
        targeting = Input.GetMouseButton(1) && combatType == 2;

    }


    void GetTarget()
    {
        GameObject minTarget;
        targetList.Clear();
        targetList=(from c in Physics.OverlapSphere(transform.position, targetRange, enemies) select c.gameObject).ToList<GameObject>();
        if (targetList.Count == 0)
            return;
        minTarget=targetList.First();
        float currAngle = Vector3.Angle(camPivot.forward, minTarget.transform.position - transform.position);
        for(int i=1; i<targetList.Count; i++)
        {
            float angle = Vector3.Angle(camPivot.forward, targetList[i].transform.position - transform.position);
            if ( angle < currAngle)
            {
                minTarget = targetList[i];
                currAngle = angle;
            }
        }

        target = minTarget;

    }

    void RemoveTarget()
    {
        target = null;
    }

    void HandleSword(int i)
    {
        handSword.SetActive(i == 1);
        backSword.SetActive(i == 0);
    }

    void HandleShield(int i)
    {
        handShield.SetActive(i == 1);
        backShield.SetActive(i == 0);
    }
    void HandleBow(int i)
    {
        handBow.SetActive(i == 1);
        backBow.SetActive(i == 0);
    }

    void HandleArrow(int i)
    {
        if (i == 1)
        {
            if (arrowCount > 0)
            {
                handArrow.SetActive(true);
                arrowCount--;
                RefreshQuiver();
            }
        }
        else
        {
            handArrow.SetActive(false);
            arrowCount++;
            RefreshQuiver();
        }
        
    }

    void ShootArrow(int i)
    {
        handArrow.SetActive(false);
        if (arrowCount <= 0)
            return;
        
        GameObject a=Instantiate(arrowPrefab, arrowEmitter.transform.position, arrowEmitter.transform.rotation);
        Rigidbody r=a.GetComponent<Rigidbody>();
        r.velocity = arrowEmitter.forward*ShootSpeed;
        Destroy(a, 20);

        
    }

    void RefreshQuiver()
    {
        arrowCount = Mathf.Clamp(arrowCount, 0, quiverSize);
        arrowUI.text = "Quiver: " + arrowCount+ " Arrows";
        if (arrowCount < quiverArrows.Length)
        {
            for (int i = 0; i < quiverArrows.Length; i++)
            {
                if (i < arrowCount)
                {
                    quiverArrows[i].SetActive(true);

                }
                else
                {
                    quiverArrows[i].SetActive(false);
                }
            }
        }
    }

    public void AddArrows(int amount)
    {
        arrowCount = Mathf.Clamp(arrowCount + amount, 0, quiverSize);
        RefreshQuiver();
    }
    
    public int QuiverSpaceLeft()
    {
        return quiverSize - arrowCount;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        ArrowScript ar = hit.gameObject.GetComponent<ArrowScript>();
        if (ar && QuiverSpaceLeft() > 0)
        {
            AddArrows(1);
            if (!handArrow.activeSelf)
                HandleArrow(1);
            Destroy(ar.gameObject);
        }

        if (hit.gameObject.layer == LayerMask.NameToLayer("Container"))
        {
            arrowCount = quiverSize;
            RefreshQuiver();
        }
    }
}
