using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from tutorial at https://www.youtube.com/watch?v=F5eE1YL1ZJY

public class PlayerController : MonoBehaviour {

    public float speed = 400f;
    public float sensitivity = 2f;
    CharacterController player;

    public GameObject eyes;


    float moveFB;
    float moveLR;


    float rotX;
    float rotY;


	void Start ()
    {
        player = GetComponent<CharacterController>();
	}
	
	
	void Update ()
    {

        moveFB = Input.GetAxis("Vertical") * speed;
        moveLR = Input.GetAxis("Horizontal") * speed;

        rotX = Input.GetAxis("Mouse X") * sensitivity;
        rotY = Input.GetAxis("Mouse Y") * sensitivity;

        Vector3 movement = new Vector3(moveLR, 0, moveFB);
        transform.Rotate(0, rotX, 0);
        eyes.transform.Rotate(-rotY, 0, 0);

        movement = transform.rotation * movement;
        player.SimpleMove(movement * Time.deltaTime);
	}


}
