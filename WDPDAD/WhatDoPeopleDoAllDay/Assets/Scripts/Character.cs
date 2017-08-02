using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;


public class Character : MonoBehaviour
{
    
    //agent
    Agent agent;
    public TextMesh voice;
    // Simple Movement
    NavMeshAgent Walker;
    public Transform target;
    public Animation anim;

    public Camera agentCamera;


    // The personality game object holds personality + state parameters
    public Personality personality;

    private AgentStateVarFloat energy, hunger, wealth, mood, temper, sociability, soberness, resources;
    // Vector that holds the agent's state
    List<AgentStateVarFloat> stateVector = new List<AgentStateVarFloat>();

    //relationships, modelled as state parameters
    public AgentStateVarFloat Agent1;



    // logging of agent behaiour info
    List<AgentLog> behaviourLog = new List<AgentLog>();
    




    void Start()
    {
        agent = GetComponent<Agent>();
        Walker = GetComponent<NavMeshAgent>();

        ConstructStateVector();

        SetActionDelegates();
    }


    // Update is called once per frame
    void Update()
    {
        // Perform utility decision making and behaviour
        agent.UpdateAI();

        Move();

    }






    #region MOVEMENT

    void Move()
    {
        // scale animation speed with navmeshagent velocity


        //Update movement
        Walker.SetDestination(target.position);
        //Walker.speed = Walker.speed * UtilityTime.speed;
        

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
        rel.relationshipValue.value += UtilityTime.time; //@TODO, fix amount
    } 

    public void ReceiveSocialiseMean(Relationship rel)
    {

        rel.relationshipValue.value += UtilityTime.time; //@TODO, fix amount
    }



    void PerformAction(string action)
    {
        //get the action, get the modification vector which will be stored by the agent (ie sign of effect and magnitude for all state variables)

        List<float> actionModificationVector = personality.actionModifierDictionary[action];

        // For each of the state parameters, perform the operation specified in that action's modification vector
        for (int i = 0; i < stateVector.Count; i++)
        {
            float stateModifier = actionModificationVector[i];

            stateVector[i].value += stateModifier * UtilityTime.time;
        }


        // logging behaviour info
        LogAction(action);
    }

    void LogAction(string _action)
    {
        //@TODO finish this
        if (behaviourLog.Count <= 0)
        {
            //Debug.Log("First action: " + _action);
            behaviourLog.Add(new AgentLog(_action));
        }
            

        if(_action != behaviourLog[behaviourLog.Count - 1].action)
        {
            //Debug.Log("Action: " + behaviourLog[behaviourLog.Count - 1].action + " ended");

            behaviourLog.Add(new AgentLog(_action));

            //Debug.Log("New action: " + behaviourLog[behaviourLog.Count - 1].action);
        }
            

        behaviourLog[behaviourLog.Count - 1].duration += UtilityTime.time;
        //Debug.Log("Current action time: " + behaviourLog[behaviourLog.Count - 1].duration);
    }


    void SetActionDelegates()
    {
        //add function delegate to action
        agent.SetVoidActionDelegate("BuyFoodAtMarket", BuyFoodAtMarket);
        agent.SetVoidActionDelegate("EatFoodAtHome", EatFoodAtHome);
        agent.SetVoidActionDelegate("StealFood", StealFood);
        agent.SetVoidActionDelegate("SleepAtHome", SleepAtHome);
        agent.SetVoidActionDelegate("SleepOnTheSpot", SleepOnTheSpot);
        agent.SetVoidActionDelegate("DrinkBelligerently", DrinkBelligerently);
        agent.SetVoidActionDelegate("DrinkAmicably", DrinkAmicably);
        agent.SetVoidActionDelegate("PrayAtChurch", PrayAtChurch);
        agent.SetVoidActionDelegate("GoFishing", GoFishing);
        agent.SetVoidActionDelegate("WorkDiligently", WorkDiligently);
        agent.SetVoidActionDelegate("SleepOnTheJob", SleepOnTheJob);
        agent.SetVoidActionDelegate("SellWares", SellWares);
        agent.SetVoidActionDelegate("Socialise", Socialise);
        agent.SetVoidActionDelegate("SocialiseNice", SocialiseNice);
        agent.SetVoidActionDelegate("SocialiseMean", SocialiseMean);
    }

    #endregion



  

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