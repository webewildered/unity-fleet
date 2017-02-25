using UnityEngine;
using System.Collections;
using System;

public class DroneShip : SquadShip
{
    public GameObject bulletPrefab;
    float shotTimer;    // Control 

    const float shotFrequency = 2.0f;

    override protected void Start()
    {
        shotTimer = 0.0f;
    }

    override protected void Update()
    {
        base.Update();
        
        // Once at the target, fire periodically
        if (SquadReady())
        {
            shotTimer -= Time.deltaTime;
            if (shotTimer <= 0)
            {
                GameObject bulletObj = GameObject.Instantiate(bulletPrefab);
                bulletObj.transform.position = transform.position;

                Bullet bullet = bulletObj.GetComponent<Bullet>();
                bullet.velocity = transform.forward * 20.0f;

                shotTimer = shotFrequency;
            }
        }
    }
}
