using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;


public class Character : MonoBehaviour
{
    public bool isDebugging;


    //non-collapsable items from agent kernel, must be set when building the agent
    public Transform homeLocation; // their house
    public Occupation occupation; // their job
    public Personality personality; // The personality game object holds personality + state parameters

    bool isActive = false;


    //agent
    public Agent thisAgent;
    public TextMesh voice;
    // Simple Movement
    NavMeshAgent Walker;
    public Transform target;
    public Animation anim;

    public Camera agentCamera;



    private AgentStateVarFloat energy, hunger, wealth, mood, temper, sociability, soberness, resources;
    // Vector that holds the agent's state
    List<AgentStateVarFloat> stateVector = new List<AgentStateVarFloat>();


    public DayNightCycle clock;
    private TownInfo towninfo;
    


    // logging of agent behaiour info
    [HideInInspector]
    public List<AgentLog> behaviourLog = new List<AgentLog>();
    // the currently running actions
    public List<string> runningActions = new List<string>();
    ActionBehaviour previousAction;
    

    // for holding all the locations
    public Dictionary<string, Transform> locationDictionary = new Dictionary<string, Transform>();


    void Awake()
    {
        // set up agent and navmesh agent
        thisAgent = GetComponent<Agent>();
        Walker = GetComponent<NavMeshAgent>();

        //SetNaveMeshAgentSpeed();

        // get the important external objects
        clock = GameObject.Find("Sun").GetComponent<DayNightCycle>();
        towninfo = GameObject.Find("Town").GetComponent<TownInfo>();

        ConstructStateVector();
    }



    public void SetupAgent(ActionBehaviour _action, float O, float C, float E, float A, float N)
    {
        if (isDebugging)
            Debug.Log("activating agent");

        // set the actions
        thisAgent.SetRootAction(_action);
        thisAgent.SetSocialInterruption();

        // build the agent's personality 
        personality.GeneratePersonality(O, C, E, A, N);

        // set the initial 
        previousAction = _action;

        // create the action delegates
        SetActionDelegates();
    }

    public void SetActive()
    {
        //isActive = !isActive;
    }

    void Update()
    {
        if (Input.GetKeyDown("l"))
            isActive = true;


        if (!isActive)
            return;

        // Perform utility decision making and behaviour
        thisAgent.UpdateAI();

        // use animations and navigations
        Move();
    }




    #region MOVEMENT


    void SetNaveMeshAgentSpeed()
    {
        Walker.speed *= UtilityTime.time;
    }

    void Move()
    {
        // scale animation speed with navmeshagent velocity


        //Update movement
        Walker.SetDestination(target.position);
        
        

        if (Vector3.Distance(transform.position, target.position) < 3.0f)
        {
            anim.Stop();
            //anim.Play("run");
        }
        else
        {
            Speak("Walking to target");
            anim.Play();
        }
    }

    // target is set by active leaf actions in their 'Execute' function
    public void SetTarget(Transform destination)
    {
        target = destination;
    }

    // focusses the agent towards an object of attention
    public void SetFocusPoint(Transform focuspoint)
    {
        transform.LookAt(focuspoint);
    }


    #endregion






    #region UI STUFF

    void Speak(string sentence)
    {
        //voice.text = sentence;
    }


    public List<AgentStateVarFloat> GetStats()
    {
        return stateVector;
    }


    #endregion





    #region AGENT LOG

    public void LogActionBegin(ActionBehaviour _action)
    {
        // check if its the same action
        if (_action.name == previousAction.name)
            return;

        if (isDebugging)
            Debug.Log(name + " " + _action + " began");

        float currentTime = clock.timeOfDay;
        behaviourLog.Add(new AgentLog(_action.name, currentTime, _action.GetActionScore()));

        // run this once to get the action decision hierarchy
        GetRunningActions();

        previousAction = _action;
    }

    public void LogActionEnd()
    {
        float currentTime = clock.timeOfDay;
        behaviourLog[behaviourLog.Count - 1].EndTime = currentTime;
    }

    // this is for providing a list of currently running actions through the hierarchy
    public void GetRunningActions()
    {
        runningActions.Clear();

        ActionBehaviour runningAction = thisAgent.linkedRootAction;
        // do a quick pass down the hierarchy
        while(!runningAction.isLeafAction)
        {
            runningActions.Add(runningAction.name);
            runningAction = runningAction.TopAction;
        }

        // add the running leaf action
        runningActions.Add(behaviourLog[behaviourLog.Count - 1].action);
    }

    #endregion




    #region PERFORMING ACTIONS


    void BuyFoodAtMarket()
    {
        PerformAction("buyfoodatmarket");

        Speak("Buying Food At Market");
    }

    void EatFoodAtHome()
    {
        PerformAction("eatfoodathome");

        Speak("Eating food at home");
    }

    void StealFood()
    {
        PerformAction("stealfood");

        Speak("Stealing Food");
    }

