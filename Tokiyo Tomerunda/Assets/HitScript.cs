using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScript : MonoBehaviour
{
    [SerializeField] float hitDistance;
    [SerializeField] float hitForce;
    [SerializeField] float hitRadius;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] Transform cam;
    TimeManager t;
    // Start is called before the first frame update
    void Start()
    {
        t = FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.SphereCast(new Ray(cam.position,cam.forward), hitRadius, out RaycastHit hit, hitDistance-hitRadius))
            {
                if (hit.rigidbody)
                {
                    if (t.timeStopped)
                    {
                        foreach (Rigidbody r in hit.rigidbody.transform.root.GetComponentsInChildren<Rigidbody>())
                            t.Resume(r, 0.1f);
                    }
                    hit.rigidbody.AddForceAtPosition(cam.forward * hitForce, hit.point);                    
                }
            }
        }
        
    }
}
