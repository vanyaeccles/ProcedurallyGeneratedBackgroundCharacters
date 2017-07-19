using UnityEngine;
using System;

[Serializable]
public class ActionConsideration
{
    public AnimationCurve utilityCurve;
    public bool enabled = true;

    // provided as text by the designer, will be linked (based on name) to agent state parameter @TODO
    public string ConsiderationParameter;
    public AgentStateParameter agentStatePar;


    // These will be agent specific
    // owner is set by the action behaviour the consideration is assigned to
    public Character owner;
    // this will be a function of agent personality
    private float weight;
    


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
    

    //retrieves the weight associated with the importance the agent assocates with that state variable
    public float Weight
    {
        get
        {   
            return weight;
        }
        set
        {
            weight = value;
        }
    }


    public void SetWeight()
    {
        Weight = owner.personality.CheckWeight(ConsiderationParameter);
        Debug.Log(Weight);
    }
}
