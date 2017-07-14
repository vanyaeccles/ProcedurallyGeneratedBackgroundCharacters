using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;


public class Character : MonoBehaviour
{
    
    //agent
    Agent agent;
    public AgentStateVarFloat energy, hunger, wealth, mood, temper, sociability, soberness;
    public TextMesh voice, stats;

    List<AgentStateVarFloat> stateVector = new List<AgentStateVarFloat>();

    Dictionary<string, List<float>> actionModifierDictionary = new Dictionary<string, List<float>>();



    // Simple Movement
    NavMeshAgent Walker;
    public Transform target;



    void Start()
    {
        agent = GetComponent<Agent>();

        ConstructStateVector();
        ConstructActionModifierDictionary();

        //add function delegate to action
        //agent.SetVoidActionDelegate("Sleep", Sleep);
        //agent.SetVoidActionDelegate("Work", Work);
        //agent.SetVoidActionDelegate("Eat", Eat);
        //agent.SetVoidActionDelegate("EatExpensive", EatExpensive);
        //agent.SetVoidActionDelegate("EatCheap", EatCheap);
        agent.SetVoidActionDelegate("BuyFoodAtMarket", BuyFoodAtMarket);
        agent.SetVoidActionDelegate("EatFoodAtHome", EatFoodAtHome);
        agent.SetVoidActionDelegate("StealFood", StealFood);
        agent.SetVoidActionDelegate("SleepAtHome", SleepAtHome);
        agent.SetVoidActionDelegate("SleepOnTheSpot", SleepOnTheSpot);
        agent.SetVoidActionDelegate("DrinkAtTavern", DrinkAtTavern);
        agent.SetVoidActionDelegate("PrayAtChurch", PrayAtChurch);
        agent.SetVoidActionDelegate("GoFishing", GoFishing);
        agent.SetVoidActionDelegate("Blacksmithing", Blacksmithing);

        Walker = GetComponent<NavMeshAgent>();
    }


    // Update is called once per frame
    void Update()
    {
        // Perform utility decision making and behaviour
        agent.UpdateAI();

        //Update movement
        Walker.SetDestination(target.position);

        // Show some stuff to UI
        DisplayStats("energy: " + energy.value.ToString("F0")  + " hunger: " + hunger.value.ToString("F0") + " wealth: " + wealth.value.ToString("F0") + "mood: " + mood.value.ToString("F0") + " temper: " + temper.value.ToString("F0") + " sociability: " + sociability.value.ToString("F0") + " soberness: " + soberness.value.ToString("F0"));
    }



