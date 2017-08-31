using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used to billboard the agent info speech bubble towards the active camera

public class AgentSpeechBubble : MonoBehaviour {

    public UIAgentManager agentManager;

    public Camera m_Camera;


    void Update()
    {
        m_Camera = agentManager.activeCam;

            transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
                m_Camera.transform.rotation * Vector3.up);
        }

}
