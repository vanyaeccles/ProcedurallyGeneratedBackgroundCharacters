using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This represents all the information needed to specify an agent
// essentially a 'collapsed' agent 


public class AgentKernel : MonoBehaviour {

    // the agents name
    public string Name;

    // the location of their house
    public Transform homeLocation;

    // an enum value representing the agents profession 
    public OccupationType occupation;


    [Header("Personality Values [-1.0,1.0] (please enter at least 2 decimal places)")]
    public float OpennessToExperience;
    public float Concientiousness;
    public float Extroversion;
    public float Agreeableness;
    public float Neuroticism;


    
}
