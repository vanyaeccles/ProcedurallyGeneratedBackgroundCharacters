using UnityEngine;
using System;


// the basic agent state parameter (hunger, energy etc) class


[Serializable]
public class AgentStateParameter : MonoBehaviour
{


    protected float normValue;

    

    public float normalizedValue
    {
        get { return normValue; }
    }


    // constructor
    public AgentStateParameter()
    {
        normValue = 50.0f;
    }
    
}
