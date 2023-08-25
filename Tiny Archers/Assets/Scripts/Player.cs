using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : Archer
{
    [SerializeField] private LayerMask Enemy;    
    [SerializeField] private GameObject objMarker, targetMarker;

    private bool isAiming;
    private float returnToNonAimedStateTime = 2;
    private PauseMenuScript pauseMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu=FindObjectOfType<PauseMenuScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseMenu.isPaused)
            return;
        MovePlayer();
        Animate();
        
    }

    private void LateUpdate()
    {
        Look();
    }

    void MovePlayer()
    {
        if (Input.touchCount > 0) {
            Touch _t = Input.GetTouch(0);
            if (_t.phase == TouchPhase.Ended && target != null)
            {
                Shoot();
            }
            else if (_t.phase == TouchPhase.Began)
            {

                if (Physics.Raycast(Camera.main.ScreenPointToRay(_t.position), out RaycastHit hit, 100) && !EventSystem.current.IsPointerOverGameObject(_t.fingerId))
                {
                    if (Enemy.ContainsLayer(hit.collider.gameObject.layer))
                    {
                        target = hit.collider.gameObject;
                        PlaceTargetIndicator();
                        Aimed = true;
                        returnToNonAimedStateTime = 2;
                    }
                    else
                    {

                        PlaceTargetMoveIndicator(hit.point, hit.normal);
                        nav.SetDestination(hit.point);
                    }
                }
            }
        }      
        
        if (nav.remainingDistance <= nav.stoppingDistance+1.5f)
        {
            RemoveTargetMoveIndicator();
        }
    }

    void PlaceTargetMoveIndicator(Vector3 pos, Vector3 normal)
    {
        objMarker.transform.position = pos+normal*0.05f;
        objMarker.transform.rotation = Quaternion.LookRotation(normal);
        objMarker.SetActive(true);
    }
    
    void RemoveTargetMoveIndicator()
    {
        objMarker.SetActive(false);
    }
    void Look()
    {
        if (target!=null)
        {
            
            nav.angularSpeed = 0;
            LookAtTarget();
            if (!Aimed)
            {
                returnToNonAimedStateTime-=Time.deltaTime;
            }
            if (returnToNonAimedStateTime <= 0)
            {
                target=null;
                RemoveTargetIndicator();
            }
        }
        else
        {
            nav.angularSpeed = 120;
        }

    }

    protected override void Death(int _)
    {
        base.Death(_);
    }

    void PlaceTargetIndicator()
    {
        if (target == null)
            return;
        targetMarker.SetActive(true);
        targetMarker.transform.rotation = Quaternion.identity;
        targetMarker.transform.parent=target.transform;
        targetMarker.transform.localPosition = Vector3.zero;

    }

    void RemoveTargetIndicator()
    {
        targetMarker.transform.parent = null;
        targetMarker.SetActive(false);
    }

   
}
