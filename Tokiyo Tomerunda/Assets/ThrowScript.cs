using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowScript : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float throwSpeed;
    [SerializeField] Transform emitter;

    TimeManager t;
    // Start is called before the first frame update
    void Start()
    {
        t=FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(1)) {
            Throw();
        }
    }

    void Throw()
    {
        GameObject projectile=Instantiate(projectilePrefab, emitter.position, emitter.rotation);
        if (t.timeStopped)
        {
            t.Resume(projectile.GetComponent<Rigidbody>(), 0.5f);
        }
        projectile.GetComponent<Rigidbody>().velocity = emitter.forward * throwSpeed;
        Destroy(projectile, 10);
    }
}
