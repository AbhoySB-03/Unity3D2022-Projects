using UnityEngine;
using UnityEngine.VFX;

public class MjolnirCharged : MonoBehaviour
{
    [Header("Charged Mode")]
    [SerializeField] public bool charged;
    [SerializeField] private GameObject lightningEffect;
    [SerializeField] private LayerMask notMe;
    [SerializeField] private ParticleSystem charge;
    [SerializeField] private ParticleSystem burst;
    [SerializeField] private float totalChargeAmount;
    [SerializeField] private float hitChargeCostFactor;
    [SerializeField] private float burstExtentFactor;
    [SerializeField] private float arcAttackRange;
    [SerializeField] private float arcCurveFactor;
    [SerializeField] private float arcArrackCost;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject targetSpark;
    [SerializeField] private GameObject LightningArc;


    private VisualEffect arcEffect;
    
    bool isArcAttacking;
    private Vector3 arcDesiredPosition;
    private float chargeLeft;
    private Material myMat;
    
    // Start is called before the first frame update
    void Awake()
    {
        myMat = GetComponent<MeshRenderer>().materials[1];
        arcEffect=LightningArc.GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        ChargedEffects();
        
        if (chargeLeft <= 0)
        {
            charged = false;
        }

        
        if (isArcAttacking) { 
            if (Physics.Raycast(LightningArc.transform.position, Camera.main.transform.forward, out RaycastHit hit, arcAttackRange, notMe))
            {
                arcDesiredPosition = hit.point;
                if (Vector3.Distance(targetTransform.position,hit.point)<0.1f && hit.rigidbody)
                {
                    hit.rigidbody.AddForce(Camera.main.transform.forward * 20f);
                }
                targetSpark.SetActive(true);
            }
            else
            {
                targetSpark.SetActive(false);
                arcDesiredPosition = LightningArc.transform.position + Camera.main.transform.forward * arcAttackRange;
            }
            chargeLeft -= Time.deltaTime * arcArrackCost;
            targetTransform.position = Vector3.Lerp(targetTransform.position, arcDesiredPosition, 0.5f);
            //arcEffect.SetVector3("Velocity", Vector3.ProjectOnPlane((arcDesiredPosition - targetTransform.position), transform.forward) * arcCurveFactor);
            arcEffect.SetVector3("Velocity", new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0)* arcCurveFactor);
        }
        else
        {
            targetSpark.SetActive(false);
        }
    }

    void ChargedEffects()
    {
        lightningEffect.SetActive(charged);
        if (charged)
        {
            myMat.EnableKeyword("_EMISSION");
        }
        else
        {
            myMat.DisableKeyword("_EMISSION");
        }
        


    }

    public void StartArcAttack()
    {
        LightningArc.SetActive(true);
        isArcAttacking = true; 
    }


    public void StopArcAttacking()
    {
        LightningArc.SetActive(false);
        arcDesiredPosition = LightningArc.transform.position;
        isArcAttacking= false;
        targetTransform.position = arcDesiredPosition;
        targetSpark.SetActive(false);

    }
    public void ChargeHammer()
    {
        charge.Play();
        charged = true;
        chargeLeft = totalChargeAmount;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!charged) return;
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") && collision.relativeVelocity.magnitude>burstExtentFactor)
        {
            chargeLeft -= hitChargeCostFactor;
            chargeLeft = Mathf.Clamp(chargeLeft, 0, totalChargeAmount);
            burst.Play();
            
        }
    }

    
}
