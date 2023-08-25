using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TimeManager : MonoBehaviour
{
    public bool timeStopped, stopTime;
    Rigidbody[] rigidBodies;
    Dictionary<Rigidbody, RigidBodyInfo> rigidBodyinfos;
    // Start is called before the first frame update
    void Start()
    {
        rigidBodies = GameObject.FindObjectsOfType<Rigidbody>();
        rigidBodyinfos = new Dictionary<Rigidbody, RigidBodyInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            stopTime = !stopTime;
        }
        if (timeStopped)
        {
            if (rigidBodies.Length != GameObject.FindObjectsOfType<Rigidbody>().Length)
            {
                rigidBodies = GameObject.FindObjectsOfType<Rigidbody>();
                RecoredRBInfos();
            }

            if (!stopTime)
            {
                ReleaseRBInfos();
                timeStopped = false;
            }
        }
        else
        {
            if (stopTime)
            {
                RecoredRBInfos();
                timeStopped = true;
            }
        }
    }

    void RecoredRBInfos()
    {
        for (int i = 0; i < rigidBodies.Length; i++)
        {
            if (rigidBodies[i])
            {
                RigidBodyInfo info = new RigidBodyInfo();
                info.useGravity = rigidBodies[i].useGravity;
                info.angularVelocity = rigidBodies[i].angularVelocity;
                info.velocity = rigidBodies[i].velocity;
                info.isKinematic = rigidBodies[i].isKinematic;
                if (!rigidBodyinfos.ContainsKey(rigidBodies[i]))
                    rigidBodyinfos.Add(rigidBodies[i], info);
                if (rigidBodies[i].isKinematic == false)
                {
                    rigidBodies[i].velocity = Vector3.zero;
                    rigidBodies[i].angularVelocity = Vector3.zero;
                }
                rigidBodies[i].isKinematic = true;
                rigidBodies[i].useGravity = false;
            }
            else
            {
                rigidBodyinfos.Remove(rigidBodies[i]);
            }
        }

    }

    void ReleaseRBInfos()
    {
        for (int i=0; i<rigidBodies.Length; i++)
        {
            if (rigidBodies[i])
            {
                rigidBodies[i].isKinematic = rigidBodyinfos[rigidBodies[i]].isKinematic;
                rigidBodies[i].useGravity = rigidBodyinfos[rigidBodies[i]].useGravity;
                if (rigidBodies[i].isKinematic == false)
                {
                    rigidBodies[i].velocity = rigidBodyinfos[rigidBodies[i]].velocity;
                    rigidBodies[i].angularVelocity = rigidBodyinfos[rigidBodies[i]].angularVelocity;
                }
                
            }

        }
        rigidBodyinfos.Clear();
    }

    public void Resume(Rigidbody rb, float time)
    {
        StartCoroutine(resumeForRB(rb,time));
    }

    IEnumerator resumeForRB(Rigidbody rb, float time)
    {
        rb.isKinematic = rigidBodyinfos[rb].isKinematic;
        
        yield return new WaitForSeconds(time);
        if (timeStopped)
        {
            rigidBodyinfos[rb].isKinematic = rb.isKinematic;
            rigidBodyinfos[rb].velocity += rb.velocity;
            rigidBodyinfos[rb].angularVelocity += rb.angularVelocity;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    
}



class RigidBodyInfo{
    public bool isKinematic;
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public bool useGravity;

}
