using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this class is used to hold a 'compressed' version of an agent filled town


public class Town : MonoBehaviour
{
    
    // collapsed agent stuff

    // the list of compressed agent 'kernel's
    public List<AgentKernel> compressedAgents = new List<AgentKernel>();

    public UIAgentManager agentManagerUI;
    
    


    // expanded agent stuff

    // used to hold all the uncompressed agents when they are instantiated
    public List<GameObject> townAgents = new List<GameObject>();
    // the gameobject in the editor hierarchy that is used to hold all the agents
    public GameObject agents;

    public GameObject baseAgent;
    public GameObject baseAction;

    public Transform startLocation;

    private ActionBehaviour tempRootaction;


    [Header("Occupations")]
    public GameObject Blacksmithing;
    public GameObject Farming;
    public GameObject Woodcutting;
    public GameObject GuardDuty;
    public GameObject Hunting;
    Dictionary<OccupationType, GameObject> occupationDictionary = new Dictionary<OccupationType, GameObject>();

    bool single = false;




    // Use this for initialization
    void Start ()
    {
        // the occupations
        BuildOccupationDictionary();
	}
	



    void UnZipAgent(AgentKernel _kernel)
    {
        //print("agent instantiated");


        // get the compressed agent
        AgentKernel agentKernel = _kernel;

        // instantiate the agent
        townAgents.Add(Instantiate(baseAgent, agentKernel.homeLocation.position, Quaternion.identity));


        GameObject newAgent = townAgents[townAgents.Count - 1];
        newAgent.SetActive(true);


        // build the agent character
        Character newAgentCharacter = newAgent.GetComponent<Character>();

        // build the agent from the given kernel
        newAgentCharacter.name = agentKernel.Name;
        newAgentCharacter.homeLocation = agentKernel.homeLocation;


        //Add the agent to the UI
        agentManagerUI.NewAgent(newAgent);





        //set the action hierarchy
        // make an instance of the gameobject, set it as a child of the new agent
        GameObject newActions = Instantiate(baseAction, newAgent.transform.position, Quaternion.identity);
        newActions.SetActive(true);
        newActions.transform.parent = newAgent.transform;


        // give the agent its base 'exist' action, 
        tempRootaction = newActions.GetComponent<ActionBehaviour>();



        // get + instantiate the object for the agent's occupation
        GameObject occupationObject = Instantiate(occupationDictionary[agentKernel.occupation], newActions.transform.position, Quaternion.identity);

        foreach (ActionBehaviour childAction in tempRootaction.linkedChildActions)
            if (childAction.name == "Work")
            {
                occupationObject.SetActive(true);

                childAction.linkedChildActions.Add(occupationObject.GetComponent<ActionBehaviour>());

                //set the parent object in the hierarchy
                occupationObject.transform.parent = childAction.transform;
            }



        // and build the personality 
        newAgentCharacter.SetupAgent(tempRootaction, agentKernel.OpennessToExperience, agentKernel.Concientiousness, agentKernel.Extroversion, agentKernel.Agreeableness, agentKernel.Neuroticism);



        // get the actions to run their StartAwake function, gives all actions their location and owner
        newActions.BroadcastMessage("StartAwake");


        //add to the town's population
        townAgents.Add(newAgent);





        // finally activate the agent
        newAgentCharacter.SetActive();
    }















    void BuildOccupationDictionary()
    {
        occupationDictionary.Add(OccupationType.Blacksmithing, Blacksmithing);
        occupationDictionary.Add(OccupationType.Farming, Farming);
        occupationDictionary.Add(OccupationType.Woodcutting, Woodcutting);
        occupationDictionary.Add(OccupationType.GuardDuty, GuardDuty);
        occupationDictionary.Add(OccupationType.Hunting, Hunting);
    }


    #region LOD Physics checking

    void OnTriggerEnter(Collider collision)
    {
        //expand the agents
        if (!single)
        {
            single = true;


            foreach (AgentKernel kern in compressedAgents)
                UnZipAgent(kern);
        }
    }



    void OnTriggerExit(Collider collision)
    {
        //collapse the agents

        //foreach (GameObject agent in townAgents)
            //Destroy(agent);

        //@TODO need to destroy agent references in UI as well


        //Debug.Log("THEY ARE GONE");

    }

    #endregion


}





// the types of job that agents can have
public enum OccupationType
{
    Blacksmithing, 
    Farming,
    Woodcutting,
    GuardDuty,
    Hunting
}
