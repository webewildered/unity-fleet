using UnityEngine;
using System.Collections;

public class Util
{
    public static float Gain(float g)
    {
        return Mathf.Min(g * Time.deltaTime, 1.0f);
    }

    public static Vector3 Interpolate(Vector3 x0, Vector3 x1, float t)
    {
        return x0 * (1.0f - t) + x1 * t;
    }

    public static Vector3 CW(Vector3 v)
    {
        return new Vector3(v.z, 0.0f, v.x);
    }

    public static Vector3 CCW(Vector3 v)
    {
        return new Vector3(-v.z, 0.0f, v.x);
    }

    public static Vector3 Up
    {
        get { return new Vector3(0, 1, 0); }
    }

    public static Vector3 Forward
    {
        get { return new Vector3(0, 0, 1); }
    }

    // Common movement algorithm.  Updates position and speed.  Acceleration limit is non-directional, so a ship could reverse direction instantly.
    public static void Move(ref Vector3 position, ref float speed, Vector3 target, float gain, float maxSpeed, float maxAccel)
    {
        // Move towards the target position.  Desired change in position is controlled by gain,
        // then limited by maximum speed and acceleration constraints.
        Vector3 difference = target - position;
        difference *= Util.Gain(gain);
        if (difference.magnitude > maxSpeed * Time.deltaTime)
        {
            difference *= maxSpeed * Time.deltaTime / difference.magnitude;
        }
        float maxSpeedIncrease = maxAccel * Time.deltaTime;
        if (difference.magnitude - speed * Time.deltaTime > maxSpeedIncrease * Time.deltaTime)
        {
            difference *= (speed + maxSpeedIncrease) * Time.deltaTime / difference.magnitude;
        }
        speed = difference.magnitude / Time.deltaTime;
        position += difference;
    }

    public const float Sqrt2 = 1.4142135623730950488016887242097f;
    public const float Sqrt3 = 1.7320508075688772935274463415059f;
    public const float Dt = 1.0f / 60.0f; // Fixed step dt
}
