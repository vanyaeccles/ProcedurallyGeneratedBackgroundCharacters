using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActivePanel : MonoBehaviour {


    public Character activeCharacter;

    //text for displaying details
    public Text AgentName;
    public Text StateText;


    public bool isDisplayingAgentHistory = false;
    public bool isDisplayingAgentState = false;
    public bool isDisplayingAgentAction = false;


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



        SetState(_character.GetStats());

    }




    void SetName(string name)
    {
        AgentName.text = "Agent Name: " + name;
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


}
