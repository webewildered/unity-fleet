using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : EnemyShip
{
    Vector3 velocity;
    float speed;
    float angle;
    float angularSpeed;
    float timer;

    public Missile()
    {
        health = 2.0f; // TODO public property?
    }

    public void Launch(Vector3 velocity)
    {
        this.velocity = velocity;
        speed = velocity.magnitude;
        angle = Mathf.Atan2(velocity.z, velocity.x);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 10.0f)
        {
            GameObject.Destroy(gameObject);
            return;
        }

        Fleet fleet = Fleet.Instance;
        Vector3 targetPosition = fleet.transform.position + fleet.Center;
        Vector3 difference = targetPosition - transform.position;
        float targetDistance = difference.magnitude;
        Vector3 targetDirection = difference / targetDistance;
        float targetAngle = Mathf.Atan2(difference.z, difference.x);

        Vector3 ccw = Util.CCW(velocity);
        float dot = Vector3.Dot(targetDirection, ccw);
        if (dot < 0.0f && targetAngle > angle)
        {
            targetAngle -= 2 * Mathf.PI;
        }
        else if (dot > 0.0f && targetAngle < angle)
        {
            targetAngle += 2 * Mathf.PI;
        }

        float time = 0.2f;// 0.5f * targetDistance / speed;
        float acceleration = (2.0f * (targetAngle - angle) - angularSpeed * time) / (time * time);
        const float maxAcceleration = 10.0f;
        if (Mathf.Abs(acceleration) > maxAcceleration)
        {
            acceleration = Mathf.Sign(acceleration) * maxAcceleration;
        }

        angularSpeed += acceleration * Time.deltaTime;
        angle += angularSpeed * Time.deltaTime;

        if (angle < -Mathf.PI)
        {
            angle += 2 * Mathf.PI;
        }
        else if (angle > Mathf.PI)
        {
            angle -= 2 * Mathf.PI;
        }

        Vector3 direction = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
        velocity = direction * speed;

        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(-angle * Mathf.Rad2Deg, Util.Up);
    }
}
