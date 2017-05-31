using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConsiderations : MonoBehaviour
{

    public float utilityValue; // value returned for action utility evaluation
    public float considerationWeight; // value that weighs in on the agent's personal evaluation of this consideration, maybe stored in the agent instead? @TODO
    


    public float calculateUtility(int funcType, float normalizedInput)
    {
        // use the function type to specify the function, feed in the normalized input value and get the result

        return utilityValue;
    }
	


    // Possibly don't need this @TODO
    public float normalizeUtility(float util)
    { 
        return util;
    }
}
