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
    public AgentStateVarFloat energy, wealth, mood, temper, sociability, soberness;

    [Header("Relationships")]
    public AgentStateParameter agent1;

    [Header("Global")]
    public AgentStateParameter timeofday;



    // contains the influences that personlaity parameters have on state variables
    // ie how does extroversion level relate to importance of sociability, @TODO could replace this with a string tuple-value dictionary
    private float[,] personalityWeightInfluences = new float[5,9];
    private Dictionary<string, int> StateVarDictionary = new Dictionary<string, int>();


    void Awake()
    {
        // the same weight influences for all agents
        BuildPersonalityWeightInfluences();
        BuildStateVarDictionary();

        Debug.Log(CheckWeight("timeofday"));
        Debug.Log(CheckWeight("hunger"));
        Debug.Log(CheckWeight("energy"));
        Debug.Log(CheckWeight("wealth"));
        Debug.Log(CheckWeight("mood"));
        Debug.Log(CheckWeight("temper"));
        Debug.Log(CheckWeight("sociability"));
        Debug.Log(CheckWeight("soberness"));
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




    //@todo could be moved to a global place?
    #region Global data storage

    void BuildPersonalityWeightInfluences()
    {
        // rows ascending
        // timeofday relationship 	wealth	hunger	energy	mood	temper	sociability	soberness
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



        // limit the values to within -0.1 to +0.1
        for (int i = 0; i < personalityWeightInfluences.GetLength(0); i++)
            for (int j = 0; j < personalityWeightInfluences.GetLength(1); j++)
                personalityWeightInfluences[i, j] *= 0.02f;
    }


    private void BuildStateVarDictionary()
    {
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

    }

    #endregion

}
