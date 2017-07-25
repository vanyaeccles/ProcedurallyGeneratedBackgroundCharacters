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
    public float NonNeuroticism;


    [Header("Agent State Parameters")]
    public AgentStateVarFloat hunger;
    public AgentStateVarFloat energy, wealth, mood, temper, sociability, soberness, resources;

    [Header("Relationships")]
    public AgentStateParameter agent1;

    [Header("Global")]
    public AgentStateParameter timeofday;



    // maps from a state variable (as a string) to its integer index
    private Dictionary<string, int> StateVarDictionary = new Dictionary<string, int>();

    // contains the influences that personality parameters have on state variable decisions
    // ie how does extroversion level relate to importance of sociability, @TODO could replace this with a string tuple-value dictionary
    private float[,] personalityWeightInfluences = new float[5,10];

    // How the personality variables influence state variable modifiers (ie when performing an action)
    private float[,] personalityModifierInfluences = new float[5, 10];

    // the dictionary thats used to execute effect of behaviours
    public Dictionary<string, List<float>> actionModifierDictionary = new Dictionary<string, List<float>>();



    void Awake()
    {
        // the same weight influences for all agents
        BuildPersonalityWeightInfluences();
        BuildStateVarDictionary();


        BuildPersonalityModifierInfluences();
        GenerateActionModifiers();

        

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
        personalityVector.Add(NonNeuroticism);
    }



    #region Making Decision Weights


    public float CheckWeight(string stateVar)
    {
        //gets the index from the state variable provided as a string
        int index = StateVarDictionary[stateVar];

        return GenerateWeight(index);
    }

    float GenerateWeight(int index)
    {
        // returns a weight proportional to how much the agent cares about that state variable
        // ie low 'concientiousness' would translate to a disregard for 'time of day' as a relevant variable

        // map from -1.0,1.0 range to 0.0,1.0
        float normO = NormaliseFloat(OpennessToExperience, 1.0f, -1.0f);
        float normC = NormaliseFloat(Concientiousness, 1.0f, -1.0f);
        float normE = NormaliseFloat(Extroversion, 1.0f, -1.0f);
        float normA = NormaliseFloat(Agreeableness, 1.0f, -1.0f);
        float normN = NormaliseFloat(NonNeuroticism, 1.0f, -1.0f);



        // @TODO add some pseudorandom noise - pcg

        float oContrib = personalityWeightInfluences[0, index] * normO;
        float cContrib = personalityWeightInfluences[1, index] * normC;
        float eContrib = personalityWeightInfluences[2, index] * normE;
        float aContrib = personalityWeightInfluences[3, index] * normA;
        float nContrib = personalityWeightInfluences[4, index] * normN;


        float weight = 1.0f + oContrib + cContrib + eContrib + aContrib + nContrib;

        return weight;
    }

    float NormaliseFloat(float inputVal, float max, float min)
    {
        return (inputVal - min) / (max - min);
    }

    #endregion




    #region Making Action Modifiers

    // this builds the agents action modifier vectors and stores them in a dictionary
    void GenerateActionModifiers()
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

        // NB Important to ensure that the list of state parameters is in the same order
        // as the modification vectors in the dictionary

        // get the agent personality specific modifiers 
        float hungerModifier = GetActionModifier(1);
        float energyModifier = GetActionModifier(2);
        float wealthModifier = GetActionModifier(3);
        float moodModifier = GetActionModifier(4);
        float temperModifier = GetActionModifier(5);
        float sociabilityModifier = GetActionModifier(6);
        float sobernessModifier = GetActionModifier(7);
        float resourcesModifier = GetActionModifier(8);

        //Check values
        //Debug.Log("hunger" + hungerModifier);
        //Debug.Log("energy" + energyModifier);
        //Debug.Log("wealth" + wealthModifier);
        //Debug.Log("mood" + moodModifier);
        //Debug.Log("temper" + temperModifier);
        //Debug.Log("sociability" + sociabilityModifier);
        //Debug.Log("soberness" + sobernessModifier);
        //Debug.Log("resources" + resourcesModifier);


        // Each action modifier is a combination of harcoded values that represent an action's effect on a variable
        // this is combined with a specific modifier generated from the agent's personality

        #region creating the action modifiers

        List<float> buyfoodatmarketValues = new List<float>();
        buyfoodatmarketValues.Add(-4.0f + hungerModifier);
        buyfoodatmarketValues.Add(-1.0f + energyModifier);
        buyfoodatmarketValues.Add(-3.0f + wealthModifier);
        buyfoodatmarketValues.Add(+0.0f + moodModifier);
        buyfoodatmarketValues.Add(+1.0f + temperModifier);
        buyfoodatmarketValues.Add(-1.0f + sociabilityModifier);
        buyfoodatmarketValues.Add(0.0f + sobernessModifier);
        buyfoodatmarketValues.Add(0.0f + resourcesModifier);

        List<float> eatfoodathomeValues = new List<float>();
        eatfoodathomeValues.Add(-4.0f + hungerModifier);
        eatfoodathomeValues.Add(-1.0f + energyModifier);
        eatfoodathomeValues.Add(-2.0f + wealthModifier);
        eatfoodathomeValues.Add(0.0f + moodModifier);
        eatfoodathomeValues.Add(+1.0f + temperModifier);
        eatfoodathomeValues.Add(3.0f + sociabilityModifier);
        eatfoodathomeValues.Add(+0.0f + sobernessModifier);
        eatfoodathomeValues.Add(-2.0f + resourcesModifier);

        List<float> stealfoodValues = new List<float>();
        stealfoodValues.Add(-2.0f + hungerModifier);
        stealfoodValues.Add(-2.0f + energyModifier);
        stealfoodValues.Add(+0.0f + wealthModifier);
        stealfoodValues.Add(-1.0f + moodModifier);
        stealfoodValues.Add(-1.0f + temperModifier);
        stealfoodValues.Add(+0.0f + sociabilityModifier);
        stealfoodValues.Add(+0.0f + sobernessModifier);
        stealfoodValues.Add(+1.0f + resourcesModifier);

        List<float> sleepathomeValues = new List<float>();
        sleepathomeValues.Add(+1.0f + hungerModifier);
        sleepathomeValues.Add(+3.0f + energyModifier);
        sleepathomeValues.Add(+0.0f + wealthModifier);
        sleepathomeValues.Add(1.0f + moodModifier);
        sleepathomeValues.Add(+3.0f + temperModifier);
        sleepathomeValues.Add(+2.0f + sociabilityModifier);
        sleepathomeValues.Add(+4.0f + sobernessModifier);
        sleepathomeValues.Add(+0.0f + resourcesModifier);

        List<float> sleeponthespotValues = new List<float>();
        sleeponthespotValues.Add(+1.0f + hungerModifier);
        sleeponthespotValues.Add(+1.0f + energyModifier);
        sleeponthespotValues.Add(+0.0f + wealthModifier);
        sleeponthespotValues.Add(-1.0f + moodModifier);
        sleeponthespotValues.Add(1.0f + temperModifier);
        sleeponthespotValues.Add(+0.0f + sociabilityModifier);
        sleeponthespotValues.Add(+4.0f + sobernessModifier);
        sleeponthespotValues.Add(+0.0f + resourcesModifier);

        List<float> drinkattavernValues = new List<float>();
        drinkattavernValues.Add(+2.0f + hungerModifier);
        drinkattavernValues.Add(-2.0f + energyModifier);
        drinkattavernValues.Add(-2.0f + wealthModifier);
        drinkattavernValues.Add(+1.0f + moodModifier);
        drinkattavernValues.Add(0.0f + temperModifier);
        drinkattavernValues.Add(-2.0f + sociabilityModifier);
        drinkattavernValues.Add(-4.0f + sobernessModifier);
        drinkattavernValues.Add(-0.0f + resourcesModifier);

        List<float> drinkamicablyValues = new List<float>();
        drinkamicablyValues.Add(+2.0f + hungerModifier);
        drinkamicablyValues.Add(-2.0f + energyModifier);
        drinkamicablyValues.Add(-2.0f + wealthModifier);
        drinkamicablyValues.Add(+1.0f + moodModifier);
        drinkamicablyValues.Add(0.0f + temperModifier);
        drinkamicablyValues.Add(-2.0f + sociabilityModifier);
        drinkamicablyValues.Add(-4.0f + sobernessModifier);
        drinkamicablyValues.Add(0.0f + resourcesModifier);

        List<float> drinkbeligerentlyValues = new List<float>();
        drinkbeligerentlyValues.Add(+2.0f + hungerModifier);
        drinkbeligerentlyValues.Add(-2.0f + energyModifier);
        drinkbeligerentlyValues.Add(-2.0f + wealthModifier);
        drinkbeligerentlyValues.Add(-1.0f + moodModifier);
        drinkbeligerentlyValues.Add(+2.0f + temperModifier);
        drinkbeligerentlyValues.Add(-2.0f + sociabilityModifier);
        drinkbeligerentlyValues.Add(-4.0f + sobernessModifier);
        drinkbeligerentlyValues.Add(+0.0f + resourcesModifier);

        List<float> prayatchurchValues = new List<float>();
        prayatchurchValues.Add(+2.0f + hungerModifier);
        prayatchurchValues.Add(-1.0f + energyModifier);
        prayatchurchValues.Add(0.0f + wealthModifier);
        prayatchurchValues.Add(+1.0f + moodModifier);
        prayatchurchValues.Add(+3.0f + temperModifier);
        prayatchurchValues.Add(-1.0f + sociabilityModifier);
        prayatchurchValues.Add(+0.0f + sobernessModifier);
        prayatchurchValues.Add(-0.0f + resourcesModifier);

        List<float> gofishingValues = new List<float>();
        gofishingValues.Add(2.0f + hungerModifier);
        gofishingValues.Add(-1.0f + energyModifier);
        gofishingValues.Add(0.0f + wealthModifier);
        gofishingValues.Add(+1.0f + moodModifier);
        gofishingValues.Add(+2.0f + temperModifier);
        gofishingValues.Add(+1.0f + sociabilityModifier);
        gofishingValues.Add(+0.0f + sobernessModifier);
        gofishingValues.Add(+3.0f + resourcesModifier);

        List<float> sleeponthejobValues = new List<float>();
        sleeponthejobValues.Add(+1.0f + hungerModifier);
        sleeponthejobValues.Add(+2.0f + energyModifier);
        sleeponthejobValues.Add(+0.0f + wealthModifier);
        sleeponthejobValues.Add(+1.0f + moodModifier);
        sleeponthejobValues.Add(1.0f + temperModifier);
        sleeponthejobValues.Add(+0.0f + sociabilityModifier);
        sleeponthejobValues.Add(+2.0f + sobernessModifier);
        sleeponthejobValues.Add(+0.0f + resourcesModifier);

        List<float> workdiligentlyValues = new List<float>();
        workdiligentlyValues.Add(+3.0f + hungerModifier);
        workdiligentlyValues.Add(-3.0f + energyModifier);
        workdiligentlyValues.Add(+0.0f + wealthModifier);
        workdiligentlyValues.Add(-1.0f + moodModifier);
        workdiligentlyValues.Add(0.0f + temperModifier);
        workdiligentlyValues.Add(+2.0f + sociabilityModifier);
        workdiligentlyValues.Add(+0.0f + sobernessModifier);
        workdiligentlyValues.Add(+4.0f + resourcesModifier);

        List<float> sellwaresValues = new List<float>();
        sellwaresValues.Add(+3.0f + hungerModifier);
        sellwaresValues.Add(-3.0f + energyModifier);
        sellwaresValues.Add(+4.0f + wealthModifier);
        sellwaresValues.Add(+0.0f + moodModifier);
        sellwaresValues.Add(0.0f + temperModifier);
        sellwaresValues.Add(-2.0f + sociabilityModifier);
        sellwaresValues.Add(+0.0f + sobernessModifier);
        sellwaresValues.Add(-3.0f + resourcesModifier);

        List<float> socialiseniceValues = new List<float>();
        socialiseniceValues.Add(+2.0f + hungerModifier);
        socialiseniceValues.Add(-1.0f + energyModifier);
        socialiseniceValues.Add(+0.0f + wealthModifier);
        socialiseniceValues.Add(+1.0f + moodModifier);
        socialiseniceValues.Add(+0.0f + temperModifier);
        socialiseniceValues.Add(-1.0f + sociabilityModifier);
        socialiseniceValues.Add(+0.0f + sobernessModifier);
        socialiseniceValues.Add(-1.0f + resourcesModifier);

        List<float> socialisemeanValues = new List<float>();
        socialisemeanValues.Add(+2.0f + hungerModifier);
        socialisemeanValues.Add(-1.0f + energyModifier);
        socialisemeanValues.Add(+0.0f + wealthModifier);
        socialisemeanValues.Add(-1.0f + moodModifier);
        socialisemeanValues.Add(+0.0f + temperModifier);
        socialisemeanValues.Add(-2.0f + sociabilityModifier);
        socialisemeanValues.Add(0.0f + sobernessModifier);
        socialisemeanValues.Add(0.0f + resourcesModifier);

        List<float> assistValues = new List<float>();
        assistValues.Add(+3.0f + hungerModifier);
        assistValues.Add(-2.0f + energyModifier);
        assistValues.Add(+0.0f + wealthModifier);
        assistValues.Add(+1.0f + moodModifier);
        assistValues.Add(0.0f + temperModifier);
        assistValues.Add(-1.0f + sociabilityModifier);
        assistValues.Add(+0.0f + sobernessModifier);
        assistValues.Add(-1.0f + resourcesModifier);

        #endregion

        //clear the dictionary in case its being rebuilt
        actionModifierDictionary.Clear();

        //add a string-vector entry for every leaf action that can be performed
        actionModifierDictionary.Add("buyfoodatmarket", buyfoodatmarketValues);
        actionModifierDictionary.Add("eatfoodathome", eatfoodathomeValues);
        actionModifierDictionary.Add("stealfood", stealfoodValues);
        actionModifierDictionary.Add("sleepathome", sleepathomeValues);
        actionModifierDictionary.Add("sleeponthespot", sleeponthespotValues);
        actionModifierDictionary.Add("drinkattavern", drinkattavernValues);
        actionModifierDictionary.Add("drinkamicably", drinkamicablyValues);
        actionModifierDictionary.Add("drinkbelligerently", drinkbeligerentlyValues);
        actionModifierDictionary.Add("prayatchurch", prayatchurchValues);
        actionModifierDictionary.Add("gofishing", gofishingValues);
        actionModifierDictionary.Add("workdiligently", workdiligentlyValues);
        actionModifierDictionary.Add("sellwares", sellwaresValues);
        actionModifierDictionary.Add("sleeponthejob", sleeponthejobValues);
        actionModifierDictionary.Add("socialisenice", socialiseniceValues);
        actionModifierDictionary.Add("socialisemean", socialisemeanValues);
    }

    // gets the action modifier for a given state variable (index)
    float GetActionModifier(int varIndex)
    {
        // returns a modifier proportional to how much the state variable is affected by performing an action

        // @TODO add some pseudorandom noise - pcg

        float oContrib = personalityModifierInfluences[0, varIndex] * OpennessToExperience;
        float cContrib = personalityModifierInfluences[1, varIndex] * Concientiousness;
        float eContrib = personalityModifierInfluences[2, varIndex] * Extroversion;
        float aContrib = personalityModifierInfluences[3, varIndex] * Agreeableness;
        float nContrib = personalityModifierInfluences[4, varIndex] * NonNeuroticism;

        // contribution from all of the personality values
        float modifier = oContrib + cContrib + eContrib + aContrib + nContrib;

        return modifier;
    }

    #endregion






    //@TODO could be moved to a global place?
    #region Global data storage

    /*
     * This is hardcoded stuff for the agents to build weights and action modifiers from
     */
    
    void BuildPersonalityWeightInfluences()
    {
        // These signify how much personality values influence the weights that are generated for each state variable

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
