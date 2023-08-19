using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float springForce, damping;
    [Range(0f, 1f)]
    public float Strength;
    private Rigidbody _body;
    private Vector3 desiredVelocity;
    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        desiredVelocity = (target.transform.position - transform.position)*springForce;
        _body.AddForce(damping *Strength* (desiredVelocity - _body.velocity));
    }
}
