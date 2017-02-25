using UnityEngine;
using System.Collections;

public static class World
{
    public static float Top = 45;
    public static float Bottom = -45;
    public static float Left = -60;
    public static float Right = 60;
    public static Bounds Bounds;
    public static Vector3 Min, Max;

    static World()
    {
        Min = new Vector3(Left, 0, Bottom);
        Max = new Vector3(Right, 0, Top);
        Bounds = new Bounds(0.5f * new Vector3(Right + Left, 0, Top + Bottom), new Vector3(Right - Left, 100.0f, Top - Bottom));
    }
}
