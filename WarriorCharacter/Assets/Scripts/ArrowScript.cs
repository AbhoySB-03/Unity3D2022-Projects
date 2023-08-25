using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb= GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_rb)
            return;
        if (_rb.velocity.sqrMagnitude > 0)
        {
            _rb.MoveRotation(Quaternion.LookRotation(_rb.velocity));
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
        transform.parent=collision.gameObject.transform;
        Destroy(_rb);
    }
}
