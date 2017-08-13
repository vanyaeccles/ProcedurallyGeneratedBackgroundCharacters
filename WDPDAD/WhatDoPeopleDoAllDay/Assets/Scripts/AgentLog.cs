using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used for logging agent behaviour

public class AgentLog {

    //public ActionBehaviour action;

    public string action;

    public float score;


    public float startTime = 0.0f;
    public float endTime = 0.0f;

    public AgentLog(string _action, float _startTime, float _score)
    {
        action = _action;
        startTime = _startTime;
        score = _score;
    }

    public float EndTime
    {
        get
        {
            return endTime;
        }
        set
        {
            endTime = value;
        }
        
    }
}
