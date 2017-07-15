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
    //public int priorityLevel;
    public bool isInterruptible;
    public bool isConsoleLogging;

    public int historyStates = 10;
    public float secondsBetweenEvaluations = 0.0f;

    private ActionBehaviour previousAction, topAction;
    private float currentActionScore, topActionScore;
    private bool isTiming = true;
    private bool isPaused = false;
    private int topLinkedActionIndex;


    [HideInInspector]
    public List<string> actionHistory = new List<string>();
    //[HideInInspector]
    public float actionTimer = 0.0f;
    [HideInInspector]
    public bool newAction;



    //appropriate weighted considerations
    public List<ActionConsideration> considerations = new List<ActionConsideration>();
    //child actions
    public List<LinkedActionBehaviour> linkedChildActions = new List<LinkedActionBehaviour>();



    //Behaviour
    private float Distance2DestinationThresh = 5.0f;
    private Character owner;
    public Transform location;




    public void Awake()
    {
        owner = FindOwner(this.gameObject);

        // Sets the owner for all considerations so they can set their weight vector 
        for (int i = 0; i < considerations.Count; i++)
        {
            considerations[i].owner = this.owner;
        }
    }

    


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
            actionTimer -= UtilityTime.time;
        }

        if (topAction == null)
        {   // evaluate + choose a child action
            EvaluateChildActions();
            actionTimer = TopAction.time;
        }
        else
        {
            //Begin timing the action

            //StartTimer();
        }



        //This is where the child actions are simultaneously told to perform their own evaluations
        topAction.UpdateAction();

        if(isConsoleLogging)
            Debug.Log(topAction + " running");




        // Performs the action
        ExecuteBehaviour();
        


 

        if (actionTimer <= 0.0f) 
        { // action ended

            if(isConsoleLogging)
                Debug.Log(TopAction.name + " action ended");

            topAction = null;

            StopTimer();

            EvaluateChildActions();

            actionTimer = TopAction.time;
        }
    }




    


    //Takes a look at child actions and picks the one with best utility score
    public float EvaluateChildActions()
    {

        if (topAction != null)
            previousAction = topAction;

        topActionScore = 0.0f;


        for (int i = 0; i < linkedChildActions.Count; i++)
        {
            if (linkedChildActions[i].isActionEnabled == true)
            {
                
                linkedChildActions[i].action.EvaluateActionUtil();

                if (isConsoleLogging)
                    Debug.Log("Evaluating " + linkedChildActions[i].action.GetName() + " Score: " + linkedChildActions[i].action.GetActionScore());

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
            //StartTimer();

        

        if (isConsoleLogging)
            Debug.Log(name + ". New topAction: " + topAction.name + ". With actionScore: " + topActionScore);

        currentActionScore = topActionScore;
        return topActionScore;
    }


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
        actionTimer = TopAction.time;
    }

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
            if (linkedChildActions[i].action.name == actionName)
            {
                linkedChildActions[i].isActionEnabled = false;
                linkedChildActions[i].action.SetActionScore(0.0f);
            }
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
        //Debug.Log("Executing: " + name);

        //Each action could have a destination or a set of animations

        // Could also have an effect on the agent's state parameters, see Character script


        if (isLeafAction)
            return;


        if (TopAction.isLeafAction)
            owner.SetTarget(TopAction.location);

        if(isConsoleLogging)
        {
            //Debug.Log("Current position: " + transform.position);
            //Debug.Log("Target: " + GetTopAction().name);
            //Debug.Log("Target position: " + GetTopAction().location.transform.position);
            //Debug.Log("Distance: " + Vector3.Distance(owner.transform.position, GetTopAction().location.transform.position));
        }


        //checks position + begins performing action
        float distance2target = Vector3.Distance(owner.transform.position, TopAction.location.transform.position);
        if (distance2target <= Distance2DestinationThresh)
        {
            //Debug.Log("Performing action " + GetTopAction().name);


            StartTimer();

            //Performs the action as specified in Character.cs, updates the agent parameters etc as specified
            if (TopAction.isLeafAction)
            {
                TopAction.handle();
                TopAction.ExecuteBehaviour();
            }   
        }
    }


    #endregion



    #region MISC

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


}
