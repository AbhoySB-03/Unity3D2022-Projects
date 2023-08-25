using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRArray : MonoBehaviour
{    
    [SerializeField] private int arrayLength;
    [SerializeField] private float width,thickness;
    [SerializeField] private GameObject IRSensorPrefab;
    [SerializeField] private int minval, maxval;

    [SerializeField]private bool debug;
    private IRSensor[] irSensors;
    private int linePos;

    
    
    // Start is called before the first frame update
    void Awake()
    {
        InitArray();
    }



    // Update is called once per frame
    void Update()
    {
        if (debug)
        {
            Debug.Log(readLine());
        }
        
    }

    void InitArray()
    {
        irSensors= new IRSensor[arrayLength];
        float distanceBetween=width/(arrayLength-1);
        for (int i=0; i<arrayLength; i++)
        {
            Vector3 pos=transform.position-transform.right*(width/2-i*distanceBetween);
            GameObject g=Instantiate(IRSensorPrefab, pos+transform.forward*thickness/2, transform.rotation, transform);
            irSensors[i]=g.GetComponent<IRSensor>();
        }
        
    }

    public int readLine()
    {
        float val=0;
        int n = 0;
        for (int i=0; i<arrayLength; i++)
        {
            if (!irSensors[i].readDigital())
            {
                val += i;
                n++;
            }

        }
        if (n == 0) return linePos;
        val /= n;

        val /= arrayLength;
        linePos=(int)Mathf.Lerp(minval, maxval, val);
        return linePos;
    }
}
