using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// This manages the Active Panel UI, when an agent is selected it will load that agents history, state and current action info

public class UIActivePanel : MonoBehaviour {

    public DayNightCycle clock;

    public Character activeCharacter;

    //UI gameobjects for holding text
    public GameObject agent;
    public GameObject state;
    public GameObject history;
    public GameObject action;
    

    //text for displaying details
    private Text AgentName;
    private Text StateText;
    private Text HistoryText;
    private Text ActionText;


    public bool isDisplayingAgentHistory = false;
    public bool isDisplayingAgentState = false;
    public bool isDisplayingAgentAction = false;

    public GameObject HistoryPanel;
    public GameObject StatePanel;
    public GameObject ActionPanel;

    public void Awake()
    {
        AgentName = agent.GetComponent<Text>();
        HistoryText = history.GetComponent<Text>();
        StateText = state.GetComponent<Text>();
        ActionText = action.GetComponent<Text>();
    }

	
	// Update is called once per frame
	void Update ()
    {
        if(activeCharacter != null)
        {
            SetAgent(activeCharacter);
        }
            
	}

   
    public void SetAgent(Character _character)
    {

        activeCharacter = _character;



        if (isDisplayingAgentState)
        {
            SetState(_character.GetStats());
        }

        if(isDisplayingAgentHistory)
        {
            SetHistory();
        }

        if (isDisplayingAgentAction)
        {
            SetAction();
        }
    }




    void SetName(string _name)
    {
        AgentName.text = "Name: " + _name + "\n" + "Current Time: " + clock.timeOfDay.ToString("F0");
    }

    // displays the agent state in the state panel
    void SetState(List<AgentStateVarFloat> stateVector)
    {
        StateText.text = "Parameter               Weight";

        StateText.text += ("\n Energy: " + stateVector[0].value.ToString("F0") + "          " + activeCharacter.personality.CheckWeight("energy")
            + "\n Hunger: " + stateVector[1].value.ToString("F0") + "          " + activeCharacter.personality.CheckWeight("hunger")
            + "\n Resources: " + stateVector[2].value.ToString("F0") + "          " + activeCharacter.personality.CheckWeight("resources")
            + "\n Wealth: " + stateVector[3].value.ToString("F0") + "          " + activeCharacter.personality.CheckWeight("wealth")
            + "\n Mood: " + stateVector[4].value.ToString("F0") + "          " + activeCharacter.personality.CheckWeight("mood")
            + "\n Temper: " + stateVector[5].value.ToString("F0") + "          " + activeCharacter.personality.CheckWeight("temper")
            + "\n Sociability: " + stateVector[6].value.ToString("F0") + "          " + activeCharacter.personality.CheckWeight("sociability")
            + "\n Soberness: " + stateVector[7].value.ToString("F0") + "          " + activeCharacter.personality.CheckWeight("soberness"));

        StateText.text += "\n \n Personality: ";

        StateText.text += ("\n \n Openess: " + activeCharacter.personality.OpennessToExperience
            + "\n Concientiousness: " + activeCharacter.personality.Concientiousness
            + "\n Extroversion: " + activeCharacter.personality.Extroversion
            + "\n Agreeableness: " + activeCharacter.personality.Agreeableness
            + "\n Neuroticism: " + activeCharacter.personality.Neuroticism
            );


        StateText.text += "\n\n Relationships:  \n \n";

        foreach (Relationship rel in activeCharacter.personality.AgentRelationships)
            StateText.text += "  " + rel.nameOfPerson + " " + rel.relationshipValue.value.ToString("F0") + "\n";
    }


    // displays the agent history in the history panel
    void SetHistory()
    {   
        HistoryText.text =  "  Action         StartTime         EndTime \n";

        HistoryText.text += "Total Number of Actions Performed: " + activeCharacter.behaviourLog.Count + "\n";

        foreach (AgentLog i in activeCharacter.behaviourLog)
        {
            HistoryText.text += "\n  " + i.action + "     " + i.startTime.ToString("F1") + "            " + i.endTime.ToString("F1");
        }
    }

    void SetAction()
    {
        ActionText.text = "";

        foreach (string i in activeCharacter.runningActions)
            ActionText.text += i + "\n";
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





    // called when modes are changed
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
