using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personality : MonoBehaviour {


    List<float> personalityVector = new List<float>();

    [Header("Personality Vector (Normalised values)")]
    public float OpennessToExperience;
    public float Concientiousness;
    public float Extroversion;
    public float Agreeableness;
    public float Neuroticism;


    [Header("Agent State Parameters")]
    public AgentStateVarFloat hunger;
    public AgentStateVarFloat energy, wealth, mood, temper, sociability, soberness, resources;

    [Header("Relationships")]
    public AgentStateParameter agent1;

    [Header("Global")]
    public AgentStateParameter timeofday;



    // contains the influences that personlaity parameters have on state variable decisions
    // ie how does extroversion level relate to importance of sociability, @TODO could replace this with a string tuple-value dictionary
    private float[,] personalityWeightInfluences = new float[5,10];
    // How the personality variables influence state variable modifiers (ie when performing an action)
    private float[,] personalityModifierInfluences = new float[5, 10];
    private Dictionary<string, int> StateVarDictionary = new Dictionary<string, int>();


    void Awake()
    {
        // the same weight influences for all agents
        BuildPersonalityWeightInfluences();
        BuildStateVarDictionary();

        //Debug.Log(CheckWeight("timeofday"));
        //Debug.Log(CheckWeight("hunger"));
        //Debug.Log(CheckWeight("energy"));
        //Debug.Log(CheckWeight("wealth"));
        //Debug.Log(CheckWeight("mood"));
        //Debug.Log(CheckWeight("temper"));
        //Debug.Log(CheckWeight("sociability"));
        //Debug.Log(CheckWeight("soberness"));
        //Debug.Log(CheckWeight("resources"));
    }

    // Use this for initialization
    void Start ()
    {
        personalityVector.Add(OpennessToExperience);
        personalityVector.Add(Concientiousness);
        personalityVector.Add(Extroversion);
        personalityVector.Add(Agreeableness);
        personalityVector.Add(Neuroticism);
    }
	




    public float CheckWeight(string stateVar)
    {
        int index = StateVarDictionary[stateVar];


        return GenerateWeight(index);
        //return 1.0f;
    }




    float GenerateWeight(int index)
    {
        // returns a weight proportional to how much the agent cares about that state variable
        // ie low 'concientiousness' would translate to a disregard for 'time of day' as a relevant variable

        // @TODO add some pseudorandom noise - pcg

        float oContrib = personalityWeightInfluences[0, index] * OpennessToExperience;
        float cContrib = personalityWeightInfluences[1, index] * Concientiousness;
        float eContrib = personalityWeightInfluences[2, index] * Extroversion;
        float aContrib = personalityWeightInfluences[3, index] * Agreeableness;
        float nContrib = personalityWeightInfluences[4, index] * Neuroticism;


        float weight = 1.0f + oContrib + cContrib + eContrib + aContrib + nContrib;



        return weight;
    }

    float GenerateActionModifiers(int index)
    {
        // returns a modifier proportional to how much the state variable is affected by performing an action
        // @TODO add some pseudorandom noise - pcg

        float oContrib = personalityModifierInfluences[0, index] * OpennessToExperience;
        float cContrib = personalityModifierInfluences[1, index] * Concientiousness;
        float eContrib = personalityModifierInfluences[2, index] * Extroversion;
        float aContrib = personalityModifierInfluences[3, index] * Agreeableness;
        float nContrib = personalityModifierInfluences[4, index] * Neuroticism;


        float modifier = oContrib + cContrib + eContrib + aContrib + nContrib;



        return modifier;
    }



    //@TODO could be moved to a global place?
    #region Global data storage

    void BuildPersonalityWeightInfluences()
    {
        // rows ascending
        // timeofday relationship 	wealth	hunger	energy	mood	temper	sociability	soberness resources
        // columns ascending
        // Neuroticism Agreeableness Extroversion Conscientiousness Openness

        // timeofday
        personalityWeightInfluences[0,0] = -2.0f;
        personalityWeightInfluences[1,0] = 3.0f;
        personalityWeightInfluences[2,0] = 0.0f;
        personalityWeightInfluences[3,0] = 1.0f;
        personalityWeightInfluences[4,0] = 2.0f;
        //relationship
        personalityWeightInfluences[0,1] = -2.0f;
        personalityWeightInfluences[1,1] = 2.0f;
        personalityWeightInfluences[2,1] = -3.0f;
        personalityWeightInfluences[3,1] = -1.0f;
        personalityWeightInfluences[4,1] = 0.0f;
        //wealth
        personalityWeightInfluences[0,2] = 0.0f;
        personalityWeightInfluences[1,2] = 3.0f;
        personalityWeightInfluences[2,2] = 0.0f;
        personalityWeightInfluences[3,2] = 1.0f;
        personalityWeightInfluences[4,2] = -1.0f;
        //hunger
        personalityWeightInfluences[0,3] = 0.0f;
        personalityWeightInfluences[1,3] = -1.0f;
        personalityWeightInfluences[2,3] = 0.0f;
        personalityWeightInfluences[3,3] = -1.0f;
        personalityWeightInfluences[4,3] = 1.0f;
        //energy
        personalityWeightInfluences[0,4] = -1.0f;
        personalityWeightInfluences[1,4] = -2.0f;
        personalityWeightInfluences[2,4] = +2.0f;
        personalityWeightInfluences[3,4] = 0.0f;
        personalityWeightInfluences[4,4] = 1.0f;
        //mood
        personalityWeightInfluences[0,5] = 0.0f;
        personalityWeightInfluences[1,5] = -2.0f;
        personalityWeightInfluences[2,5] = 2.0f;
        personalityWeightInfluences[3,5] = 0.0f;
        personalityWeightInfluences[4,5] = 2.0f;
        //temper
        personalityWeightInfluences[0,6] = 0.0f;
        personalityWeightInfluences[1,6] = -1.0f;
        personalityWeightInfluences[2,6] = 1.0f;
        personalityWeightInfluences[3,6] = -1.0f;
        personalityWeightInfluences[4,6] = +2.0f;
        //sociability
        personalityWeightInfluences[0,7] = -1.0f;
        personalityWeightInfluences[1,7] = 0.0f;
        personalityWeightInfluences[2,7] = 2.0f;
        personalityWeightInfluences[3,7] = -1.0f;
        personalityWeightInfluences[4,7] = 2.0f;
        //soberness
        personalityWeightInfluences[0,8] = 0.0f;
        personalityWeightInfluences[1,8] = -2.0f;
        personalityWeightInfluences[2,8] = 1.0f;
        personalityWeightInfluences[3,8] = -1.0f;
        personalityWeightInfluences[4,8] = 1.0f;
        //resources
        personalityWeightInfluences[0, 9] = 0.0f;
        personalityWeightInfluences[1, 9] = -2.0f;
        personalityWeightInfluences[2, 9] = 1.0f;
        personalityWeightInfluences[3, 9] = -1.0f;
        personalityWeightInfluences[4, 9] = 1.0f;


        // limit the values to within -0.1 to +0.1
        for (int i = 0; i < personalityWeightInfluences.GetLength(0); i++)
            for (int j = 0; j < personalityWeightInfluences.GetLength(1); j++)
                personalityWeightInfluences[i, j] *= 0.02f;
    }


    void BuildPersonalityModifierInfluences()
    {
        // rows ascending
        // timeofday relationship 	wealth	hunger	energy	mood	temper	sociability	soberness resources
        // columns ascending
        // Neuroticism Agreeableness Extroversion Conscientiousness Openness

        //relationship
        personalityModifierInfluences[0, 0] = -1.0f;
        personalityModifierInfluences[1, 0] = 0.0f;
        personalityModifierInfluences[2, 0] = +1.0f;
        personalityModifierInfluences[3, 0] = +1.0f;
        personalityModifierInfluences[4, 0] = -2.0f;
        //hunger
        personalityModifierInfluences[0, 1] = 0.0f;
        personalityModifierInfluences[1, 1] = -1.0f;
        personalityModifierInfluences[2, 1] = 0.0f;
        personalityModifierInfluences[3, 1] = -1.0f;
        personalityModifierInfluences[4, 1] = +1.0f;
        //energy
        personalityModifierInfluences[0, 2] = 0.0f;
        personalityModifierInfluences[1, 2] = 0.0f;
        personalityModifierInfluences[2, 2] = +1.0f;
        personalityModifierInfluences[3, 2] = +1.0f;
        personalityModifierInfluences[4, 2] = -1.0f;
        //wealth
        personalityModifierInfluences[0, 3] = -1.0f;
        personalityModifierInfluences[1, 3] = +1.0f;
        personalityModifierInfluences[2, 3] = 0.0f;
        personalityModifierInfluences[3, 3] = +1.0f;
        personalityModifierInfluences[4, 3] = -2.0f;
        //mood
        personalityModifierInfluences[0, 4] = +1.0f;
        personalityModifierInfluences[1, 4] = +1.0f;
        personalityModifierInfluences[2, 4] = 0.0f;
        personalityModifierInfluences[3, 4] = +2.0f;
        personalityModifierInfluences[4, 4] = +2.0f;
        //temper
        personalityModifierInfluences[0, 5] = 0.0f;
        personalityModifierInfluences[1, 5] = +1.0f;
        personalityModifierInfluences[2, 5] = 0.0f;
        personalityModifierInfluences[3, 5] = +1.0f;
        personalityModifierInfluences[4, 5] = -2.0f;
        //sociability
        personalityModifierInfluences[0, 6] = +1.0f;
        personalityModifierInfluences[1, 6] = -1.0f;
        personalityModifierInfluences[2, 6] = +1.0f;
        personalityModifierInfluences[3, 6] = -1.0f;
        personalityModifierInfluences[4, 6] = +1.0f;
        //soberness
        personalityModifierInfluences[0, 7] = 0.0f;
        personalityModifierInfluences[1, 7] = -1.0f;
        personalityModifierInfluences[2, 7] = +1.0f;
        personalityModifierInfluences[3, 7] = -1.0f;
        personalityModifierInfluences[4, 7] = 0.0f;
        //resources
        personalityModifierInfluences[0, 8] = 0.0f;
        personalityModifierInfluences[1, 8] = -2.0f;
        personalityModifierInfluences[2, 8] = 1.0f;
        personalityModifierInfluences[3, 8] = -1.0f;
        personalityModifierInfluences[4, 8] = 1.0f;
        


        // limit the values to within -1.0 to +1.0
        for (int i = 0; i < personalityModifierInfluences.GetLength(0); i++)
            for (int j = 0; j < personalityModifierInfluences.GetLength(1); j++)
                personalityModifierInfluences[i, j] *= 0.2f;
    }


    void BuildActionModifiers()
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

        float hungerModifier = GenerateActionModifiers(1);
        float energyModifier = GenerateActionModifiers(2);
        float wealthModifier = GenerateActionModifiers(3);
        float moodModifier = GenerateActionModifiers(4);
        float temperModifier = GenerateActionModifiers(5);
        float sociabilityModifier = GenerateActionModifiers(6);
        float sobernessModifier = GenerateActionModifiers(7);
        float resourcesModifier = GenerateActionModifiers(8);


        List<float> buyfoodatmarketValues = new List<float>();
        buyfoodatmarketValues.Add(-4.0f + hungerModifier);
        buyfoodatmarketValues.Add(-1.0f + energyModifier);
        buyfoodatmarketValues.Add(-3.0f);
        buyfoodatmarketValues.Add(+0.0f);
        buyfoodatmarketValues.Add(+1.0f);
        buyfoodatmarketValues.Add(-1.0f);
        buyfoodatmarketValues.Add(0.0f);
        buyfoodatmarketValues.Add(0.0f);
        List<float> eatfoodathomeValues = new List<float>();
        eatfoodathomeValues.Add(-4.0f + hungerModifier);
        eatfoodathomeValues.Add(-1.0f + energyModifier);
        eatfoodathomeValues.Add(-2.0f);
        eatfoodathomeValues.Add(0.0f);
        eatfoodathomeValues.Add(+1.0f);
        eatfoodathomeValues.Add(3.0f);
        eatfoodathomeValues.Add(+0.0f);
        eatfoodathomeValues.Add(-2.0f);
        List<float> stealfoodValues = new List<float>();
        stealfoodValues.Add(-2.0f + hungerModifier);
        stealfoodValues.Add(-2.0f + energyModifier);
        stealfoodValues.Add(+0.0f);
        stealfoodValues.Add(-1.0f);
        stealfoodValues.Add(-1.0f);
        stealfoodValues.Add(+0.0f);
        stealfoodValues.Add(+0.0f);
        stealfoodValues.Add(+1.0f);
        List<float> sleepathomeValues = new List<float>();
        sleepathomeValues.Add(+1.0f + hungerModifier);
        sleepathomeValues.Add(+3.0f + energyModifier);
        sleepathomeValues.Add(+0.0f);
        sleepathomeValues.Add(1.0f);
        sleepathomeValues.Add(+3.0f);
        sleepathomeValues.Add(+2.0f);
        sleepathomeValues.Add(+4.0f);
        sleepathomeValues.Add(+0.0f);
        List<float> sleeponthespotValues = new List<float>();
        sleeponthespotValues.Add(+1.0f + hungerModifier);
        sleeponthespotValues.Add(+1.0f);
        sleeponthespotValues.Add(+0.0f);
        sleeponthespotValues.Add(-1.0f);
        sleeponthespotValues.Add(1.0f);
        sleeponthespotValues.Add(+0.0f);
        sleeponthespotValues.Add(+4.0f);
        sleeponthespotValues.Add(+0.0f);
        List<float> drinkattavernValues = new List<float>();
        drinkattavernValues.Add(+2.0f + hungerModifier);
        drinkattavernValues.Add(-2.0f);
        drinkattavernValues.Add(-2.0f);
        drinkattavernValues.Add(+1.0f);
        drinkattavernValues.Add(0.0f);
        drinkattavernValues.Add(-2.0f);
        drinkattavernValues.Add(-4.0f);
        drinkattavernValues.Add(-0.0f);
        List<float> drinkAmicablyValues = new List<float>();
        drinkAmicablyValues.Add(+2.0f + hungerModifier);
        drinkAmicablyValues.Add(-2.0f);
        drinkAmicablyValues.Add(-2.0f);
        drinkAmicablyValues.Add(+1.0f);
        drinkAmicablyValues.Add(0.0f);
        drinkAmicablyValues.Add(-2.0f);
        drinkAmicablyValues.Add(-4.0f);
        drinkAmicablyValues.Add(0.0f);
        List<float> drinkBeligerentlyValues = new List<float>();
        drinkBeligerentlyValues.Add(+2.0f + hungerModifier);
        drinkBeligerentlyValues.Add(-2.0f);
        drinkBeligerentlyValues.Add(-2.0f);
        drinkBeligerentlyValues.Add(-1.0f);
        drinkBeligerentlyValues.Add(+2.0f);
        drinkBeligerentlyValues.Add(-2.0f);
        drinkBeligerentlyValues.Add(-4.0f);
        drinkBeligerentlyValues.Add(+0.0f);
        List<float> prayatchurchValues = new List<float>();
        prayatchurchValues.Add(+2.0f + hungerModifier);
        prayatchurchValues.Add(-1.0f);
        prayatchurchValues.Add(0.0f);
        prayatchurchValues.Add(+1.0f);
        prayatchurchValues.Add(+3.0f);
        prayatchurchValues.Add(-1.0f);
        prayatchurchValues.Add(+0.0f);
        prayatchurchValues.Add(-0.0f);
        List<float> gofishingValues = new List<float>();
        gofishingValues.Add(2.0f + hungerModifier);
        gofishingValues.Add(-1.0f);
        gofishingValues.Add(0.0f);
        gofishingValues.Add(+1.0f);
        gofishingValues.Add(+2.0f);
        gofishingValues.Add(+1.0f);
        gofishingValues.Add(+0.0f);
        gofishingValues.Add(+3.0f);
        List<float> sleeponthejobValues = new List<float>();
        sleeponthejobValues.Add(+1.0f + hungerModifier);
        sleeponthejobValues.Add(+2.0f);
        sleeponthejobValues.Add(+0.0f);
        sleeponthejobValues.Add(+1.0f);
        sleeponthejobValues.Add(1.0f);
        sleeponthejobValues.Add(+0.0f);
        sleeponthejobValues.Add(+2.0f);
        sleeponthejobValues.Add(+0.0f);
        List<float> workdiligentlyValues = new List<float>();
        workdiligentlyValues.Add(+3.0f + hungerModifier);
        workdiligentlyValues.Add(-3.0f);
        workdiligentlyValues.Add(+0.0f);
        workdiligentlyValues.Add(-1.0f);
        workdiligentlyValues.Add(0.0f);
        workdiligentlyValues.Add(+2.0f);
        workdiligentlyValues.Add(+0.0f);
        workdiligentlyValues.Add(+4.0f);
        List<float> sellwaresValues = new List<float>();
        sellwaresValues.Add(+3.0f + hungerModifier);
        sellwaresValues.Add(-3.0f);
        sellwaresValues.Add(+4.0f);
        sellwaresValues.Add(+0.0f);
        sellwaresValues.Add(0.0f);
        sellwaresValues.Add(-2.0f);
        sellwaresValues.Add(+0.0f);
        sellwaresValues.Add(-3.0f);
        List<float> socialiseNiceValues = new List<float>();
        socialiseNiceValues.Add(+2.0f + hungerModifier);
        socialiseNiceValues.Add(-1.0f);
        socialiseNiceValues.Add(+0.0f);
        socialiseNiceValues.Add(+1.0f);
        socialiseNiceValues.Add(+0.0f);
        socialiseNiceValues.Add(-1.0f);
        socialiseNiceValues.Add(+0.0f);
        socialiseNiceValues.Add(-1.0f);
        List<float> socialiseMeanValues = new List<float>();
        socialiseMeanValues.Add(+2.0f + hungerModifier);
        socialiseMeanValues.Add(-1.0f);
        socialiseMeanValues.Add(+0.0f);
        socialiseMeanValues.Add(-1.0f);
        socialiseMeanValues.Add(+0.0f);
        socialiseMeanValues.Add(-2.0f);
        socialiseMeanValues.Add(0.0f);
        socialiseMeanValues.Add(0.0f);
        List<float> assistValues = new List<float>();
        assistValues.Add(+3.0f + hungerModifier);
        assistValues.Add(-2.0f);
        assistValues.Add(+0.0f);
        assistValues.Add(+1.0f);
        assistValues.Add(0.0f);
        assistValues.Add(-1.0f);
        assistValues.Add(+0.0f);
        assistValues.Add(-1.0f);
    }

    private void BuildStateVarDictionary()
    {
        //@TODO fix naming convention to avoid the multiple dictionary entries to the same integer


        // timeofday relationship 	wealth	hunger	energy	mood	temper	sociability	soberness
        StateVarDictionary.Add("TimeOfDay", 0);
        StateVarDictionary.Add("timeofday", 0);
        StateVarDictionary.Add("Time", 0);

        StateVarDictionary.Add("Relationship", 1);
        StateVarDictionary.Add("relationship", 1);
        StateVarDictionary.Add("Agent1", 2);

        StateVarDictionary.Add("Wealth", 2);
        StateVarDictionary.Add("wealth", 2);

        StateVarDictionary.Add("Hunger", 3);
        StateVarDictionary.Add("hunger", 3);

        StateVarDictionary.Add("Energy", 4);
        StateVarDictionary.Add("energy", 4);

        StateVarDictionary.Add("Mood", 5);
        StateVarDictionary.Add("mood", 5);

        StateVarDictionary.Add("Temper", 6);
        StateVarDictionary.Add("temper", 6);

        StateVarDictionary.Add("Sociability", 7);
        StateVarDictionary.Add("sociability", 7);

        StateVarDictionary.Add("Soberness", 8);
        StateVarDictionary.Add("soberness", 8);

        StateVarDictionary.Add("Resources", 9);
        StateVarDictionary.Add("resources", 9);

    }

    #endregion

}
