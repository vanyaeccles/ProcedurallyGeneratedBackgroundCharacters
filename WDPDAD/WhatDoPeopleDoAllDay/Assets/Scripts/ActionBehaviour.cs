using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBehaviour : MonoBehaviour
{

    public string actionName;
    public float utilityValueTotal;


    public bool isEvaluating;

    
    public List<ActionConsiderations> actionConsiderations;





    #region Utility Evaluation

    public float EvaluateUtility(Agent agent)
    {
        EvaluateActionConsiderations(agent);

        return utilityValueTotal;
    }

    //Evaluate overall utility for the action
    public void EvaluateActionConsiderations(Agent agent)
    {
        resetUtilityTotal();

        int numbEvaluatedConsiderations = 0;
        
        for(int i = 0; i < actionConsiderations.Count; i++)
        {
            if(actionConsiderations[i].enabled)
            {
                utilityValueTotal += actionConsiderations[i].utilityValue * actionConsiderations[i].considerationWeight;
                numbEvaluatedConsiderations++;
            }
        }

        utilityValueTotal /= numbEvaluatedConsiderations;
    }

    private void resetUtilityTotal()
    {
        utilityValueTotal = 0.0f;
    }

    #endregion







    #region Behaviour

    #endregion
}
