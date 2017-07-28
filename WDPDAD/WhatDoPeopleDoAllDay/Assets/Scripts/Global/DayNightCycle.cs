using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {

    public AgentStateVarFloat time;

    float timeOfDay = 12.0f;

    public float angle = 0.0f;

    [HideInInspector]
    public float hoursPerFrame;

    float anglesPerHour = 15.0f;

	
	// Update is called once per frame
	void Update ()
    {

        // Update the sun's rotation (geocentric)
        hoursPerFrame = (UtilityTime.time);

        angle = hoursPerFrame * anglesPerHour;
        transform.RotateAround(Vector3.zero, Vector3.forward, angle);




        //update time with the per frame rate
        timeOfDay += hoursPerFrame;
        // a new day dawns
        if (timeOfDay >= 24.0f)
            timeOfDay = 0.0f;

        time.value = timeOfDay;
    }
}
