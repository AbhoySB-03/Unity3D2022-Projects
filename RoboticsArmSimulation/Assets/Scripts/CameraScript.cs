using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float camDistanceMax=3f, camDistanceMin=0.5f;
    [SerializeField] private Transform cam;
    [SerializeField] private float camZoomSpeed = 50, camRotSpeed = 100;
    [SerializeField] private LayerMask Targetable;
    [SerializeField] private GameObject targetWarningText;
    private float _camDistance;
    private Vector3 _camRotation;
    private Vector3 targetPos;
    private Vector3 defaultPos;
    private float doubleClickTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        _camRotation = transform.eulerAngles;
        _camDistance = -2;
        defaultPos= transform.position;
        targetPos = defaultPos;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        SetCam();
        
    }

    void GetInputs()
    {
        _camDistance = Mathf.Clamp(_camDistance+Input.GetAxis("Mouse ScrollWheel")*camZoomSpeed*Time.deltaTime, -camDistanceMax, -camDistanceMin);
        if (Input.GetMouseButton(1))
        {
            _camRotation.x -= Input.GetAxis("Mouse Y") * camRotSpeed * Time.deltaTime;
            _camRotation.y += Input.GetAxis("Mouse X") * camRotSpeed * Time.deltaTime;
            _camRotation.x = Mathf.Clamp(_camRotation.x, 0, 90);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState= CursorLockMode.Locked;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (doubleClickTimer > 0)
            {
                DoubleClick();
            }
            else
            {
                doubleClickTimer = 0.5f;
            }
        }

        if (doubleClickTimer > 0)
        {
            doubleClickTimer-=Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            targetPos = defaultPos;
        }
    }


    void DoubleClick()
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, Targetable))
            return;
        if (targetPos == defaultPos)
        {
            StartCoroutine(DisplayTarget());
        }
        SetTargetObject(hit.collider.gameObject);
    }
    void SetCam()
    {
        transform.eulerAngles= _camRotation;
        cam.localPosition = new Vector3(0,0,_camDistance);
        if (Vector3.Distance(targetPos, transform.position) > 0.01f)
        {
            transform.position=Vector3.Lerp(transform.position, targetPos, 50 * Time.deltaTime);
        }
    }


    void SetTargetObject(GameObject obj)
    {
        targetPos = obj.transform.position;
    }

    IEnumerator DisplayTarget()
    {
        StopCoroutine(DisplayTarget());
        targetWarningText.SetActive(true);
        yield return new WaitForSeconds(3);
        targetWarningText.SetActive(false);
    }
    
    
}
