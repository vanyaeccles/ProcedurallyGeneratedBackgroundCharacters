using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
[AddComponentMenu("UtilityAI/Action")]
public class ActionBehaviour : MonoBehaviour
{
    // Parent Action fields
    public bool isRootAction = false;
    public bool isLeafAction = false;

    private float actionUtilScore;
    public float time;
    public delegate void Del();
    public Del handle;
    public int priorityLevel;
    public bool interruptible;

    public int historyStates = 10;
    public float secondsBetweenEvaluations = 0.0f;

    private ActionBehaviour previousAction, topAction;
    private float currentActionScore, topActionScore;
    private bool isTiming = true;
    private bool paused = false;
    private int topLinkedActionIndex;

    //[HideInInspector]
    //public List<LinkedActionBehaviour> linkedRootActions = new List<LinkedActionBehaviour>();
    [HideInInspector]
    public List<string> actionHistory = new List<string>();
    //[HideInInspector]
    public float actionTimer = 0.0f;
    [HideInInspector]
    public bool newAction;



    //appropriate weighted considerations
    public List<ActionConsideration> considerations = new List<ActionConsideration>();

    public List<LinkedActionBehaviour> linkedChildActions = new List<LinkedActionBehaviour>();


    void Awake()
    {

    }




    
    public void UpdateAction()
    {
        if (paused)
        {
            Debug.Log("paused");
            return;
        }

        if (isLeafAction)
            return;

        if (isTiming)
        {
            actionTimer -= UtilityTime.time;
        }

        if (topAction == null)
        {
            EvaluateChildActions();
            actionTimer = GetTopAction().time;
        }
        else
        {
            //Begin timing the action
            StartTimer();
        }


        //This is where the child actions are simultaneously told to perform their own evaluations
        topAction.UpdateAction();


        //Performs the action as specified in Character.cs, updates the agent parameters etc as specified
        if (GetTopAction().isLeafAction)
                GetTopAction().handle();


 

        if (actionTimer <= 0.0f) 
        { // action ended

            Debug.Log(name + " action ended");


            StopTimer();

            EvaluateChildActions();

            actionTimer = GetTopAction().time;
        }
    }




    public void StartTimer()
    {
        isTiming = true;
    }

    public void StopTimer()
    {
        isTiming = false;
    }


    //Takes a look at child actions and picks the one with best utility score
    public float EvaluateChildActions()
    {

        Debug.Log("Evaluating " + name + "'s child actions");

        if (topAction != null)
            previousAction = topAction;

        topActionScore = 0.0f;


        for (int i = 0; i < linkedChildActions.Count; i++)
        {
            if (linkedChildActions[i].isActionEnabled == true)
            {
                linkedChildActions[i].action.EvaluateActionUtil();
                if (linkedChildActions[i].action.GetActionScore() > topActionScore)
                {
                    topAction = linkedChildActions[i].action;
                    topActionScore = linkedChildActions[i].action.GetActionScore();
                    topLinkedActionIndex = i;
                }
            }
        }

        if (topAction != previousAction)
            newAction = true;
        else
            StartTimer();

        

        //actionHistory.Add(topAction.name);

        //if (actionHistory.Count > historyStates)
        //{
        //    actionHistory.RemoveAt(0);
        //}

        //if (linkedChildActions[topLinkedActionIndex].cooldown > 0.0f)
        //{
        //    DisableAction(linkedChildActions[topLinkedActionIndex].action.name);
        //    StartCoroutine(CooldownAction(topLinkedActionIndex));
        //}


        


        //if (consoleLogging)
            Debug.Log(name + ". New topAction: " + topAction.name + ". With actionScore: " + topActionScore);

        currentActionScore = topActionScore;
        return topActionScore;
    }



    public ActionBehaviour GetTopAction()
    {
        return topAction;
    }


    public void DisableAction(string actionName)
    {
        for (int i = 0; i < linkedChildActions.Count; i++)
        {
            if (linkedChildActions[i].action.name == actionName)
            {
                linkedChildActions[i].isActionEnabled = false;
                linkedChildActions[i].action.SetActionScore(0.0f);

                //if (consoleLogging)
                //    Debug.Log(agentName + ". Action Disabled: " + actionName);
            }
        }
    }


    IEnumerator CooldownAction(int i)
    {
        while (linkedChildActions[i].cooldown >= linkedChildActions[i].cooldownTimer)
        {
            linkedChildActions[i].cooldownTimer += UtilityTime.time;
            yield return new WaitForEndOfFrame();
        }
        linkedChildActions[i].isActionEnabled = true;
        linkedChildActions[i].cooldownTimer = 0.0f;
    }




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
