using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolleyTimer
{
    float volleyFrequency;
    float shotFrequency;
    int numShots;

    float timer;
    int shotsLeft;

    public VolleyTimer(float volleyFrequency, float shotFrequency, int numShots)
    {
        this.volleyFrequency = volleyFrequency;
        this.shotFrequency = shotFrequency;
        this.numShots = numShots;

        timer = volleyFrequency;
        shotsLeft = numShots;
    }

    // Update is called once per frame
    public bool Update(float dt)
    {
        timer -= dt;
        if (timer <= 0)
        {
            // Reset the timer
            shotsLeft--;
            if (shotsLeft == 0)
            {
                timer = volleyFrequency;
                shotsLeft = numShots;
            }
            else
            {
                timer = shotFrequency;
            }

            return true;
        }

        return false;
    }
}
