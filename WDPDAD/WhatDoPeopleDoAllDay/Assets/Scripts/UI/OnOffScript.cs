using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This just handles the minimize buttons at the top of UI panels

public class OnOffScript : MonoBehaviour {

    public GameObject gameob;

    bool toggle = true;
	
    public void onClick()
    {
        toggle = !toggle;

        gameob.SetActive(toggle);
    }

}
