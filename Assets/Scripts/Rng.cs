﻿using UnityEngine;
using System.Collections;

public class Rng
{
    // Internal implementation
    System.Random random;

    // Global rng for values that don't need to be deterministic
    public static Rng Global;

    static Rng()
    {
        Global = new Rng((int)System.DateTime.Now.Ticks);
    }

    public Rng(int seed)
    {
        random = new System.Random(seed);
    }

    public float Range(float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    public int Range(int min, int max)
    {
        return random.Next(min, max);
    }

    public bool Flip()
    {
        return (random.Next(0, 2) != 0);
    }

    public float PlusOrMinusOne()
    {
        return (Range(0, 2) * 2 - 1);
    }
}
