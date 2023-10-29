using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public TargetPositioning t;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t.desVelocity = (Vector3.forward * Input.GetAxisRaw("Vertical")+Vector3.right * Input.GetAxisRaw("Horizontal")).normalized*speed;
    }
}
