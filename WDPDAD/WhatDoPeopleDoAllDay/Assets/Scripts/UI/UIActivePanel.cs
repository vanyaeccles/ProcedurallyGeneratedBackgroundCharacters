using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActivePanel : MonoBehaviour {


    public Character activeCharacter;

    //text for displaying details
    public Text AgentName;
    public Text StateText;
    public Text HistoryText;


    public bool isDisplayingAgentHistory = false;
    public bool isDisplayingAgentState = false;
    public bool isDisplayingAgentAction = false;

    public GameObject HistoryPanel;
    public GameObject StatePanel;
    public GameObject ActionPanel;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        SetAgent(activeCharacter);
	}

   
    public void SetAgent(Character _character)
    {
        //Debug.Log(_character);

        activeCharacter = _character;

        // display the agent's name
        SetName(_character.name);


        if(isDisplayingAgentState)
        {
            SetState(_character.GetStats());
        }

        if(isDisplayingAgentHistory)
        {
            SetHistory();
        }

        if (isDisplayingAgentAction)
        {

        }

    }




    void SetName(string name)
    {
        AgentName.text = "Name: " + name;
    }

    void SetState(List<AgentStateVarFloat> stateVector)
    {
        StateText.text = (" Energy: " + stateVector[0].value.ToString("F0")
            + "\n Hunger: " + stateVector[1].value.ToString("F0")
            + "\n Resources: " + stateVector[2].value.ToString("F0")
            + "\n Wealth: " + stateVector[3].value.ToString("F0")
            + "\n Mood: " + stateVector[4].value.ToString("F0")
            + "\n Temper: " + stateVector[5].value.ToString("F0")
            + "\n Sociability: " + stateVector[6].value.ToString("F0")
            + "\n Soberness: " + stateVector[7].value.ToString("F0"));
    }

    void SetHistory()
    {   // @TODO could maybe be optimised
        HistoryText.text =  "  Action         StartTime         EndTime \n";

        foreach(AgentLog i in activeCharacter.behaviourLog)
        {
            HistoryText.text += "\n  " + i.action + "     " + i.startTime.ToString("F1") + "            " + i.endTime.ToString("F1");
        }
    }









    #region MODE SELECTION

    // called by the buttons in the active panel
    public void ModeSelect(string modeSelect)
    {
        switch (modeSelect)
        {
            case "History":
                DeactivateModes();
                ActivateHistoryPanel();
                break;
            case "State":
                DeactivateModes();
                ActivateStatePanel();
                break;
            case "Action":
                DeactivateModes();
                ActivateActionPanel();
                break;
        }
    }

    void ActivateHistoryPanel()
    {
        isDisplayingAgentHistory = true;
        HistoryPanel.SetActive(true);
    }

    void ActivateStatePanel()
    {
        isDisplayingAgentState = true;
        StatePanel.SetActive(true);
    }

    void ActivateActionPanel()
    {
        isDisplayingAgentAction = true;
        ActionPanel.SetActive(true);
    }






    void DeactivateModes()
    {
        isDisplayingAgentHistory = false;
        HistoryPanel.SetActive(false);

        isDisplayingAgentState = false;
        StatePanel.SetActive(false);

        isDisplayingAgentAction = false;
        ActionPanel.SetActive(false);
    }

    #endregion


}
