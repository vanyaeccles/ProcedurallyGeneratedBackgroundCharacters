using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("UtilityAI/Agent")]
public class Agent : MonoBehaviour
{
    [HideInInspector]
    public string agentName;

    public bool consoleLogging = false;
    
    public LinkedActionBehaviour linkedRootAction = new LinkedActionBehaviour();

    public LinkedActionBehaviour socialInteruption = new LinkedActionBehaviour();

    


    public void Awake()
    {
        agentName = name;
    }


    public void UpdateAI()
    {
        // Performs the 'exist' action
        linkedRootAction.action.UpdateAction();
    }


    public void StoreAction()
    {

    }





    #region Setting Action Delegates

    //Could be moved somewhere? What do I want to keep in 'agent'? @TODO

    //Sets delegates for every action in the agent's decision hierarchy

    public void SetVoidActionDelegate(string name, ActionBehaviour.Del del)
    {
         CheckActionChildren(linkedRootAction.action, name, del);
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
