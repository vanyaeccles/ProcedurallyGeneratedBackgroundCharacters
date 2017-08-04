using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this class is used to hold a 'compressed' version of an agent filled town


public class Town : MonoBehaviour
{
    
    // collapsed agent stuff

    // the list of compressed agent 'kernel's
    public List<AgentKernel> compressedAgents = new List<AgentKernel>();




    // expanded agent stuff

    // used to hold all the uncompressed agents when they are instantiated
    List<GameObject> townAgents = new List<GameObject>();
    // the gameobject in the editor hierarchy that is used to hold all the agents
    public GameObject agents;

    public GameObject baseAgent;
    public GameObject baseAction;

    public Transform startLocation;

    private ActionBehaviour tempRootaction;

    public GameObject townExistAction;

    bool single = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey("b") && !single)
        {
            single = true;

            //print("agent instantiated");

            townAgents.Add(Instantiate(baseAgent, startLocation.position, Quaternion.identity));


            GameObject newAgent = townAgents[townAgents.Count - 1];
            newAgent.SetActive(true);

            // set the name
            newAgent.name = "AliveBoy";


            Character newAgentCharacter = newAgent.GetComponent<Character>();



            //set the home
            newAgentCharacter.homeLocation = startLocation;

            //set the occupation
            //newAgentCharacter.occupation = OccupationType.1;

            //set the action hierarchy
            // make an instance of the gameobject, set it as a child of the new agent
            GameObject newActions = Instantiate(baseAction, newAgent.transform.position, Quaternion.identity);
            newActions.SetActive(true);


            newActions.transform.parent = newAgent.transform;

            // get the actions to run their StartAwake function
            newActions.BroadcastMessage("StartAwake");

            tempRootaction = newActions.GetComponent<ActionBehaviour>();

            newAgentCharacter.ActivateAgent(tempRootaction);

        }
            
    }



    #region LOD Physics checking

    //void OnTriggerEnter(Collision collision)
    //{
    //    //expand the agents
    //}

    //void OnTriggerExit(Collision collision)
    //{
    //    //collapse the agents
    //}

    #endregion


}


// the types of job that agents can have
public enum OccupationType
{
    BLACKSMITH, 
    FARMER,
    WOODCUTTER,
    GUARD,
    HUNTER
}
