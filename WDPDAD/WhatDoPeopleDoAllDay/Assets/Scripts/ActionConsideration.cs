using UnityEngine;
using System;

[Serializable]
public class ActionConsideration
{
    public AnimationCurve utilityCurve;
    public bool enabled = true;

    // provided as text by the designer, will be linked (based on name) to agent state parameter
    public string ConsiderationParameter;
    public AgentStateParameter agentStatePar;


    // These will be agent specific
    // owner is set by the action behaviour the consideration is assigned to
    public Character owner;
    // this will be a function of agent personality
    public float weight;
    


    public void Awake()
    {
        weight = 1.0f;
    }


    public float propertyScore
    {
        get
        {
            return agentStatePar.normalizedValue;
        }
    }

    //Returns a score from the utility animation curve
    public float evaluateConsiderationUtil
    {
        get
        {
            return utilityCurve.Evaluate(agentStatePar.normalizedValue);
        }
    }


    public void GetConsiderationWeight()
    {

    }
}
