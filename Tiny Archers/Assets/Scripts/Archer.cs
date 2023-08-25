using UnityEngine;

public class Archer : Character
{
    [SerializeField] protected Transform emitterTransform;
    [SerializeField] protected GameObject handArrow;
    [SerializeField] protected float shootSpeed;
    [SerializeField] protected AudioClip bowPullAudio,bowReleaseAudio;

    protected GameObject target;
    protected bool Aimed;
    
    

    // Update is called once per frame
    
    protected override void Animate()
    {
        base.Animate();
        anim.SetBool("Aim", Aimed);
    }

    protected void Shoot()
    {
        Aimed= false;
    }

    protected virtual void LookAtTarget()
    {
        transform.rotation=Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position-transform.position), 10*Time.deltaTime);
    }
    void ShootArrow(int _)
    {
        audioSource.clip= bowReleaseAudio;
        audioSource.Play();
        GameObject g = ObjectPooler.Instance.SpawnPoolObject("Arrow", emitterTransform.position, emitterTransform.rotation);
        Rigidbody r = g.GetComponent<Rigidbody>();
        r.velocity=emitterTransform.forward*shootSpeed;
    }

    void BowPull(int _)
    {
        audioSource.clip= bowPullAudio;
        audioSource.Play();
    }
    void HandleArrow(int i)
    {
        handArrow.SetActive(i == 1);
    }
}
