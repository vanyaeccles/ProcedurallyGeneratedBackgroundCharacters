using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffScript : MonoBehaviour {

    public GameObject gameob;

    bool toggle = true;
	
    public void onClick()
    {
        toggle = !toggle;

        gameob.SetActive(toggle);
    }

}
