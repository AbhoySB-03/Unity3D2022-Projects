using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float maxFoodValue;

    protected float foodValue;
    public virtual float Consume()
    {
        foodValue -= 0.1f * maxFoodValue;
        return 0.1f * maxFoodValue;

    }

    public virtual bool IsConsumable()
    {
        return foodValue > 0;
    }

   
}
