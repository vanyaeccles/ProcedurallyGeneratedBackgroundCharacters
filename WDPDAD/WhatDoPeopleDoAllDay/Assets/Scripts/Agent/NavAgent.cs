using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// Handles the agent's navigation
public class NavAgent : MonoBehaviour {

    public Transform target;
    NavMeshAgent agent;


	// Use this for initialization
	void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
	}
	

	// Update is called once per frame
	void Update ()
    {
        agent.SetDestination(target.position);
	}
}
