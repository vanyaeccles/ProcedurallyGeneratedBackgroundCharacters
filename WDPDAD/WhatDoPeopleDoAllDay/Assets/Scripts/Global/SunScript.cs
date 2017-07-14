using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour {

    public AgentStateVarFloat time;

    public float angle = 0.0f;

    float hoursPerFrame = 0.0f;

    float anglesPerHour = 15.0f;

    // Update is called once per frame
    void Update ()
    {

        //float timeValue = time.value / 24.0f;

        //angle = 5.0f * UtilityTime.time;

        hoursPerFrame = (0.3f * UtilityTime.time);

        angle = hoursPerFrame * anglesPerHour;

        // @TODO fix this
        transform.RotateAround(Vector3.zero, Vector3.forward,  angle);


    }
}
