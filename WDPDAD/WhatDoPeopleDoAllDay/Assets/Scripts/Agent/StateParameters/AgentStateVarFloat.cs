using UnityEngine;
using System.Collections;


//This defines a state variable for an NPC, ie hunger, energy, money



public class AgentStateVarFloat : AgentStateParameter
{

    public float minValue = 0.0f;
    public float maxValue = 100.0f;
    public float startValue = 50.0f;
    //public float ChangePerSec = 0.0f;
    public float currentValue;

    void Start()
    {
        currentValue = startValue;
    }


    public float value
    {
        get { return currentValue; }
        set
        {
            currentValue = value;
            if (currentValue < minValue)
                currentValue = minValue;
            if (currentValue > maxValue)
                currentValue = maxValue;
            normValue = (currentValue - minValue) / (maxValue - minValue);
        }
    }


    public AgentStateVarFloat()
    {
        currentValue = startValue;
    }
}
