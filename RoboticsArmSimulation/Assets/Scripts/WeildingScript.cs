using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeildingScript : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private GameObject spark;
    [SerializeField] private LayerMask weildingObject;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        spark.SetActive(Physics.Raycast(transform.position, transform.forward, distance, weildingObject));
        
    }
}
