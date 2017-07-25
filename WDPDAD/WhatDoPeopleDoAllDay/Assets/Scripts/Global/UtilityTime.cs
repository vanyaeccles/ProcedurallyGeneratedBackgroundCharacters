using UnityEngine;
using System.Collections;

public class UtilityTime : MonoBehaviour
{

    public static float speed = 1.0f;
    public static bool paused = false;

    // can set speed to gloablly scale time calculations

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
