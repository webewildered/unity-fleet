using UnityEngine;
using System;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    struct Collision : IComparable<Collision>
    {
        public GameObject GameObject;
        public int Priority;

        public int CompareTo(Collision other)
        {
            return Priority - other.Priority;
        }
    };

    public float damage;
    public Vector3 velocity;
    List<Collision> collisions;
    Collider coll;

    void Start ()
    {
        collisions = new List<Collision>();
        coll = gameObject.GetComponent<Collider>();
    }
	
	void Update ()
    {
        // Sort and apply collisions
        collisions.Sort();
        foreach (Collision collision in collisions)
        {
            Ship ship = collision.GameObject.GetComponent<Ship>();
            if (ship != null && ship.Hit(damage))
            {
                GameObject.Destroy(gameObject);
                return;
            }
        }
        collisions.Clear();

        // Integrate
        transform.position += velocity * Time.deltaTime;

        // Check out of bounds
        if (!coll.bounds.Intersects(World.Bounds))
        {
            GameObject.Destroy(gameObject);
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Collision collision = new Collision();
        collision.GameObject = other.attachedRigidbody.gameObject;
        collision.Priority = 0; // TODO
        collisions.Add(collision);
    }
}
