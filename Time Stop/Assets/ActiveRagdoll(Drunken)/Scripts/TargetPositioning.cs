
using UnityEngine;

public class TargetPositioning : MonoBehaviour
{
    
    Rigidbody ParentRgb;
    public Vector3 desVelocity;
    Vector3 targetOffset;
    TimeManager t;

    public float targetPosOffsetFactor;
    void Start()
    {
        ParentRgb = transform.parent.GetComponent<Rigidbody>();
        t= FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (t.timeStopped) return;
        if (desVelocity.sqrMagnitude > 0)
        {
            targetOffset = desVelocity;
        }
        else
        {
            targetOffset = ParentRgb.velocity;
        }
        targetOffset.y = 0;
        targetOffset *= targetPosOffsetFactor;


        transform.eulerAngles = new Vector3(0, transform.parent.localEulerAngles.y, 0);

        transform.position =transform.parent.position + targetOffset;
        
    }
}
