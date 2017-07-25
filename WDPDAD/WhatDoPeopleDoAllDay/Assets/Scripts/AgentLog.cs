using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used for logging agent behaviour

public class AgentLog {

    //public ActionBehaviour action;

    public string action;

    public float duration = 0.0f;

    public AgentLog(string _action)
    {
        action = _action;
    }
}
