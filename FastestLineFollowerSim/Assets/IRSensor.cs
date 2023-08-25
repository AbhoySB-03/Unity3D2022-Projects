using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRSensor : MonoBehaviour
{
    [SerializeField] private float range, maxVal, minVal;
    [SerializeField, Range(0,1)] private float bias;
    Renderer texRend;
    // Start is called before the first frame update
    

    float GetPerc()
    {
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range))
            return 0;
        texRend = hit.transform.GetComponent<Renderer>();
        Texture2D tex = texRend.material.mainTexture as Texture2D;
        Vector2 texCoor = hit.textureCoord;
        texCoor.x *= tex.width;
        texCoor.y *= tex.height;

        Color c = tex.GetPixel((int)texCoor.x, (int)texCoor.y);
        return c.grayscale;
    }
    public int readAnalog()
    {        
        return (int)Mathf.Lerp(minVal, maxVal, GetPerc());
    }

    public int readDigitalInt()
    {
        return readDigital()?1:0;
        
    }

    public bool readDigital()
    {
        return GetPerc() > bias;
    }


}
