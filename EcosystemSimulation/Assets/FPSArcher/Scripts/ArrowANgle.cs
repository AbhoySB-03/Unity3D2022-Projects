using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowANgle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<Rigidbody>() != null)
        {
            this.transform.rotation = Quaternion.LookRotation(this.GetComponent<Rigidbody>().velocity);
        }
        
        
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Player")
        {
            
            Destroy(this.GetComponent<Rigidbody>());

            this.transform.SetParent(col.gameObject.transform);
        }
    }
}
