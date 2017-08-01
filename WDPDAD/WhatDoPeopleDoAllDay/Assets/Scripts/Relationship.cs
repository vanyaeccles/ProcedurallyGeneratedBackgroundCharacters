using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Relationship 
{

    public string nameOfPerson;

    public AgentStateVarFloat relationshipValue;


    // constructor
    public Relationship(string _name, AgentStateVarFloat _rellyVal)
    {
        nameOfPerson = _name;
        relationshipValue = _rellyVal;
    }

    public Relationship()
    {

    }

}
