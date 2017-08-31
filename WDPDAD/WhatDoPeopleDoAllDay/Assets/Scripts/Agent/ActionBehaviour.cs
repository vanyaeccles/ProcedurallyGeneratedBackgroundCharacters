using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


// this is an action behaviour, a general class for all actions in the action hierarchy
// has considerations, child actions and calculates neccessary utility scores
// if the leaf action boolean is set to true, it will run its action delegate, having an effect on the agent

[Serializable]
public class ActionBehaviour : MonoBehaviour
{
    public bool isActionEnabled = true;


    // Parent Action fields
    public bool isRootAction = false;
    public bool isLeafAction = false;

    private float actionUtilScore;
    public float ActionDuration;
    public delegate void Del();
    public Del handle;
    //public int priorityLevel;
    public bool isInterruptible;
    public bool isConsoleLogging;
    public int historyStates = 10;
    public float secondsBetweenEvaluations = 0.0f;

    // for action addition, in order to specify agent-specific action location
    public bool isSelfAction;
    public bool isHomeAction;

    

    private ActionBehaviour previousAction, topAction;
    private float currentActionScore, topActionScore;
    private bool isTiming = true;
    private bool isPaused = false;
    private int topLinkedActionIndex;



    [HideInInspector]
    public List<string> actionHistory = new List<string>();
    //[HideInInspector]
    public float ActionTimer = 0.0f;
    [HideInInspector] // the new action thats been chosen
    public bool newAction;



    //appropriate weighted considerations
    public List<ActionConsideration> considerations = new List<ActionConsideration>();
    //child actions
    public List<ActionBehaviour> linkedChildActions = new List<ActionBehaviour>();



    //Behaviour
    private float Distance2DestinationThresh = 3.0f;
    private Character owner;
    public Transform location;



    #region SETUP

    // this function must be called when creating an agent
    public void StartAwake()
    {

        owner = FindOwner(this.gameObject);

        // Sets the owner for all considerations so they can set their weight value
        for (int i = 0; i < considerations.Count; i++)
        {
            considerations[i].owner = this.owner;

            // set the state parameter
            considerations[i].agentStatePar = owner.personality.stateParameterDictionary[considerations[i].ConsiderationParameter];

            considerations[i].SetWeight();
        }

        GetLocation();
    }


    // get the location from the character
    void GetLocation()
    {
        // if its a 'self action' then can be performed at the agents current location
        if (isSelfAction)
            location = owner.transform;

        if (isHomeAction)
            location = owner.homeLocation;
    }


    public static Character FindOwner(GameObject currentObject)
    {
        //Finds the owner of the action behaviour
        Transform c = currentObject.transform;

        while (c.parent != null)
        {
            if (c.parent.tag == "Agent")
            {
                return c.parent.gameObject.GetComponent<Character>();
            }

            c = c.parent.transform;
        }
        return null;
    }


    #endregion




    public void UpdateAction()
    {
        if (isLeafAction) // leaf actions don't update themselves
            return;

        if (isPaused)
        {
            Debug.Log("paused");
            return;
        }


        if (isTiming)
        {
            ActionTimer -= UtilityTime.time;
        }


        if (topAction == null)
        {   
            // evaluate + choose a child action
            EvaluateChildActions();

            ActionTimer = topAction.GetTime();

            // for the UI agent log
            if (topAction.isLeafAction)
            {
                // log the new action
                owner.LogActionBegin(topAction);
            }
        }



        //This is where the child actions are simultaneously told to perform their own evaluations
        topAction.UpdateAction();



        if(isConsoleLogging)
            Debug.Log(topAction + " running");




        // Performs the action
        ExecuteBehaviour();
        


 

        if (ActionTimer <= 0.0f) 
        { // action ended

            // for the UI agent log
            if (topAction.isLeafAction)
                owner.LogActionEnd();


            if(isConsoleLogging)
                Debug.Log(TopAction.name + " action ended");


            topAction = null;

            StopTimer();



            //EvaluateChildActions();

            //actionTimer = TopAction.time;
        }
    }


    

   


    #region Utility Evaluation

    //Evaluate overall utility for the action
    public void EvaluateActionUtil()
    {
        actionUtilScore = 0.0f;
        int enabledConsiderationsCount = 0;


        // if there are no considerations then just pick the action and execute
        if (considerations.Count == 0)
        {
            //Debug.Log("No considerations");
            actionUtilScore = 1.0f;
            return;
        }

        //else evaluate appropriate considerations 
        for (int i = 0; i < considerations.Count; i++)
        {
            //calc utility score if the consideration is enabled
            if (considerations[i].enabled)
            {
                actionUtilScore += considerations[i].evaluateConsiderationUtil * considerations[i].Weight;
                enabledConsiderationsCount++;
            }
        }
        //determine average from accumulated utility score
        actionUtilScore = actionUtilScore / enabledConsiderationsCount;
    }


    //Takes a look at child actions and picks the one with best utility score
    public float EvaluateChildActions()
    {

        if (topAction != null)
            previousAction = topAction;

        topActionScore = -10.0f;


        for (int i = 0; i < linkedChildActions.Count; i++)
        {
            if (linkedChildActions[i].isActionEnabled == true)
            {

                linkedChildActions[i].EvaluateActionUtil();

                if (isConsoleLogging)
                    Debug.Log("Evaluating " + linkedChildActions[i].GetName() + " Score: " + linkedChildActions[i].GetActionScore());

                if (linkedChildActions[i].GetActionScore() >= topActionScore)
                {
                    topAction = linkedChildActions[i];
                    topActionScore = linkedChildActions[i].GetActionScore();
                    topLinkedActionIndex = i;
                }
            }
        }


        if (isConsoleLogging)
            Debug.Log(name + ". New topAction: " + topAction.name + ". With actionScore: " + topActionScore);



        if (topAction == null)
        {
            //EvaluateChildActions();
            topAction = linkedChildActions[0];
            Debug.Log(owner.name + "No top action");
        }




        currentActionScore = topActionScore;
        return topActionScore;
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
        //Debug.Log("Executing: " + name);



        if (isLeafAction)
            return;


        if (TopAction.isLeafAction)
        {
            // set the target for navigation
            owner.SetTarget(TopAction.location);
        }
            

        //checks position + begins performing action if close enough
        float distance2target = Vector3.Distance(owner.transform.position, TopAction.location.transform.position);
        if (distance2target <= Distance2DestinationThresh)
        {
            StartTimer();

            //Performs the action as specified in Character.cs, updates the agent parameters etc as specified
            if (TopAction.isLeafAction)
            {
                // run the delegate
                TopAction.handle();
                //TopAction.ExecuteBehaviour();
            }   
        }
    }


    #endregion


    #region TIMER

    public void StartTimer()
    {
        isTiming = true;
    }

    public void StopTimer()
    {
        isTiming = false;
    }

    public void ResetTimer()
    {
        ActionTimer = TopAction.ActionDuration;
    }

    public float GetTime()
    {
        return ActionDuration;
    }

    #endregion





    #region MISC

    public ActionBehaviour TopAction
    {
        get
        {
            return topAction;
        }

        set
        {
            previousAction = topAction;
            topAction = value;
        }

    }




    public string GetName()
    {
        return name;
    }

    public void DisableAction(string actionName)
    {
        for (int i = 0; i < linkedChildActions.Count; i++)
        {
            if (linkedChildActions[i].name == actionName)
            {
                linkedChildActions[i].isActionEnabled = false;
                linkedChildActions[i].SetActionScore(0.0f);
            }
        }
    }

    #endregion

}
