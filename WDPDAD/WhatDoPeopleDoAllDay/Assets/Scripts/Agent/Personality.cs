using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This is the personality class, holds the weight and action modifiers and operates the pseudorandom number generator
public class Personality : MonoBehaviour {

    public Character character;
    

    [Header("Personality Values (please enter at least 2 decimal places)")]
    public float OpennessToExperience;
    public float Concientiousness;
    public float Extroversion;
    public float Agreeableness;
    public float Neuroticism;
    List<float> personalityVector = new List<float>();

    //modifiers
    //[HideInInspector]
    public float relationshipModifier;
    //[HideInInspector]
    public float hungerModifier;
    //[HideInInspector]
    public float energyModifier;
    //[HideInInspector]
    public float wealthModifier;
    //[HideInInspector]
    public float moodModifier;
    //[HideInInspector]
    public float temperModifier;
    //[HideInInspector]
    public float sociabilityModifier;
    //[HideInInspector]
    public float sobernessModifier;
    //[HideInInspector]
    public float resourcesModifier;









    [Header("Agent State Parameters")]
    public AgentStateVarFloat hunger;
    public AgentStateVarFloat energy, wealth, mood, temper, sociability, soberness, resources;
 

    [Header("Relationships")]
    public List<Relationship> AgentRelationships = new List<Relationship>();
    public List<AgentStateVarFloat> AgentRelationshipValues = new List<AgentStateVarFloat>();
    //private GameObject RelationshipsHolder;

    [Header("Global")]
    public AgentStateVarFloat timeofday;
    public GlobalAgentProperties globalAgentInfo;

    public Dictionary<string, AgentStateVarFloat> stateParameterDictionary = new Dictionary<string, AgentStateVarFloat>();

    // PCG variables
    private PRNG prng;
    int seed1, seed2;
    List<double> randModifiers = new List<double>();
    List<double> randWeightModifiers = new List<double>();


    public void Awake()
    {
        GenerateStateParameterDictionary();

        // get the global agent info
        globalAgentInfo = GameObject.Find("GlobalValues").GetComponent<GlobalAgentProperties>();

        prng = GetComponent<PRNG>();
    }

    void Start()
    {
        //TestGaussianGenerator();
    }


    // this is called to procedurally generate the action modifiers and the weights
    public void GeneratePersonality(float O, float C, float E, float A, float N)
    {
        //sets the personality values
        OpennessToExperience = O;
        Concientiousness = C;
        Extroversion = E;
        Agreeableness = A;
        Neuroticism = N;


        // get the seed numbers from the agents personality values for PCG
        GenerateSeeds();
        GenerateRandomGaussianNumbers();


        GenerateActionModifiers();
    }


    #region Making Decision Weights


    public float CheckWeight(string stateVar)
    {
        //gets the index from the state variable provided as a string
        int index0 = globalAgentInfo.StateVarDictionary[stateVar];

        return GenerateWeight(index0);
    }

    // these are generated as they are needed by the considerations
    float GenerateWeight(int index1)
    {
        // returns a weight proportional to how much the agent cares about that state variable
        // ie high 'concientiousness' would translate to a regard for 'resources' as a relevant variable



        // map from -1.0,1.0 range to 0.0,1.0
        //float normO = NormaliseFloat(OpennessToExperience, 1.0f, -1.0f);
        //float normC = NormaliseFloat(Concientiousness, 1.0f, -1.0f);
        //float normE = NormaliseFloat(Extroversion, 1.0f, -1.0f);
        //float normA = NormaliseFloat(Agreeableness, 1.0f, -1.0f);
        //float normN = NormaliseFloat(Neuroticism, 1.0f, -1.0f);

        // use personality values as is (permits weights of < 1.0)
        float normO = OpennessToExperience;
        float normC = Concientiousness;
        float normE = Extroversion;
        float normA = Agreeableness;
        float normN = Neuroticism;


        float oContrib = globalAgentInfo.personalityWeightInfluences[0, index1] * normO;
        float cContrib = globalAgentInfo.personalityWeightInfluences[1, index1] * normC;
        float eContrib = globalAgentInfo.personalityWeightInfluences[2, index1] * normE;
        float aContrib = globalAgentInfo.personalityWeightInfluences[3, index1] * normA;
        float nContrib = globalAgentInfo.personalityWeightInfluences[4, index1] * normN;


        //weight is composed of a mean value (from gaussian with mean of 1.0f) + contributions from personality
        float weight = (float)randWeightModifiers[index1] + oContrib + cContrib + eContrib + aContrib + nContrib;

        //Debug.Log(weight);
        //float contrib = oContrib + cContrib + eContrib + aContrib + nContrib;
        //Debug.Log("diff: " + contrib);

        return weight;
    }

