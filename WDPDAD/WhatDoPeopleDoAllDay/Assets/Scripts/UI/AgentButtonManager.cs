﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentButtonManager : MonoBehaviour
{
    // the generic button object
    public GameObject AgentButtonTemplate;

    public UIAgentManager agentManager;
	


    public void CreateNewAgentButton(GameObject _agent)
    {
        // makes a new button
        GameObject newAgentButton = Instantiate(AgentButtonTemplate, this.transform.position, Quaternion.identity);
        newAgentButton.transform.parent = this.transform;


        // a reference to the button script
        AgentButton newAgentButtonInfo = newAgentButton.GetComponent<AgentButton>();

        //set the important button details
        newAgentButtonInfo.agentManager = this.agentManager;
        newAgentButtonInfo.SetAgent(_agent);
        

    }

}
