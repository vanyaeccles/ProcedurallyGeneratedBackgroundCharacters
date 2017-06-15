using UnityEngine;
using System;

[Serializable]
public class ActionConsideration
{
    public AnimationCurve utilityCurve;
    public AgentStateParameter agentStatePar;


    public float weight = 1.0f;
    public bool enabled = true;



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
}