    float NormaliseFloat(float inputVal, float max, float min)
    {
        //normalises a float from a specified range to [0,1]
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
        relationshipModifier = GetActionModifier(0);
        hungerModifier = GetActionModifier(1);
        energyModifier = GetActionModifier(2);
        wealthModifier = GetActionModifier(3);
        moodModifier = GetActionModifier(4);
        temperModifier = GetActionModifier(5);
        sociabilityModifier = GetActionModifier(6);
        sobernessModifier = GetActionModifier(7);
        resourcesModifier = GetActionModifier(8);

        

        //Check values
        //Debug.Log("relationship " + relationshipModifier);
        //Debug.Log("hunger " + hungerModifier);
        //Debug.Log("energy " + energyModifier);
        //Debug.Log("wealth " + wealthModifier);
        //Debug.Log("mood " + moodModifier);
        //Debug.Log("temper " + temperModifier);
        //Debug.Log("sociability " + sociabilityModifier);
        //Debug.Log("soberness " + sobernessModifier);
        //Debug.Log("resources " + resourcesModifier);


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

        

        List<float> beforestealfoodValues = new List<float>();
        beforestealfoodValues.Add(-2.0f);
        beforestealfoodValues.Add(-2.0f);
        beforestealfoodValues.Add(+0.0f);
        beforestealfoodValues.Add(-1.0f);
        beforestealfoodValues.Add(-1.0f);
        beforestealfoodValues.Add(+0.0f);
        beforestealfoodValues.Add(+0.0f);
        beforestealfoodValues.Add(+1.0f);

       // for (int i = 0; i < beforestealfoodValues.Count; i++)
        //    Debug.Log("before" + i + " " + beforestealfoodValues[i]);

        List<float> stealfoodValues = new List<float>();
        stealfoodValues.Add(-2.0f + hungerModifier);
        stealfoodValues.Add(-2.0f + energyModifier);
        stealfoodValues.Add(+0.0f + wealthModifier);
        stealfoodValues.Add(-1.0f + moodModifier);
        stealfoodValues.Add(-1.0f + temperModifier);
        stealfoodValues.Add(+0.0f + sociabilityModifier);
        stealfoodValues.Add(+0.0f + sobernessModifier);
        stealfoodValues.Add(+1.0f + resourcesModifier);

        //for (int i = 0; i < stealfoodValues.Count; i++)
         //   Debug.Log("after" + i + " " + stealfoodValues[i]);


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
        globalAgentInfo.actionModifierDictionary.Clear();

        //add a string-vector entry for every leaf action that can be performed
        globalAgentInfo.actionModifierDictionary.Add("buyfoodatmarket", buyfoodatmarketValues);
        globalAgentInfo.actionModifierDictionary.Add("eatfoodathome", eatfoodathomeValues);
        globalAgentInfo.actionModifierDictionary.Add("stealfood", stealfoodValues);
        globalAgentInfo.actionModifierDictionary.Add("sleepathome", sleepathomeValues);
        globalAgentInfo.actionModifierDictionary.Add("sleeponthespot", sleeponthespotValues);
        globalAgentInfo.actionModifierDictionary.Add("drinkattavern", drinkattavernValues);
        globalAgentInfo.actionModifierDictionary.Add("drinkamicably", drinkamicablyValues);
        globalAgentInfo.actionModifierDictionary.Add("drinkbelligerently", drinkbeligerentlyValues);
        globalAgentInfo.actionModifierDictionary.Add("prayatchurch", prayatchurchValues);
        globalAgentInfo.actionModifierDictionary.Add("gofishing", gofishingValues);
        globalAgentInfo.actionModifierDictionary.Add("workdiligently", workdiligentlyValues);
        globalAgentInfo.actionModifierDictionary.Add("sellwares", sellwaresValues);
        globalAgentInfo.actionModifierDictionary.Add("sleeponthejob", sleeponthejobValues);
        globalAgentInfo.actionModifierDictionary.Add("socialisenice", socialiseniceValues);
        globalAgentInfo.actionModifierDictionary.Add("socialisemean", socialisemeanValues);
    }

    // gets the action modifier for a given state variable (index)
    float GetActionModifier(int varIndex)
    {
        // returns a modifier proportional to how much the state variable is affected by performing an action
        float oContrib = globalAgentInfo.personalityModifierInfluences[0, varIndex] * OpennessToExperience;
        float cContrib = globalAgentInfo.personalityModifierInfluences[1, varIndex] * Concientiousness;
        float eContrib = globalAgentInfo.personalityModifierInfluences[2, varIndex] * Extroversion;
        float aContrib = globalAgentInfo.personalityModifierInfluences[3, varIndex] * Agreeableness;
        float nContrib = globalAgentInfo.personalityModifierInfluences[4, varIndex] * Neuroticism;

        // contribution from all of the personality values, 
        //the mean of the PCG distribution
        float originalModifier = oContrib + cContrib + eContrib + aContrib + nContrib;

        // add some pseudorandom noise - pcg
        float pcgModifier = originalModifier + (float)randModifiers[varIndex];

        //Debug.Log("Original Modifier: " + originalModifier);
        //Debug.Log("With PCG modification: " + pcgModifier);

        return pcgModifier;
    }