    void SleepAtHome()
    {
        PerformAction("sleepathome");

        Speak("Sleeping at home");
    }

    void SleepOnTheSpot()
    {
        PerformAction("sleeponthespot");

        Speak("Sleeping on the spot");
    }

    void DrinkBelligerently()
    {
        PerformAction("drinkbelligerently");

        Speak("Drinking at the tavern (belligerent)");
    }

    void DrinkAmicably()
    {
        PerformAction("drinkamicably");

        Speak("Drinking at the tavern (Amicable)");
    }

    void PrayAtChurch()
    {
        PerformAction("prayatchurch");

        Speak("Praying at the church");
    }

    void GoFishing()
    {
        PerformAction("gofishing");

        Speak("Going Fishing");
    }

    void Blacksmithing()
    {
        PerformAction("blacksmithing");

        Speak("Blacksmithing");
    }

    void SleepOnTheJob()
    {
        PerformAction("sleeponthejob");

        Speak("Sleeping on the job");
    }

    void WorkDiligently()
    {
        PerformAction("workdiligently");

        Speak("Working diligently");
    }

    void SellWares()
    {
        PerformAction("sellwares");

        Speak("Selling Wares");
    }

    void Socialise()
    {
        PerformAction("socialise");

        Speak("Socialising");
    }

    void SocialiseNice()
    {
        PerformAction("socialisenice");

        Speak("Socialising (nice)");

        //pass the message to the interrupt class so it can be relayed to the social recipient
        GetComponent<Interrupt>().SendNiceInteraction();
    }

    void SocialiseMean()
    {
        PerformAction("socialisemean");

        Speak("Socialising (mean)");

        //pass the message to the interrupt class so it can be relayed to the social recipient
        GetComponent<Interrupt>().SendMeanInteraction();
    }


    public void ReceiveSocialiseNice(Relationship rel)
    {
        //Debug.Log(rel.nameOfPerson + " is being nice to " + this.name);
        rel.relationshipValue.value += UtilityTime.time; //@TODO, fix amount
    } 

    public void ReceiveSocialiseMean(Relationship rel)
    {
        //Debug.Log(rel.nameOfPerson + " is being mean to "  + this.name);
        rel.relationshipValue.value -= UtilityTime.time; //@TODO, fix amount
    }




    void PerformAction(string action)
    {
        //get the action, get the modification vector which will be stored by the agent (ie sign of effect and magnitude for all state variables)

        // get the list from the global agent info @TODO maybe fix this
        List<float> actionModificationVector = personality.globalAgentInfo.actionModifierDictionary[action];

        // For each of the state parameters, perform the operation specified in that action's modification vector
        for (int i = 0; i < stateVector.Count; i++)
        {
            float stateModifier = actionModificationVector[i];

            stateVector[i].value += stateModifier * UtilityTime.time;
        }
    }




    void SetActionDelegates()
    {
        //add function delegate to action
        thisAgent.SetVoidActionDelegate("BuyFoodAtMarket", BuyFoodAtMarket);
        thisAgent.SetVoidActionDelegate("EatFoodAtHome", EatFoodAtHome);
        thisAgent.SetVoidActionDelegate("StealFood", StealFood);
        thisAgent.SetVoidActionDelegate("SleepAtHome", SleepAtHome);
        thisAgent.SetVoidActionDelegate("SleepOnTheSpot", SleepOnTheSpot);
        thisAgent.SetVoidActionDelegate("DrinkBelligerently", DrinkBelligerently);
        thisAgent.SetVoidActionDelegate("DrinkAmicably", DrinkAmicably);
        thisAgent.SetVoidActionDelegate("PrayAtChurch", PrayAtChurch);
        thisAgent.SetVoidActionDelegate("GoFishing", GoFishing);
        thisAgent.SetVoidActionDelegate("WorkDiligently", WorkDiligently);
        thisAgent.SetVoidActionDelegate("SleepOnTheJob", SleepOnTheJob);
        thisAgent.SetVoidActionDelegate("SellWares", SellWares);

        thisAgent.SetVoidActionDelegate("Socialise", Socialise);
        thisAgent.SetVoidActionDelegate("SocialiseNice", SocialiseNice);
        thisAgent.SetVoidActionDelegate("SocialiseMean", SocialiseMean);
    }

    #endregion



  
    // for modification when performing actions
    void ConstructStateVector()
    {
        hunger = personality.hunger;
        energy = personality.energy;
        wealth = personality.wealth;
        mood = personality.mood;
        temper = personality.temper;
        sociability = personality.sociability;
        soberness = personality.soberness;
        resources = personality.resources;

        stateVector.Add(hunger);
        stateVector.Add(energy);
        stateVector.Add(wealth);
        stateVector.Add(mood);
        stateVector.Add(temper);
        stateVector.Add(sociability);
        stateVector.Add(soberness);
        stateVector.Add(resources);
    }


}