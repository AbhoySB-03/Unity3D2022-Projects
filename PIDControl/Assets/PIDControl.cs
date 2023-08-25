
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PIDControl : MonoBehaviour
{
    // Start is called before the first frame update
    public float l_kp,a_kp;
    public float stopdis;
    public float wheelEffortLimit;
    public GoalMarker marker;
    public GameObject MenuScreen;
    public InputField lkp, akp, stpdis, efflim;
    Vector3 targetPosition;

    public WheelCollider w1, w2, w3, w4;
    Rigidbody rgb;
    void Start()
    {
       targetPosition = transform.position;
        rgb= GetComponent<Rigidbody>();
        InitUI();
    }

    // Update is called once per frame

    private void Update()
    {
        GetTarget();
        SetUI();
    }
    void FixedUpdate()
    {
        
        SetWheelVelocities();        
    }

    void SetUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            MenuScreen.SetActive(!MenuScreen.activeSelf);
        }
    }

    void SetWheelVelocities()
    {
        float theta = Vector3.SignedAngle(transform.forward, (targetPosition - transform.position), transform.up)*Mathf.Deg2Rad;
        float twistVel, forwardVel;
        float forwardTorque, angularTorque;
        float distance = Vector3.ProjectOnPlane((targetPosition - transform.position),Vector3.up).magnitude
            *Mathf.Sign(Vector3.Dot((targetPosition - transform.position).normalized,transform.forward));

        twistVel = theta * a_kp;
        forwardVel = distance * l_kp;
        forwardTorque = (forwardVel - Vector3.Dot(rgb.velocity, transform.forward)) / 0.02f;
        angularTorque = (twistVel - rgb.angularVelocity.y) / 0.02f;

        if (Mathf.Abs(distance) < stopdis)
        {
            w1.brakeTorque = w2.brakeTorque = w3.brakeTorque = w4.brakeTorque = 1000;
            w1.motorTorque = w2.motorTorque = 0;
            w4.motorTorque = w3.motorTorque = 0;
        }
        else
        {
            w1.brakeTorque = w2.brakeTorque = w3.brakeTorque = w4.brakeTorque = 0;
            w1.motorTorque = w2.motorTorque = Mathf.Clamp(forwardTorque + angularTorque, -wheelEffortLimit, wheelEffortLimit);
            w4.motorTorque = w3.motorTorque = Mathf.Clamp(forwardTorque - angularTorque, -wheelEffortLimit, wheelEffortLimit);
        }
        



    }

    void InitUI()
    {
        lkp.text = l_kp.ToString();
        akp.text= a_kp.ToString();   
        efflim.text= wheelEffortLimit.ToString();
        stpdis.text= stopdis.ToString(); 
    }

    public void SetLKp()
    {
        l_kp = float.Parse(lkp.text);
    }

    public void SetAKp()
    {
        a_kp = float.Parse(akp.text);
    }

    public void SetEffLim()
    {
        wheelEffortLimit = float.Parse(efflim.text);
    }

    public void SetStopDis()
    {
        stopdis=float.Parse(stpdis.text);
    }

    void GetTarget()
    {
        if (!MenuScreen.activeSelf && Input.GetMouseButton(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out RaycastHit hit, 300)){
            targetPosition= hit.point;
        }
        marker.transform.position = targetPosition;
    }

    
}
