using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepController : MonoBehaviour
{
    public FootStep LF,RF;
    public LegScript L, R;
    public int movLegIndex=1,nextLegIndex=2;
   
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Stepping())
        {
            movLegIndex = nextLegIndex;
        }
        else
        {
            nextLegIndex = getNextLeg();
        }
        if (movLegIndex == 1)
        {
            L.canStep = true;
            R.canStep = false;

        }        
       else if (movLegIndex == 2)
        {
            L.canStep = false;
            R.canStep = true;
        }       
    }

    bool Stepping()
    {
       if (LF.Stepping || RF.Stepping)
        {
            return true;
        }
        return false;

    }

    int getNextLeg()
    {
        if (LF.Stepping)
        {
            return 2;
        }
        else if(RF.Stepping)
        {
            return 1;
        }
        else
        {
            return (1);
        }
    }
   
    
   
}
