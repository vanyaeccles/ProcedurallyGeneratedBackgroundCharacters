using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    /*
     * This is hardcoded stuff for the agents to build weights and action modifiers from
     */


public class GlobalAgentProperties : MonoBehaviour
{


    // contains the influences that personality parameters have on state variable decisions
    // ie how does extroversion level relate to importance of sociability, @TODO could replace this with a string tuple-value dictionary
    public float[,] personalityWeightInfluences = new float[5, 10];


    // How the personality variables influence state variable modifiers (ie when performing an action)
    public float[,] personalityModifierInfluences = new float[5, 10];


    // the dictionary thats used to execute effect of behaviours
    public Dictionary<string, List<float>> actionModifierDictionary = new Dictionary<string, List<float>>();


    public Dictionary<string, int> StateVarDictionary = new Dictionary<string, int>();


    // Use this for initialization
    void Awake()
    {
        // build the global arrays + dictionaries for agent personalities to access

        BuildPersonalityWeightInfluences();
        BuildPersonalityModifierInfluences();
        BuildStateVarDictionary();
    }


    // builds the weight influences
    void BuildPersonalityWeightInfluences()
    {
        // These signify how much personality values influence the weights that are generated for each state variable

        // rows ascending
        // timeofday relationship 	wealth	hunger	energy	mood	temper	sociability	soberness resources

        // columns ascending
        // Neuroticism Agreeableness Extroversion Conscientiousness Openness

        // timeofday
        personalityWeightInfluences[0, 0] = -2.0f;
        personalityWeightInfluences[1, 0] = 3.0f;
        personalityWeightInfluences[2, 0] = 0.0f;
        personalityWeightInfluences[3, 0] = 1.0f;
        personalityWeightInfluences[4, 0] = 2.0f;
        //relationship
        personalityWeightInfluences[0, 1] = -2.0f;
        personalityWeightInfluences[1, 1] = 2.0f;
        personalityWeightInfluences[2, 1] = -3.0f;
        personalityWeightInfluences[3, 1] = -1.0f;
        personalityWeightInfluences[4, 1] = 0.0f;
        //wealth
        personalityWeightInfluences[0, 2] = 0.0f;
        personalityWeightInfluences[1, 2] = 3.0f;
        personalityWeightInfluences[2, 2] = 0.0f;
        personalityWeightInfluences[3, 2] = 1.0f;
        personalityWeightInfluences[4, 2] = -1.0f;
        //hunger
        personalityWeightInfluences[0, 3] = 0.0f;
        personalityWeightInfluences[1, 3] = -1.0f;
        personalityWeightInfluences[2, 3] = 0.0f;
        personalityWeightInfluences[3, 3] = -1.0f;
        personalityWeightInfluences[4, 3] = 1.0f;
        //energy
        personalityWeightInfluences[0, 4] = -1.0f;
        personalityWeightInfluences[1, 4] = -2.0f;
        personalityWeightInfluences[2, 4] = +2.0f;
        personalityWeightInfluences[3, 4] = 0.0f;
        personalityWeightInfluences[4, 4] = 1.0f;
        //mood
        personalityWeightInfluences[0, 5] = 0.0f;
        personalityWeightInfluences[1, 5] = -2.0f;
        personalityWeightInfluences[2, 5] = 2.0f;
        personalityWeightInfluences[3, 5] = 0.0f;
        personalityWeightInfluences[4, 5] = 2.0f;
        //temper
        personalityWeightInfluences[0, 6] = 0.0f;
        personalityWeightInfluences[1, 6] = -1.0f;
        personalityWeightInfluences[2, 6] = 1.0f;
        personalityWeightInfluences[3, 6] = -1.0f;
        personalityWeightInfluences[4, 6] = +2.0f;
        //sociability
        personalityWeightInfluences[0, 7] = -1.0f;
        personalityWeightInfluences[1, 7] = 0.0f;
        personalityWeightInfluences[2, 7] = 2.0f;
        personalityWeightInfluences[3, 7] = -1.0f;
        personalityWeightInfluences[4, 7] = 2.0f;
        //soberness
        personalityWeightInfluences[0, 8] = 0.0f;
        personalityWeightInfluences[1, 8] = -2.0f;
        personalityWeightInfluences[2, 8] = 1.0f;
        personalityWeightInfluences[3, 8] = -1.0f;
        personalityWeightInfluences[4, 8] = 1.0f;
        //resources
        personalityWeightInfluences[0, 9] = 0.0f;
        personalityWeightInfluences[1, 9] = -2.0f;
        personalityWeightInfluences[2, 9] = 1.0f;
        personalityWeightInfluences[3, 9] = -1.0f;
        personalityWeightInfluences[4, 9] = 1.0f;


        // limit the values to within -0.1 to +0.1
        for (int i = 0; i < personalityWeightInfluences.GetLength(0); i++)
            for (int j = 0; j < personalityWeightInfluences.GetLength(1); j++)
                personalityWeightInfluences[i, j] *= 0.2f;
    }


