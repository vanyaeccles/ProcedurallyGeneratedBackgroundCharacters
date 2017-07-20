using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;


public class Character : MonoBehaviour
{
    
    //agent
    Agent agent;
    public TextMesh voice, stats;
    // Simple Movement
    NavMeshAgent Walker;
    public Transform target;
    public Animation anim;




    // The personality game object holds personality + state parameters
    public Personality personality;
    private AgentStateVarFloat energy, hunger, wealth, mood, temper, sociability, soberness, resources;
    // Vector that holds the agent's state
    List<AgentStateVarFloat> stateVector = new List<AgentStateVarFloat>();

    //relationships, modelled as state parameters
    public AgentStateVarFloat Agent1;

    // the dictionary thats used to execute effect of behaviours
    Dictionary<string, List<float>> actionModifierDictionary = new Dictionary<string, List<float>>();

    


    void Start()
    {
        agent = GetComponent<Agent>();
        Walker = GetComponent<NavMeshAgent>();


        ConstructStateVector();
        ConstructActionModifierDictionary();

        //add function delegate to action
        agent.SetVoidActionDelegate("BuyFoodAtMarket", BuyFoodAtMarket);
        agent.SetVoidActionDelegate("EatFoodAtHome", EatFoodAtHome);
        agent.SetVoidActionDelegate("StealFood", StealFood);
        agent.SetVoidActionDelegate("SleepAtHome", SleepAtHome);
        agent.SetVoidActionDelegate("SleepOnTheSpot", SleepOnTheSpot);
        agent.SetVoidActionDelegate("DrinkAtTavern", DrinkAtTavern);
        agent.SetVoidActionDelegate("PrayAtChurch", PrayAtChurch);
        agent.SetVoidActionDelegate("GoFishing", GoFishing);
        agent.SetVoidActionDelegate("Blacksmithing", Blacksmithing);
        agent.SetVoidActionDelegate("WorkDiligently", WorkDiligently);
        agent.SetVoidActionDelegate("SleepOnTheJob", SleepOnTheJob);
        agent.SetVoidActionDelegate("SellWares", SellWares);

        agent.SetVoidActionDelegate("Socialise", Socialise);


        
    }


    // Update is called once per frame
    void Update()
    {
        // Perform utility decision making and behaviour
        agent.UpdateAI();

        Move();

        // Show some stuff to UI
        DisplayStats(" energy: " + energy.value.ToString("F0")
            + "\n hunger: " + hunger.value.ToString("F0")
            + "\n resources: " + resources.value.ToString("F0")
            + "\n wealth: " + wealth.value.ToString("F0") 
            + "\n mood: " + mood.value.ToString("F0") 
            + "\n temper: " + temper.value.ToString("F0") 
            + "\n sociability: " + sociability.value.ToString("F0")
            + "\n soberness: " + soberness.value.ToString("F0"));
    }


