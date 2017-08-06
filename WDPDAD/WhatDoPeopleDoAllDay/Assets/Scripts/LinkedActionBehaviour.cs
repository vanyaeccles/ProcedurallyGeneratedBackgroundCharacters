using UnityEngine;
using System;

// This specifies an action that belongs to a specific NPC

[Serializable]
public class LinkedActionBehaviour
{
    public ActionBehaviour action;
    public bool isActionEnabled = true;
    //public float cooldown = 0.0f;
    //public float cooldownTimer = 0.0f;


    //public Transform GetTransform()
    //{
    //    return gameObject.transform;
    //}
}
