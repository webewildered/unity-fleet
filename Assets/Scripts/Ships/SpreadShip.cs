using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShip : SquadShip
{
    public SpreadShip()
    {
        health = 10.0f;
    }

    // Shooting
    public GameObject bulletPrefab;
    
    VolleyTimer timer;

    static Vector3 shotDirection;

    static SpreadShip()
    {
        const float shotAngle = 0.2f - Mathf.PI / 2.0f;
        shotDirection = new Vector3(Mathf.Cos(shotAngle), 0.0f, Mathf.Sin(shotAngle));
    }

    protected override void Start()
    {
        base.Start();

        const float volleyFrequency = 3.0f;
        const float shotFrequency = 0.1f;
        const int numShots = 10;
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
                for (int i = 0; i < 2; i++)
                {
                    float sign = (i * 2.0f) - 1.0f;

                    GameObject bulletObj = GameObject.Instantiate(bulletPrefab);
                    bulletObj.transform.position = transform.position + new Vector3(sign * 2.0f, 0.0f, 0.0f);

                    Bullet bullet = bulletObj.GetComponent<Bullet>();
                    bullet.velocity = new Vector3(shotDirection.x * sign, shotDirection.y, shotDirection.z) * 20.0f;
                }
            }
        }
    }
}