    void Move()
    {
        // scale animation speed with navmeshagent velocity


        //Update movement
        Walker.SetDestination(target.position);

        

        if (Vector3.Distance(transform.position, target.position) < 1.0f)
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

    



    #region UI STUFF

    void Speak(string sentence)
    {
        voice.text = sentence;
    }
    void DisplayStats(string sentence)
    {
        stats.text = sentence;
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

    void DrinkAtTavern()
    {
        PerformAction("drinkattavern");

        Speak("Drinking at the tavern");
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



    void PerformAction(string action)
    {
        //get the action, get the modification vector which will be stored by the agent (ie sign of effect and magnitude for all state variables)

        List<float> actionModificationVector = actionModifierDictionary[action];

        // For each of the state parameters, perform the operation specified in that action's modification vector
        for (int i = 0; i < stateVector.Count; i++)
        {
            float stateModifier = actionModificationVector[i];

            stateVector[i].value += stateModifier * UtilityTime.time;
        }
    }

    #endregion


    public void SetTarget(Transform destination)
    {
        target = destination;
    }




    #region CONSTRUCTING AGENT VARIABLES

    void ConstructActionModifierDictionary()
    {
        /*
            State variables:
            Hunger
            Energy
            Wealth
            Mood
            Temper
            Sociability
            Soberness
            Resources
        */

        //This is temporary, creates the modification vectors for every action in the game
        //Important to ensure that the list of state parameters is in the same order as the modification vectors in the dictionary

        // the money option in particular could be different, how could the agent's personality specify how much money they spend?

        List<float> buyfoodatmarketValues = new List<float>();
        buyfoodatmarketValues.Add(-5.0f);
        buyfoodatmarketValues.Add(-1.0f);
        buyfoodatmarketValues.Add(-5.0f);
        buyfoodatmarketValues.Add(+2.0f);
        buyfoodatmarketValues.Add(+0.0f);
        buyfoodatmarketValues.Add(-1.0f);
        buyfoodatmarketValues.Add(+0.0f);
        buyfoodatmarketValues.Add(-5.0f);
        List<float> eatfoodathomeValues = new List<float>();
        eatfoodathomeValues.Add(-5.0f);
        eatfoodathomeValues.Add(-1.0f);
        eatfoodathomeValues.Add(-1.0f);
        eatfoodathomeValues.Add(0.0f);
        eatfoodathomeValues.Add(+0.0f);
        eatfoodathomeValues.Add(+2.0f);
        eatfoodathomeValues.Add(+0.0f);
        eatfoodathomeValues.Add(-1.0f);
        List<float> stealfoodValues = new List<float>();
        stealfoodValues.Add(-2.0f);
        stealfoodValues.Add(-3.0f);
        stealfoodValues.Add(+0.0f);
        stealfoodValues.Add(-1.0f);
        stealfoodValues.Add(+0.0f);
        stealfoodValues.Add(+0.0f);
        stealfoodValues.Add(+0.0f);
        stealfoodValues.Add(+0.0f);
        List<float> gofishingValues = new List<float>();
        gofishingValues.Add(-3.0f);
        gofishingValues.Add(+0.0f);
        gofishingValues.Add(+2.0f);
        gofishingValues.Add(+5.0f);
        gofishingValues.Add(+5.0f);
        gofishingValues.Add(+3.0f);
        gofishingValues.Add(+0.0f);
        gofishingValues.Add(+2.0f);
        List<float> sleepathomeValues = new List<float>();
        sleepathomeValues.Add(+5.0f);
        sleepathomeValues.Add(+10.0f);
        sleepathomeValues.Add(+0.0f);
        sleepathomeValues.Add(+3.0f);
        sleepathomeValues.Add(+3.0f);
        sleepathomeValues.Add(+1.0f);
        sleepathomeValues.Add(+2.0f);
        sleepathomeValues.Add(+0.0f);
        List<float> sleeponthespotValues = new List<float>();
        sleeponthespotValues.Add(+7.0f);
        sleeponthespotValues.Add(+5.0f);
        sleeponthespotValues.Add(+0.0f);
        sleeponthespotValues.Add(-3.0f);
        sleeponthespotValues.Add(-3.0f);
        sleeponthespotValues.Add(+0.0f);
        sleeponthespotValues.Add(+2.0f);
        sleeponthespotValues.Add(+0.0f);
        List<float> drinkattavernValues = new List<float>();
        drinkattavernValues.Add(+1.0f);
        drinkattavernValues.Add(-4.0f);
        drinkattavernValues.Add(-4.0f);
        drinkattavernValues.Add(+5.0f);
        drinkattavernValues.Add(-2.0f);
        drinkattavernValues.Add(-5.0f);
        drinkattavernValues.Add(-10.0f);
        drinkattavernValues.Add(-4.0f);
        List<float> prayatchurchValues = new List<float>();
        prayatchurchValues.Add(+3.0f);
        prayatchurchValues.Add(-4.0f);
        prayatchurchValues.Add(-2.0f);
        prayatchurchValues.Add(+5.0f);
        prayatchurchValues.Add(+5.0f);
        prayatchurchValues.Add(-2.0f);
        prayatchurchValues.Add(+5.0f);
        prayatchurchValues.Add(-2.0f);
        List<float> blacksmithingValues = new List<float>();
        blacksmithingValues.Add(+3.0f);
        blacksmithingValues.Add(-5.0f);
        blacksmithingValues.Add(+5.0f);
        blacksmithingValues.Add(+1.0f);
        blacksmithingValues.Add(-1.0f);
        blacksmithingValues.Add(+2.0f);
        blacksmithingValues.Add(+0.0f);
        blacksmithingValues.Add(+5.0f);
        List<float> workdiligentlyValues = new List<float>();
        workdiligentlyValues.Add(+3.0f);
        workdiligentlyValues.Add(-5.0f);
        workdiligentlyValues.Add(+3.0f);
        workdiligentlyValues.Add(+1.0f);
        workdiligentlyValues.Add(-1.0f);
        workdiligentlyValues.Add(+2.0f);
        workdiligentlyValues.Add(+0.0f);
        workdiligentlyValues.Add(+3.0f);
        List<float> sleeponthejobValues = new List<float>();
        sleeponthejobValues.Add(+2.0f);
        sleeponthejobValues.Add(+5.0f);
        sleeponthejobValues.Add(+0.0f);
        sleeponthejobValues.Add(+1.0f);
        sleeponthejobValues.Add(-1.0f);
        sleeponthejobValues.Add(+2.0f);
        sleeponthejobValues.Add(+0.0f);
        sleeponthejobValues.Add(+0.0f);
        List<float> sellwaresValues = new List<float>();
        sellwaresValues.Add(+3.0f);
        sellwaresValues.Add(-5.0f);
        sellwaresValues.Add(+5.0f);
        sellwaresValues.Add(+1.0f);
        sellwaresValues.Add(-1.0f);
        sellwaresValues.Add(+2.0f);
        sellwaresValues.Add(+0.0f);
        sellwaresValues.Add(+5.0f);
        List<float> socialiseValues = new List<float>();
        socialiseValues.Add(+3.0f);
        socialiseValues.Add(-5.0f);
        socialiseValues.Add(+5.0f);
        socialiseValues.Add(+1.0f);
        socialiseValues.Add(-1.0f);
        socialiseValues.Add(+2.0f);
        socialiseValues.Add(+0.0f);
        socialiseValues.Add(+5.0f);

        actionModifierDictionary.Add("buyfoodatmarket", buyfoodatmarketValues);
        actionModifierDictionary.Add("eatfoodathome", eatfoodathomeValues);
        actionModifierDictionary.Add("stealfood", stealfoodValues);
        actionModifierDictionary.Add("gofishing", gofishingValues);
        actionModifierDictionary.Add("sleepathome", sleepathomeValues);
        actionModifierDictionary.Add("sleeponthespot", sleeponthespotValues);
        actionModifierDictionary.Add("drinkattavern", drinkattavernValues);
        actionModifierDictionary.Add("prayatchurch", prayatchurchValues);
        actionModifierDictionary.Add("blacksmithing", blacksmithingValues);
        actionModifierDictionary.Add("workdiligently", workdiligentlyValues);
        actionModifierDictionary.Add("sellwares", sellwaresValues);
        actionModifierDictionary.Add("sleeponthejob", sleeponthejobValues);
        actionModifierDictionary.Add("socialise", socialiseValues);
    }

    // @TODO could automatically add state parameters?
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

    #endregion




    //Returns the NPC state parameter modifier for a given action
    //Might not be needed @TODO
    float ActionModifierParser(string operat, float modifValue)
    {
        float modifier = 0.0f;

        switch (operat)
        {
            case "+":
                modifier += modifValue * UtilityTime.time;
                break;
            case "-":
                modifier -= modifValue * UtilityTime.time;
                break;
            default:
                Debug.LogWarning("Incorrect operator passed to action modifier: " + operat);
                break;
        }

        return modifier;
    }



}