    #endregion




    #region SOCIAL STUFF
    
    public AgentStateParameter GetRelationship(string _name)
    {
        foreach (Relationship relly in AgentRelationships)
        {
            if (relly.nameOfPerson == _name)
            {
                return relly.relationshipValue;
            }    
        }

        //else, if relationship doesn't exist form a new one
        AgentStateVarFloat newRelValue = new AgentStateVarFloat();

        Relationship newRel = new Relationship(_name, newRelValue);

        AgentRelationships.Add(newRel);
        AgentRelationshipValues.Add(newRelValue);

        //Debug.Log(newRelValue.value);

        return newRel.relationshipValue;
    }


    public void ReceiveSocial(string _name, bool mean)
    {

        // search for relationship based on name
        foreach (Relationship rel in AgentRelationships)
        {
            if (rel.nameOfPerson == _name)
            {

                if (mean)
                    character.ReceiveSocialiseMean(rel);
                else
                    character.ReceiveSocialiseNice(rel);
            }
        }
    }


    #endregion




     #region PROCEDURAL CONTENT GENERATION


    void GenerateRandomGaussianNumbers()
    {
        // Initialise with seed numbers
        // provided with 2 integers between 0 and ~ 30000
        prng.RandomInitialise(seed1, seed2);


        randModifiers.Clear();
        randWeightModifiers.Clear();

        // Generate the random Gaussian numbers for the action modifiers
        double mean = 0.0f;
        double stdev = 0.5f;
        for (int i = 0; i < 9; i++)
        {
            // assuming the seed is the same, this will generate a the same list of numbers picked from a gaussian dist
            randModifiers.Add(prng.RandomGaussian(mean, stdev));
            //Debug.Log(prng.RandomGaussian(mean, stdev));
        }

        // Generate the random Gaussian numbers for the parameter weights
        // reset the mean + stddev for weights, these need to be less dramatic @TODO test this
        mean = 1.0f; // mean parameter weight is 1
        stdev = 0.3f;
        for (int i = 0; i < 20; i++)
        {
            randWeightModifiers.Add(prng.RandomGaussian(mean, stdev));
        }
    }




    void GenerateSeeds()
    {
        // makes two seeds between 0 and 30000, derived from personality values

        // clear the list in case the values have been modified, then add them
        personalityVector.Clear();
        personalityVector.Add(OpennessToExperience);
        personalityVector.Add(Concientiousness);
        personalityVector.Add(Extroversion);
        personalityVector.Add(Agreeableness);
        personalityVector.Add(Neuroticism);

        string seedString1 = "";
        string seedString2 = "";

        foreach (float pvalue in personalityVector)
        {

            float npvalue = NormaliseFloat(pvalue, 1.0f, -1.0f);

            //Remove everything starting with index 0 and ending at the index of ([the dot .] + 1)
            string personVal = npvalue.ToString().Remove(0, npvalue.ToString().IndexOf(".") + 1);

            // values from personality contribute to the PRNG seed
            seedString1 += personVal[0];
            seedString2 += personVal[1];
        }

        //Debug.Log("First seed: " + seedString1);
        //Debug.Log("Second seed: " + seedString2);

        // convert back to a number
        seed1 = Convert.ToInt32(seedString1);
        seed2 = Convert.ToInt32(seedString2);

        // scale them to size
        seed1 = TestSeed(seed1);
        seed2 = TestSeed(seed2);

        //Debug.Log("First seed (tested): " + seed1);
        //Debug.Log("Second seed (tested): " + seed2);
    }

    // basic means to handle cases where seeds are > 30,000
    int TestSeed(int seed2test)
    {
        //ensures seed is less than 30000 (required by PRNG)
        while(seed2test > 30000)
        {
            seed2test /= 2;
        }
        
        return seed2test;
    }

    #endregion


    void GenerateStateParameterDictionary()
    {
        stateParameterDictionary.Add("Hunger", hunger);
        stateParameterDictionary.Add("Energy", energy);
        stateParameterDictionary.Add("Wealth", wealth);
        stateParameterDictionary.Add("Mood", mood);
        stateParameterDictionary.Add("Temper", temper);
        stateParameterDictionary.Add("Sociability", sociability);
        stateParameterDictionary.Add("Soberness", soberness);
        stateParameterDictionary.Add("Resources", resources);
        stateParameterDictionary.Add("TimeOfDay", timeofday);
        stateParameterDictionary.Add("Time", timeofday);

        // temporary, until its set
        stateParameterDictionary.Add("Relationship", sociability);
    }


}
