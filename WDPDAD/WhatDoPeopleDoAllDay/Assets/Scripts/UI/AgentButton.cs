using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentButton : MonoBehaviour
{
    // references to important objects
    public UIAgentManager agentManager;
    GameObject thisButtonAgent;

    // this button details
    Button thisButton;
    Text agentName;


    void Awake()
    {
        agentName = GetComponentInChildren<Text>();
        thisButton = GetComponent<Button>();
    }



    public void SetAgent(GameObject _agent)
    {
        //set the gameobject
        thisButtonAgent = _agent;
        //set the name text
        agentName.text = _agent.name;
        //set the on click event
        thisButton.onClick.AddListener(SetAgentInAgentManager);
    }

	void SetAgentInAgentManager()
    {
        // let the UI agent manager know that this agent is to be set as active agent
        agentManager.SetActiveAgent(thisButtonAgent);
    }


}
