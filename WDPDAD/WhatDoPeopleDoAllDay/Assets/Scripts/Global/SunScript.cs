using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour {

    public AgentStateVarFloat time;

    public float angle = 0.0f;

	// Update is called once per frame
	void Update ()
    {

        //float timeValue = time.value / 24.0f;

        angle = 5.0f * UtilityTime.time;

        // @TODO fix this
        transform.RotateAround(Vector3.zero, Vector3.forward,  angle);


    }
}
