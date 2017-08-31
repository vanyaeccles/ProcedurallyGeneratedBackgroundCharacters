using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The agent class runs the action hierarchy + sets the delegates for performing actions

public class Agent : MonoBehaviour
{
    [HideInInspector]
    public string agentName;

    public bool consoleLogging = false;
    
    public ActionBehaviour linkedRootAction;

    public ActionBehaviour socialInteruption;

    


    public void Awake()
    {
        agentName = name;
    }

    public void SetSocialInterruption(/*ActionBehaviour arg*/)
    {
        //link up the social interruption action
        foreach (ActionBehaviour action in linkedRootAction.linkedChildActions)
        {
            if (action.name == "Socialise")
                socialInteruption = action;
        }
        //socialInteruption = arg;
    }



    public void SetRootAction(ActionBehaviour arg)
    {
        linkedRootAction = arg;
    }

    public void UpdateAI()
    {
        // Performs the 'exist' action
        linkedRootAction.UpdateAction();
    }






    #region Setting Action Delegates

    //Sets delegates for every action in the agent's decision hierarchy

    public void SetVoidActionDelegate(string name, ActionBehaviour.Del del)
    {
        CheckActionChildren(linkedRootAction, name, del);
    }


    public void SetActionDelegate(ActionBehaviour action, ActionBehaviour.Del del)
    {
        action.handle = del;
        return;
    }

    public void CheckActionChildren(ActionBehaviour action, string _name, ActionBehaviour.Del del)
    {

        if (action == null)
        {
            Debug.Log("Bingo " + action + _name);
        }
            

        if (action.GetName() == _name)
        {
            SetActionDelegate(action, del);
            return;
        }

        else if (!action.isLeafAction)
        {
            for (int i = 0; i < action.linkedChildActions.Count; i++)
                CheckActionChildren(action.linkedChildActions[i], _name, del);
        }

        else
            return;
    }

    #endregion

   
}
