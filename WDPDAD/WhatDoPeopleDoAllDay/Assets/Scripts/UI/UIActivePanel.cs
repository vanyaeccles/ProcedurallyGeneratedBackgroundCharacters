using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActivePanel : MonoBehaviour {

    private DayNightCycle clock;

    public Character activeCharacter;

    //text for displaying details
    public Text AgentName;
    public Text StateText;
    public Text HistoryText;
    public Text ActionText;


    public bool isDisplayingAgentHistory = false;
    public bool isDisplayingAgentState = false;
    public bool isDisplayingAgentAction = false;

    public GameObject HistoryPanel;
    public GameObject StatePanel;
    public GameObject ActionPanel;

    public void Awake()
    {
        clock = GameObject.Find("Sun").GetComponent<DayNightCycle>();
    }

	
	// Update is called once per frame
	void Update ()
    {
        //SetAgent(activeCharacter);
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
            SetAction();
        }
    }




    void SetName(string name)
    {
        AgentName.text = "Name: " + name + "\n" + "Current Time: " + clock.timeOfDay.ToString("F0");
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
    {   // @TODO could maybe be optimised
        HistoryText.text =  "  Action         StartTime         EndTime \n";

        foreach(AgentLog i in activeCharacter.behaviourLog)
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
