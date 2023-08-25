using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Food
{
    [SerializeField] private float growthTime;

    private float growningCurrTime;
    private bool isRecovering;
    // Start is called before the first frame update
    void Start()
    {
        foodValue = maxFoodValue;
        growningCurrTime = growthTime;
    }

    // Update is called once per frame
    void Update()
    {       
        transform.localScale = Vector3.one *(0.5f+0.5f* foodValue / maxFoodValue);
        if (growningCurrTime < growthTime)
        {
            growningCurrTime += Time.deltaTime;
        }
        else
        {
            foodValue += Time.deltaTime;
        }
        if (foodValue >= maxFoodValue && isRecovering) isRecovering = false;
        if (foodValue == 0 && !isRecovering) isRecovering = true;
        foodValue = Mathf.Clamp(foodValue, 0, maxFoodValue);
    }

    public override float Consume()
    {
        growningCurrTime = 0;
        return base.Consume();
    }

    public override bool IsConsumable()
    {
        return !isRecovering;
    }
}