    // builds the influences that personality has on state variable change
    void BuildPersonalityModifierInfluences()
    {
        // rows ascending
        // timeofday relationship 	wealth	hunger	energy	mood	temper	sociability	soberness resources
        // columns ascending
        // Neuroticism Agreeableness Extroversion Conscientiousness Openness

        //relationship
        personalityModifierInfluences[0, 0] = +3.0f;
        personalityModifierInfluences[1, 0] = 0.0f;
        personalityModifierInfluences[2, 0] = 0.0f;
        personalityModifierInfluences[3, 0] = 0.0f;
        personalityModifierInfluences[4, 0] = 0.0f;
        //hunger
        personalityModifierInfluences[0, 1] = 0.0f;
        personalityModifierInfluences[1, 1] = 0.0f;
        personalityModifierInfluences[2, 1] = 0.0f;
        personalityModifierInfluences[3, 1] = 0.0f;
        personalityModifierInfluences[4, 1] = +3.0f;
        //energy
        personalityModifierInfluences[0, 2] = 0.0f;
        personalityModifierInfluences[1, 2] = 0.0f;
        personalityModifierInfluences[2, 2] = +3.0f;
        personalityModifierInfluences[3, 2] = 0.0f;
        personalityModifierInfluences[4, 2] = 0.0f;
        //wealth
        personalityModifierInfluences[0, 3] = 0.0f;
        personalityModifierInfluences[1, 3] = +3.0f;
        personalityModifierInfluences[2, 3] = 0.0f;
        personalityModifierInfluences[3, 3] = 0.0f;
        personalityModifierInfluences[4, 3] = 0.0f;
        //mood
        personalityModifierInfluences[0, 4] = 0.0f;
        personalityModifierInfluences[1, 4] = 0.0f;
        personalityModifierInfluences[2, 4] = 0.0f;
        personalityModifierInfluences[3, 4] = 0.0f;
        personalityModifierInfluences[4, 4] = +3.0f;
        //temper
        personalityModifierInfluences[0, 5] = 0.0f;
        personalityModifierInfluences[1, 5] = 0.0f;
        personalityModifierInfluences[2, 5] = 0.0f;
        personalityModifierInfluences[3, 5] = +3.0f;
        personalityModifierInfluences[4, 5] = 0.0f;
        //sociability
        personalityModifierInfluences[0, 6] = 0.0f;
        personalityModifierInfluences[1, 6] = 0.0f;
        personalityModifierInfluences[2, 6] = +3.0f;
        personalityModifierInfluences[3, 6] = 0.0f;
        personalityModifierInfluences[4, 6] = 0.0f;
        //soberness
        personalityModifierInfluences[0, 7] = 0.0f;
        personalityModifierInfluences[1, 7] = 0.0f;
        personalityModifierInfluences[2, 7] = 0.0f;
        personalityModifierInfluences[3, 7] = +3.0f;
        personalityModifierInfluences[4, 7] = 0.0f;
        //resources
        personalityModifierInfluences[0, 8] = 0.0f;
        personalityModifierInfluences[1, 8] = +3.0f;
        personalityModifierInfluences[2, 8] = 0.0f;
        personalityModifierInfluences[3, 8] = 0.0f;
        personalityModifierInfluences[4, 8] = 0.0f;



        // limit the values to within -1.0 to +1.0
        //for (int i = 0; i < personalityModifierInfluences.GetLength(0); i++)
        //    for (int j = 0; j < personalityModifierInfluences.GetLength(1); j++)
        //        personalityModifierInfluences[i, j] *= 0.2f;
    }


    // builds the dictionary that maps from string values (for each agent state variable) to integers
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
}

