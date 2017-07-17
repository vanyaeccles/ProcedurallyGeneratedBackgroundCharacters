using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInfoBoard : MonoBehaviour {

    public Camera m_Camera;

    public void Start()
    {
        m_Camera = GameObject.Find("Camera").GetComponent<Camera>();
    }


    void Update()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }
}