using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScript : MonoBehaviour
{
    [SerializeField] private GameObject hitParticle;
    [SerializeField] private LayerMask hitLayer;
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision collision)
    {
        if (checkLayerInLayermask(collision.gameObject.layer, hitLayer)){
            GameObject g = Instantiate(hitParticle, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
            Destroy(g, 1);
        }
    }

    private static bool checkLayerInLayermask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
    
}
