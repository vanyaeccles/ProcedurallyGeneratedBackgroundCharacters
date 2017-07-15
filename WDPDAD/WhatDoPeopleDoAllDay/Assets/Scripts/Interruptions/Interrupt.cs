using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interrupt : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

	}


    void OnTriggerEnter(Collider c)
    {

        if (c.gameObject.tag == "Agent")
        {
            //GameObject.Find(c.gameObject.name).SendMessage("Interrupt");

            c.gameObject.GetComponent<Agent>().Interrupt(name, true, false);

            //Debug.Log("interuption sent");
        }
    }
}
