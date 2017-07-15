using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("UtilityAI/Agent")]
public class Agent : MonoBehaviour
{

    public string agentName;
    public bool consoleLogging = false;
    
    public LinkedActionBehaviour linkedRootAction = new LinkedActionBehaviour();

    public LinkedActionBehaviour socialInteruption = new LinkedActionBehaviour();



    public void UpdateAI()
    {
        // Performs the 'exist' action
        linkedRootAction.action.UpdateAction();
    }



    public void Interrupt(string origin, bool social, bool assist)
    {
        //Debug.Log("Interuption recieved");

        //Debug.Log("Interuption from: " + origin + " of type " + social + " or " + assist);


        if (social)
        {


            socialInteruption.action.EvaluateActionUtil();

            //Debug.Log(socialInteruption.action.GetActionScore());

            if (socialInteruption.action.GetActionScore() >= 0.0f)
            {
                // pause action hierarchy, store original topaction

                //Debug.Log("Social interaction initiated");

                linkedRootAction.action.TopAction = null;

                // set the new top action
                linkedRootAction.action.TopAction = socialInteruption.action;
                linkedRootAction.action.ResetTimer();

            }
        }
            


    }





    #region Setting Action Delegates

    //Could be moved somewhere? What do I want to keep in 'agent'? @TODO

    //Sets delegates for every action in the agent's decision hierarchy

    public void SetVoidActionDelegate(string name, ActionBehaviour.Del del)
    {

        //for (int i = 0; i < linkedRootActions.Count; i++)
        //{
        //    if (linkedRootActions[i].action.name == name)
        //    {
        //        SetActionDelegate(linkedRootActions[i].action, del);
        //        return;
        //    }
        //    else
                CheckActionChildren(linkedRootAction.action, name, del);
        //}

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
