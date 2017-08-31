using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;



// this class is used to hold a 'compressed' version of an agent filled town
// it has an associated sphere collider, agent's are built/destroyed as the player enters/leaves

public class Town : MonoBehaviour
{
    
    // collapsed agent stuff

    // the list of compressed agent 'kernel's
    public List<AgentKernel> compressedAgents = new List<AgentKernel>();

    public UIAgentManager agentManagerUI;
    
    


    // expanded agent stuff

    // used to hold all the uncompressed agents when they are instantiated
    public List<GameObject> townAgents = new List<GameObject>();
    public List<Character> townAgentCharacters = new List<Character>();


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
    // toggle this to either generate agents per frame or not
    public bool isUnzippingPerFrame = true;



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
        // add this to the list of characters
        townAgentCharacters.Add(newAgentCharacter);

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

        for (int i = 0; i < tempRootaction.linkedChildActions.Count; i++)
        {
            ActionBehaviour childAction = tempRootaction.linkedChildActions[i];
            if (childAction.name == "Work")
            {
                occupationObject.SetActive(true);

                childAction.linkedChildActions.Add(occupationObject.GetComponent<ActionBehaviour>());

                //set the parent object in the hierarchy
                occupationObject.transform.parent = childAction.transform;
            }
        }
            



        // and build the personality 
        newAgentCharacter.SetupAgent(tempRootaction, agentKernel.OpennessToExperience, agentKernel.Concientiousness, agentKernel.Extroversion, agentKernel.Agreeableness, agentKernel.Neuroticism);



        // get the actions to run their StartAwake function, gives all actions their location and owner
        newActions.BroadcastMessage("StartAwake");


        //add to the town's population
        townAgents.Add(newAgent);
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
        // test that its the player object
        if (collision.gameObject.tag != "Player")
            return;

        //expand the agents using the UnZipPerFrameCoroutine
        if (!single)
        {
            single = true;

            if(isUnzippingPerFrame)
                StartCoroutine("UnZipPerFrame");
            else
            {
                foreach (AgentKernel kern in compressedAgents)
                {
                    UnZipAgent(kern);
                }

                //after building each agent, set them active
                foreach (Character chara in townAgentCharacters)
                    chara.SetActive();
            }
        }
    }


    IEnumerator UnZipPerFrame()
    {
        foreach (AgentKernel kern in compressedAgents)
        {
            UnZipAgent(kern);
            //wait for a frame
            yield return null;
        }

        // after building each agent, set them active
        foreach (Character chara in townAgentCharacters)
        {
            chara.SetActive();

            //yield return null;
        }
            
    }



    void OnTriggerExit(Collider collision)
    {
        // test that its the player object
        if (collision.gameObject.tag != "Player")
            return;

        //collapse the agents
        foreach (GameObject agent in townAgents)
            Destroy(agent);

        //destroy agent references in UI as well
        agentManagerUI.CollapseAgents();


        UnityEngine.Debug.Log("They have gone, they have vanished");

        single = false;
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
