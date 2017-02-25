using UnityEngine;
using System.Collections;
using System;

public class FleetShip : Ship
{
    public GameObject bulletPrefab;

    public Fleet fleet;
    public int slot;        // Slot in the fleet formation, set by fleet
    public Vector3 target;  // Target position in fleet local space, set by fleet
    public bool fire;       // Whether the ship should fire, set by fleet

    // Last frame's speed of movement towards the target in fleet space, used to apply max accel
    float speed;

    // Position in fleet space
    Vector3 position;

    // Previous frame position in world space
    Vector3 lastPosition;

    // transform.position = position + offset.  Offset is used to add a little noisy motion
    Vector3 offset;
    Vector3 offsetTarget;
    float offsetTime;

    // When true, the ship will be removed at the next LateUpdate()
    bool remove;

    float shotTimer;

    void Start()
    {
        position = transform.localPosition;
    }

    void Update()
    {
        // Move towards the target position
        const float targetGain = 10.0f;
        const float maxSpeed = 50.0f;
        const float maxAccel = 50.0f;
        Util.Move(ref position, ref speed, target, targetGain, maxSpeed, maxAccel);

        // Update offset
        offsetTime -= Time.deltaTime;
        if (offsetTime <= 0.0f)
        {
            // Choose a new offset target
            float offsetAngle = Rng.Global.Range(0.0f, 2.0f * Mathf.PI);
            float offsetDistance = Rng.Global.Range(0.0f, 0.2f);
            offsetTime = Rng.Global.Range(0.5f, 2.0f);
            offsetTarget.x = Mathf.Cos(offsetAngle) * offsetDistance;
            offsetTarget.z = Mathf.Sin(offsetAngle) * offsetDistance;
        }

        const float offsetGain = 2.0f;
        offset += (offsetTarget - offset) * Util.Gain(offsetGain);

        // Update transform
        transform.localPosition = position + offset;

        // Roll
        float speedX = (transform.position.x - lastPosition.x) / Time.deltaTime;
        Quaternion targetRoll = Quaternion.AngleAxis(-speedX * 2.0f, Util.Forward);
        const float rollGain = 20.0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRoll, Util.Gain(rollGain));

        lastPosition = transform.position;

        // Fire weapon
        if (fire)
        {
            shotTimer -= Time.deltaTime;
            if (shotTimer <= 0.0f)
            {
                GameObject bulletObj = GameObject.Instantiate(bulletPrefab);
                bulletObj.transform.position = transform.position;
                bulletObj.layer = Global.PlayerBulletLayer;

                Bullet bullet = bulletObj.GetComponent<Bullet>();
                bullet.velocity = transform.forward * 100.0f;
                bullet.damage = 1.0f;

                const float shotFrequency = 0.2f;
                shotTimer = shotFrequency;
            }
        }
    }

    void LateUpdate()
    {
        if (remove)
        {
            fleet.RemoveShip(this);
        }
    }

    public override bool Hit(float damage)
    {
        // TODO - maybe there will be situations where the ship is invulnerable and doesn't collide with bullets
        remove = true;
        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Fleet ship handles all ship vs ship collisions
        Ship ship = other.attachedRigidbody.gameObject.GetComponent<Ship>();
        if (ship != null)
        {
            const float damage = 1.0f;
            ship.Hit(damage);
            remove = true;
        }
    }
}