    public void Interrupt()
    {

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

    //void Sleep()
    //{ 
    //    PerformAction("sleep");

    //    Speak("Sleeping");
    //}

    //void Eat()
    //{
    //    PerformAction("eat");

    //    Speak("Eating");

    //    //Evaluate children + execute them
    //}

    //void EatExpensive()
    //{
    //    PerformAction("eatexpensive");

    //    Speak("Eating Expensive Food");
    //}

    //void EatCheap()
    //{
    //    PerformAction("eatcheap");

    //    Speak("Eating Cheap Food");
    //}

    //void Work()
    //{
    //    PerformAction("work");

    //    Speak("Working");
    //}

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
        //Debug.Log("sleep");
    
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
        List<float> eatfoodathomeValues = new List<float>();
        eatfoodathomeValues.Add(-5.0f);
        eatfoodathomeValues.Add(-1.0f);
        eatfoodathomeValues.Add(-1.0f);
        eatfoodathomeValues.Add(0.0f);
        eatfoodathomeValues.Add(+0.0f);
        eatfoodathomeValues.Add(+2.0f);
        eatfoodathomeValues.Add(+0.0f);
        List<float> stealfoodValues = new List<float>();
        stealfoodValues.Add(-2.0f);
        stealfoodValues.Add(-3.0f);
        stealfoodValues.Add(+0.0f);
        stealfoodValues.Add(-1.0f);
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
        List<float> sleepathomeValues = new List<float>();
        sleepathomeValues.Add(+5.0f);
        sleepathomeValues.Add(+10.0f);
        sleepathomeValues.Add(+0.0f);
        sleepathomeValues.Add(+3.0f);
        sleepathomeValues.Add(+3.0f);
        sleepathomeValues.Add(+1.0f);
        sleepathomeValues.Add(+2.0f);
        List<float> sleeponthespotValues = new List<float>();
        sleeponthespotValues.Add(+7.0f);
        sleeponthespotValues.Add(+5.0f);
        sleeponthespotValues.Add(+0.0f);
        sleeponthespotValues.Add(-3.0f);
        sleeponthespotValues.Add(-3.0f);
        sleeponthespotValues.Add(+0.0f);
        sleeponthespotValues.Add(+2.0f);
        List<float> drinkattavernValues = new List<float>();
        drinkattavernValues.Add(+1.0f);
        drinkattavernValues.Add(-4.0f);
        drinkattavernValues.Add(-4.0f);
        drinkattavernValues.Add(+5.0f);
        drinkattavernValues.Add(-2.0f);
        drinkattavernValues.Add(-5.0f);
        drinkattavernValues.Add(-10.0f);
        List<float> prayatchurchValues = new List<float>();
        prayatchurchValues.Add(+3.0f);
        prayatchurchValues.Add(-4.0f);
        prayatchurchValues.Add(-2.0f);
        prayatchurchValues.Add(+5.0f);
        prayatchurchValues.Add(+5.0f);
        prayatchurchValues.Add(-2.0f);
        prayatchurchValues.Add(+5.0f);
        List<float> blacksmithingValues = new List<float>();
        blacksmithingValues.Add(+3.0f);
        blacksmithingValues.Add(-5.0f);
        blacksmithingValues.Add(+5.0f);
        blacksmithingValues.Add(+1.0f);
        blacksmithingValues.Add(-1.0f);
        blacksmithingValues.Add(+2.0f);
        blacksmithingValues.Add(+0.0f);
        //List<float> sleepValues = new List<float>();
        //sleepValues.Add(+10.0f);
        //sleepValues.Add(+5.0f);
        //sleepValues.Add(+0.0f); 

        //List<float> eatValues = new List<float>();
        //eatValues.Add(+4.0f);
        //eatValues.Add(-10.0f);
        //eatValues.Add(-5.0f);

        //List<float> eatExpensiveValues = new List<float>();
        //eatExpensiveValues.Add(+2.0f);
        //eatExpensiveValues.Add(-8.0f);
        //eatExpensiveValues.Add(-10.0f);

        //List<float> eatCheapValues = new List<float>();
        //eatCheapValues.Add(+2.0f);
        //eatCheapValues.Add(-8.0f);
        //eatCheapValues.Add(-5.0f);

        //List<float> workValues = new List<float>();
        //workValues.Add(-1.0f);
        //workValues.Add(+10.0f);
        //workValues.Add(+20.0f);

        //actionModifierDictionary.Add("sleep", sleepValues);
        //actionModifierDictionary.Add("eat", eatValues);
        //actionModifierDictionary.Add("eatexpensive", eatExpensiveValues);
        //actionModifierDictionary.Add("eatcheap", eatCheapValues);
        //actionModifierDictionary.Add("work", workValues);

        actionModifierDictionary.Add("buyfoodatmarket", buyfoodatmarketValues);
        actionModifierDictionary.Add("eatfoodathome", eatfoodathomeValues);
        actionModifierDictionary.Add("stealfood", stealfoodValues);
        actionModifierDictionary.Add("gofishing", gofishingValues);
        actionModifierDictionary.Add("sleepathome", sleepathomeValues);
        actionModifierDictionary.Add("sleeponthespot", sleeponthespotValues);
        actionModifierDictionary.Add("drinkattavern", drinkattavernValues);
        actionModifierDictionary.Add("prayatchurch", prayatchurchValues);
        actionModifierDictionary.Add("blacksmithing", blacksmithingValues);
    }

    // @TODO could automatically add state parameters?
    void ConstructStateVector()
    {
        stateVector.Add(hunger);
        stateVector.Add(energy);
        stateVector.Add(wealth);
        stateVector.Add(mood);
        stateVector.Add(temper);
        stateVector.Add(sociability);
        stateVector.Add(soberness);
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