using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float moveSpeed;
    private ProceduralFootPlacement footPlacement;
    // Start is called before the first frame update
    void Start()
    {
        footPlacement= GetComponent<ProceduralFootPlacement>();
    }

    // Update is called once per frame
    void Update()
    {
        footPlacement.moveDir = (Input.GetAxisRaw("Vertical") * Vector3.forward + Input.GetAxisRaw("Horizontal") * Vector3.right).normalized;
        footPlacement.moveFactor = moveSpeed;
    }
}
