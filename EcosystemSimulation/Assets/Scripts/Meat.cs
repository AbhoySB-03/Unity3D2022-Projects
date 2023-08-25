using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : Food
{
    public float decomposeRate;

    private void Start()
    {
        foodValue = maxFoodValue;
    }
    // Update is called once per frame
    void Update()
    {
        //foodValue -= decomposeRate* Time.deltaTime;
        Mathf.Clamp(foodValue, 0, maxFoodValue);
        if (foodValue <= 0)
        {
            Destroy(gameObject);
        }       
        
    }

    
}
