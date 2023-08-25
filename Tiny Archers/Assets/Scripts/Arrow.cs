using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Arrow : MonoBehaviour
{
    [SerializeField] private LayerMask attachable;
    [SerializeField] private LayerMask destroyOnTouch;
    [SerializeField] private float damage;
    

    private bool collided;
    private AudioSource myAudio;
    private Rigidbody _rb;
    // Start is called before the first frame update
    void Awake()
    {
        _rb= GetComponent<Rigidbody>();
        _rb.isKinematic = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        collided = false;
        myAudio= GetComponent<AudioSource>();
    }

    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (_rb.velocity.magnitude > 0 && !collided)
        {
            _rb.MoveRotation(Quaternion.LookRotation(_rb.velocity));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        myAudio.Play();
        if (destroyOnTouch.ContainsLayer(collision.gameObject.layer))
        {
            _rb.velocity = Vector3.zero;
            _rb.isKinematic = true;
            transform.parent = collision.gameObject.transform;
            StartCoroutine(DespawnArrow(0.2f));
        }        
        else if (attachable.ContainsLayer(collision.gameObject.layer))
        {
            _rb.velocity = Vector3.zero;
            _rb.isKinematic = true;
            transform.parent = collision.gameObject.transform;
            StartCoroutine(DespawnArrow(4f));

        }
        
        collided= true;
        if (collision.gameObject.GetComponent<Character>())
        collision.gameObject.SendMessage("TakeDamage", damage);   
        
        
    }

    IEnumerator DespawnArrow(float sec)
    {
        yield return new WaitForSeconds(sec);
        transform.parent = null;
        _rb.isKinematic = false;
        gameObject.SetActive(false);

    }
}
