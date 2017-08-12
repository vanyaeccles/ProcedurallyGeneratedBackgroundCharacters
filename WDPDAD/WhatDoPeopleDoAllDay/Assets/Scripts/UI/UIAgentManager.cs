using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for setting agent + camera via UI buttons

public class UIAgentManager : MonoBehaviour {

    //for managing agent selection buttons
    public AgentButtonManager agentButtonManager;

    // for displaying info
    public UIActivePanel activePanel;

    // list of characters
    public List<Character> characters = new List<Character>();

    //the currently active agent
    private Character currentCharacter;



    // the currently active camera
    public Camera activeCam;
    // the players cam, may get added so the player plays as an agent
    public Camera playerCam;
    // the list of agent cameras
    public List<Camera> cameras = new List<Camera>();
    // a dictionary used to find and activate agent cameras
    public Dictionary<string, Camera> agentCams = new Dictionary<string, Camera>();



    void Awake()
    {
        SetupCameras();
    }

    void Start()
    {

        // start with the player's camera
        SetCamera("player");
    }

	
    public void NewAgent(GameObject _agent)
    {
        //make a new button for the agent
        agentButtonManager.CreateNewAgentButton(_agent);


        Character newChar = _agent.GetComponent<Character>();

        // add the character to the list
        characters.Add(newChar);

        //add the camera to the list
        cameras.Add(newChar.agentCamera);
        agentCams.Add(newChar.name, newChar.agentCamera);

        // keep the player's character until agent is selected
        SetCamera("player");
    }



    // button click event
    // this activates the agent's UIonce the button has been clicked
    public void SetActiveAgent(GameObject agent)
    {

        SetCharacter(agent.GetComponent<Character>());


        // set the agent for displaying info
        activePanel.SetAgent(currentCharacter);


        // set the main camera to follow them
        SetCamera(agent.name);
    }



    void SetCharacter(Character charac)
    {
        currentCharacter = charac;
    }



    #region CAMERA STUFF

    // this is used on awake to build the list + dictionary of cameras
    void SetupCameras()
    {
        //add the main (player) camera
        cameras.Add(playerCam);
        agentCams.Add("player", playerCam);
    }


    // called when selecting agents to follow with camera
    public void SetCamera(string name)
    {
        // disable all cams
        foreach(Camera cam in cameras)
        {
            cam.enabled = false;
        }
        //enable the agent's camera and set it as the active camera
        agentCams[name].enabled = true;
        activeCam = agentCams[name];
    }

    #endregion


}
