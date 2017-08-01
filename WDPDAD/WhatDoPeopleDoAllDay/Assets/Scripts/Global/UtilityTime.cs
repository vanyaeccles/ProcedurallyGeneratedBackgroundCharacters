using UnityEngine;
using System.Collections;

public class UtilityTime : MonoBehaviour
{


    // can set speed to globally scale time calculations
    public static float speed = 0.5f;
    public static bool paused = false;

    

    static public float time
    {
        get
        {
            if (paused)
                return 0.0f;
            else
                return Time.deltaTime * speed;
        }
    }

}
