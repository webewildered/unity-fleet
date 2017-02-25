using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherShip : SquadShip
{
    public LauncherShip()
    {
        health = 10.0f;
    }

    // Shooting
    public GameObject missilePrefab;
    VolleyTimer timer;

    protected override void Start()
    {
        base.Start();

        const float volleyFrequency = 3.0f;
        const float shotFrequency = 12.0f;// 0.33f;
        const int numShots = 100;
        timer = new VolleyTimer(volleyFrequency, shotFrequency, numShots);
    }

    override protected void Update()
    {
        // Move
        base.Update();

        // Once at the target, fire periodically
        if (SquadReady())
        {
            if (timer.Update(Time.deltaTime))
            {
                // Fire a shot
                GameObject bulletObj = GameObject.Instantiate(missilePrefab);
                bulletObj.transform.position = transform.position;

                Missile missile = bulletObj.GetComponent<Missile>();
                missile.Launch(transform.forward * 15.0f);
            }
        }
    }
}
