using UnityEngine;
using System.Collections;


//This defines a state variable for an NPC, ie hunger, energy, money


[AddComponentMenu("UtilityAI/Float State Variable")]
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

    void Update()
    {
        //This could change automatically, such as when in between actions - or should it? @TODO
        //value += UtilityTime.time * ChangePerSec;
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
}
