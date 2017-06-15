using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("UtilityAI/Agent")]
public class Agent : MonoBehaviour
{

    public string agentName;
    public bool consoleLogging = false;
    


    public LinkedActionBehaviour linkedRootAction = new LinkedActionBehaviour();




    public int historyStates = 10;
    public float secondsBetweenEvaluations = 0.0f;
    //[HideInInspector]
    public List<LinkedActionBehaviour> linkedRootActions = new List<LinkedActionBehaviour>();
    [HideInInspector]
    public List<string> actionHistory = new List<string>();
    //[HideInInspector]
    public float actionTimer = 0.0f;
    [HideInInspector]
    public bool newAction;

    private ActionBehaviour previousAction, topAction;
    private float currentActionScore;
    private bool isTiming = true;
    private bool paused = false;
    private int topLinkedActionIndex;

    


    public void DisableAction(string actionName)
    {
        for (int i = 0; i < linkedRootActions.Count; i++)
        {
            if (linkedRootActions[i].action.name == actionName)
            {
                linkedRootActions[i].isActionEnabled = false;
                linkedRootActions[i].action.SetActionScore(0.0f);

                if (consoleLogging)
                    Debug.Log(agentName + ". Action Disabled: " + actionName);
            }
        }
    }

    public void UpdateAI()
    {
        if (paused)
        {
            Debug.Log("paused");
            return;
        }    

        if (isTiming)
        {
            actionTimer -= UtilityTime.time;
        }

        if (topAction == null)
        {
            Evaluate();
            actionTimer = GetTopAction().time; 
        }
        else
        {
            //Begin timing the action
            StartTimer();
        }

        //Performs the action as specified in Character.cs, updates the agent parameters etc as specified
        GetTopAction().handle();
        
        

        if (actionTimer <= 0.0f)
        { // action ended

            Debug.Log("action ended");

            
            StopTimer();
            Evaluate();
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



    public float Evaluate()
    {

        Debug.Log("Evaluating");

        if (topAction != null)
            previousAction = topAction;

        float topActionScore = 0.0f;


        for (int i = 0; i < linkedRootActions.Count; i++)
        {
            if (linkedRootActions[i].isActionEnabled == true)
            {
                linkedRootActions[i].action.EvaluateActionUtil();
                if (linkedRootActions[i].action.GetActionScore() > topActionScore)
                {
                    topAction = linkedRootActions[i].action;
                    topActionScore = linkedRootActions[i].action.GetActionScore();
                    topLinkedActionIndex = i;
                }
            }
        }

        if (topAction != previousAction)
            newAction = true;
        else
            StartTimer();


        actionHistory.Add(topAction.name);

        if (actionHistory.Count > historyStates)
        {
            actionHistory.RemoveAt(0);
        }

        if (linkedRootActions[topLinkedActionIndex].cooldown > 0.0f)
        {
            DisableAction(linkedRootActions[topLinkedActionIndex].action.name);
            StartCoroutine(CooldownAction(topLinkedActionIndex));
        }



        if (consoleLogging)
            Debug.Log(agentName + ". New topAction: " + topAction.name + ". With actionScore: " + topActionScore);

        currentActionScore = topActionScore;
        return topActionScore;
    }



    public ActionBehaviour GetTopAction()
    {
        return topAction;
    }

    IEnumerator CooldownAction(int i)
    {
        while (linkedRootActions[i].cooldown >= linkedRootActions[i].cooldownTimer)
        {
            linkedRootActions[i].cooldownTimer += UtilityTime.time;
            yield return new WaitForEndOfFrame();
        }
        linkedRootActions[i].isActionEnabled = true;
        linkedRootActions[i].cooldownTimer = 0.0f;
    }







    #region Setting Action Delegates

    //Sets delegates for every action in the agent's decision hierarchy

    public void SetVoidActionDelegate(string name, ActionBehaviour.Del del)
    {

        for (int i = 0; i < linkedRootActions.Count; i++)
        {
            if (linkedRootActions[i].action.name == name)
            {
                SetActionDelegate(linkedRootActions[i].action, del);
                return;
            }
            else
                CheckActionChildren(linkedRootActions[i].action, name, del);
        }

    }

    public void SetActionDelegate(ActionBehaviour action, ActionBehaviour.Del del)
    {
        action.handle = del;
        return;
    }


    public void CheckActionChildren(ActionBehaviour action, string name, ActionBehaviour.Del del)
    {
        if (action.name == name)
        {
            SetActionDelegate(action, del);
            return;
        }

        else if (!action.isLeafAction)
        {
            for (int i = 0; i < action.linkedChildActions.Count; i++)
                CheckActionChildren(action.linkedChildActions[i].action, name, del);
        }

        else
            return;
    }

    #endregion

   
}
