using UnityEngine;
using System.Collections.Generic;



public class Character : MonoBehaviour
{
    
    //agent
    Agent agent;
    public AgentStateVarFloat energy, hunger, money;
    public TextMesh voice, stats;

    List<AgentStateVarFloat> stateVector = new List<AgentStateVarFloat>();

    Dictionary<string, List<float>> actionModifierDictionary = new Dictionary<string, List<float>>();



    void Start()
    {
        agent = GetComponent<Agent>();

        ConstructStateVector();
        ConstructActionModifierDictionary();

        //add function delegate to action
        agent.SetVoidActionDelegate("Sleep", Sleep);
        agent.SetVoidActionDelegate("Work", Work);
        agent.SetVoidActionDelegate("Eat", Eat);
        agent.SetVoidActionDelegate("EatExpensive", EatExpensive);
        agent.SetVoidActionDelegate("EatCheap", EatCheap);
    }

    // Update is called once per frame
    void Update()
    {
        agent.UpdateAI();

        DisplayStats("energy: " + energy.value.ToString("F0")  + " hunger: " + hunger.value.ToString("F0") + " money: " + money.value.ToString("F0"));
    }


    #region CONSTRUCTING AGENT VARIABLES

    void ConstructActionModifierDictionary()
    {
        /*
            State variables:
            Energy
            Hunger
            Money
        */

        //This is temporary, creates the modification vectors for every action in the game
        //Important to ensure that the list of state parameters is in the same order as the modification vectors in the dictionary
        List<float> sleepValues = new List<float>();
        sleepValues.Add(+10.0f);
        sleepValues.Add(+5.0f);
        sleepValues.Add(+0.0f); // the money option in particular could be different, how could the agent's personality specify how much money they spend?

        List<float> eatValues = new List<float>();
        eatValues.Add(+4.0f);
        eatValues.Add(-10.0f);
        eatValues.Add(-5.0f);

        List<float> eatExpensiveValues = new List<float>();
        eatExpensiveValues.Add(+2.0f);
        eatExpensiveValues.Add(-8.0f);
        eatExpensiveValues.Add(-10.0f);

        List<float> eatCheapValues = new List<float>();
        eatCheapValues.Add(+2.0f);
        eatCheapValues.Add(-8.0f);
        eatCheapValues.Add(-5.0f);

        List<float> workValues = new List<float>(); 
        workValues.Add(-1.0f);
        workValues.Add(+10.0f);
        workValues.Add(+20.0f);

        actionModifierDictionary.Add("sleep", sleepValues);
        actionModifierDictionary.Add("eat", eatValues);
        actionModifierDictionary.Add("eatexpensive", eatExpensiveValues);
        actionModifierDictionary.Add("eatcheap", eatCheapValues);
        actionModifierDictionary.Add("work", workValues);
    }

    // @TODO could automatically add state parameters?
    void ConstructStateVector()
    {
        stateVector.Add(energy);
        stateVector.Add(hunger);
        stateVector.Add(money);
    }

    #endregion




    void Speak(string sentence)
    {
        voice.text = sentence;
    }
    void DisplayStats(string sentence)
    {
        stats.text = sentence;
    }
   

    void Sleep()
    { 
        PerformAction("sleep");

        Speak("Sleeping");
    }

    void Eat()
    {
        PerformAction("eat");

        Speak("Eating");

        //Evaluate children + execute them
    }

    void EatExpensive()
    {
        PerformAction("eatexpensive");

        Speak("Eating Expensive Food");
    }

    void EatCheap()
    {
        PerformAction("eatcheap");

        Speak("Eating Cheap Food");
    }



    void Work()
    {
        PerformAction("work");

        Speak("Working");
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