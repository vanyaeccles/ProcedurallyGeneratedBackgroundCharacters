using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
[AddComponentMenu("UtilityAI/Action")]
public class ActionBehaviour : MonoBehaviour
{

    public float time;
    public delegate void Del();
    public Del handle;
    public int priorityLevel;
    public bool interruptible;

    //appropriate weighted considerations
    public List<ActionConsideration> considerations = new List<ActionConsideration>();

    private float actionUtilScore;



    // Parent Action fields
    public bool isLeafAction = false;
    public List<LinkedActionBehaviour> linkedChildActions = new List<LinkedActionBehaviour>();




   

    #region Utility Evaluation

    //Evaluate overall utility for the action
    public void EvaluateActionUtil()
    {
        actionUtilScore = 0.0f;
        int enabledConsiderationsCount = 0;
        //evaluate appropriate considerations
        for (int i = 0; i < considerations.Count; i++)
        {
            //calc utility score if the consideration is enabled
            if (considerations[i].enabled)
            {
                actionUtilScore += considerations[i].evaluateConsiderationUtil * considerations[i].weight;
                enabledConsiderationsCount++;
            }
        }
        //determine average from accumulated utility score
        actionUtilScore = actionUtilScore / enabledConsiderationsCount;
    }
   

    public float GetActionScore()
    {
        return actionUtilScore;
    }
    public void SetActionScore(float val)
    {
        actionUtilScore = val;
    }

    #endregion



    #region Behaviour

    public void ExecuteBehaviour()
    {

        //Each action could have a destination or a set of animations

        // Could also have an effect on the agent's state parameters, see Character script

    }


    #endregion


}
