using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassDisplacement : MonoBehaviour
{
    [SerializeField] private Vector3 _displacementOffset ;
    
    // Start is called before the first frame update


    // Update is called once per frame
    
    void Update()
    {
        Shader.SetGlobalVector("_Displacement", transform.position+_displacementOffset);
    }
}
