using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] public Player player;
    [SerializeField] public float cameraShiftingFactor;

    
    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;
        transform.position=player.transform.position+player.transform.forward*cameraShiftingFactor;       
    }
}
