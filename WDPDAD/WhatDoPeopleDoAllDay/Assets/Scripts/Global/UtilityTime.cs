using UnityEngine;
using System.Collections;

// This class is referenced gloabally when scaling things by deltatime (time taken for last frame in seconds)

public class UtilityTime : MonoBehaviour
{


    // can set speed to globally scale time calculations 
    public static float speed = 0.05f;
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
