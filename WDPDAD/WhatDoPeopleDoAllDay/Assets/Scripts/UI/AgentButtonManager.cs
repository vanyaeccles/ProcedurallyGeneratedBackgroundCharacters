using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This is owned by the agent button grid
// Used to make a new button when the AgentUIManager is told to add a new agent by the Town script


public class AgentButtonManager : MonoBehaviour
{
    // the generic button object
    public GameObject AgentButtonTemplate;

    public UIAgentManager agentManager;

    List<GameObject> agentButtons = new List<GameObject>();


    public void CreateNewAgentButton(GameObject _agent)
    {
        // makes a new button
        GameObject newAgentButton = Instantiate(AgentButtonTemplate, this.transform.position, Quaternion.identity);
        newAgentButton.transform.parent = this.transform;
        agentButtons.Add(newAgentButton);


        // a reference to the button script
        AgentButton newAgentButtonInfo = newAgentButton.GetComponent<AgentButton>();

        //set the important button details
        newAgentButtonInfo.agentManager = this.agentManager;
        newAgentButtonInfo.SetAgent(_agent);
    }

    public void DestroyButtons()
    {
        foreach (GameObject button in agentButtons)
            Destroy(button);
    }



}
