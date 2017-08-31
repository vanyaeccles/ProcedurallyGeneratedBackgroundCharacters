using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the interruption system, currently limited to a single 'socialise' interruption

public class Interrupt : MonoBehaviour {

    public bool isDebugging;
    public bool isActive = false;

    public Agent thisAgent;
    public Personality thisAgentPersonality;
    public Character thisCharacter;

    bool isSocial = true;
    bool isSeekingAssist = false;

    Interrupt interruptSender;
    Interrupt previousInterrupt;


	


    public void ReceiveInterrupt(Interrupt sender, string origin, bool social, bool assist)
    {
        if(isDebugging)
            Debug.Log("Interuption from: " + origin + " of type " + (social ? "social" : "") + (assist ? "assist" : ""));


        // check if already engaged in a social interaction
        if (interruptSender != null)
            return; 


        if (social)
        {
            // if its the last person the agent spoke to then ignore
            if (sender == previousInterrupt)
                return;

            bool acceptSocial = ProcessSocial(origin);

            if (acceptSocial)
            {
                interruptSender = sender;
                previousInterrupt = sender;


                if(isDebugging)
                    Debug.Log("My name: " + name + " sender: " + sender.name);


                // tell the sender to initialise social action 
                sender.Socialise(this);
                // execute social action
                Socialise(sender);
            }
        }



        if(assist)
        {
            // 'assist' interrupt actions not yet implemented
        }
    }




    // this method decides whether to proceed with the social interaction
    bool ProcessSocial(string origin)
    {
        // get the relevant relationship parameter
        AgentStateParameter relationshipInQuestion = thisAgentPersonality.GetRelationship(origin);
        // set the relationship for consideration
        thisAgent.socialInteruption.considerations[0].agentStatePar = relationshipInQuestion;

        // calculate the utility
        thisAgent.socialInteruption.EvaluateActionUtil();

        if (isDebugging)
            Debug.Log("Social interaction score: " + thisAgent.socialInteruption.GetActionScore());

        // if the utility of socialising beats the current utility score, then socialise
        if (thisAgent.socialInteruption.GetActionScore() >= thisCharacter.behaviourLog[thisCharacter.behaviourLog.Count - 1].score)
        {
            // set relationship variable for all social actions
            foreach (ActionBehaviour socialAction in thisAgent.socialInteruption.linkedChildActions)
            {
                socialAction.considerations[0].agentStatePar = relationshipInQuestion;
            }

            return true;
        }
            
        else
            return false;
    }



    void Socialise(Interrupt sender)
    {

        // pause action hierarchy, store original topaction @TODO

        if (isDebugging)
            Debug.Log("Social interaction initiated");

        thisAgent.linkedRootAction.TopAction = null;

        //set 'location' of social interaction
        thisAgent.socialInteruption.location = sender.transform;

        // set the new top action
        thisAgent.linkedRootAction.TopAction = thisAgent.socialInteruption;
        thisAgent.linkedRootAction.ResetTimer();
    }


    #region Interruption begin/end (collision enter/exit)

    // send interuption
    void OnTriggerEnter(Collider c)
    {

        if (!isActive)
            return;


        if (c.gameObject.tag == "Agent")
        {
            CheckStatus();

            bool allowInterrupt = ProcessSocial(c.GetComponent<Interrupt>().name);

            if (allowInterrupt)
            {
                c.gameObject.GetComponent<Interrupt>().ReceiveInterrupt(this, this.name, isSocial, isSeekingAssist);
            }


            if (isDebugging)
                Debug.Log("interuption sent");
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.GetComponent<Interrupt>() == interruptSender)
        {
            // no longer socially occupied
            interruptSender = null;
        }
    }

    #endregion



    void CheckStatus()
    {
        // if seeking assistance (eg if agent is incapacitated), disable the social option

        if (isSeekingAssist)
            isSocial = false;
        else
            isSocial = true;
    }


    #region sending and receiving social interactions

    public void SendNiceInteraction()
    {
        if (interruptSender == null)
            return;
        //thisCharacter.SetFocusPoint(interruptSender.transform);
        interruptSender.GetNiceInteraction(thisCharacter.name);
    }

    public void SendMeanInteraction()
    {
        if (interruptSender == null)
            return;
        //thisCharacter.SetFocusPoint(interruptSender.transform);
        interruptSender.GetMeanInteraction(thisCharacter.name);
    }

   

    public void GetNiceInteraction(string name)
    {
        bool mean = false;

        thisAgentPersonality.ReceiveSocial(name, mean);
    }
    public void GetMeanInteraction(string name)
    {
        bool mean = true;

        thisAgentPersonality.ReceiveSocial(name, mean);
    }

    #endregion

}
