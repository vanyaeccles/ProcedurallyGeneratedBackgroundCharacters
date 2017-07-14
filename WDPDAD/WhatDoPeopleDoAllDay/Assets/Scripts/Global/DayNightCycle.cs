using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {

    public AgentStateVarFloat time;

    float timeOfDay = 12.0f;
     
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //update time
        timeOfDay += 0.3f * UtilityTime.time;

        if (timeOfDay >= 24.0f)
            timeOfDay = 0.0f;



        time.value = timeOfDay;
    }
}
