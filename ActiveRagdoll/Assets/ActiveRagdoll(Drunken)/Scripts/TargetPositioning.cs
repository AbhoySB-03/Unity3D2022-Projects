
using UnityEngine;

public class TargetPositioning : MonoBehaviour
{
    
    Rigidbody ParentRgb;
    public Vector3 desVelocity;
    Vector3 targetOffset;

    public float targetPosOffsetFactor;
    void Start()
    {
        ParentRgb = transform.parent.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
