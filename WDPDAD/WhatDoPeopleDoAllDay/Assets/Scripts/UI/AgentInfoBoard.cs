using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is the script for controlling the info panel above agents 

public class AgentInfoBoard : MonoBehaviour {

    // the camera it is following
    private Camera m_Camera;

    // the agent manager it reports to
    public UIAgentManager agentManager;



    void Update()
    {
        m_Camera = agentManager.activeCam;


        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }



}