using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainShip : EnemyShip
{
    public ChainPath path;

    Vector3 direction;
    Vector3 nextDirection;
    float distance;
    int nextTurn;
    bool turning;
    Vector3 pivot;

    public void SetPath(ChainPath path, int index)
    {
        const float size = 3.0f;

        this.path = path;
        direction = path.startDirection;
        float offset = index * size;
        transform.position = path.startPosition - direction * offset;
        distance = path.turns[0].distance + offset;
        setPivot(path.startPosition, path.turns[0].distance);
        nextTurn = 0;
    }

    void setPivot(Vector3 startPosition, float distance)
    {
        nextDirection = path.turns[nextTurn].direction * new Vector3(-direction.z, 0.0f, direction.x);
        pivot = startPosition + direction * distance + nextDirection * path.turns[nextTurn].radius;
    }

    protected void Update()
    {
        // Follow the path
        const float speed = 30.0f;
        float d = Time.deltaTime * speed;

        while (true)
        {
            if (turning)
            {
                float radius = path.turns[nextTurn].radius;

                if (distance < d)
                {
                    // End turn
                    d -= distance;
                    transform.position = pivot + direction * radius;
                    direction = nextDirection;
                    turning = false;

                    nextTurn++;
                    if (nextTurn < path.turns.Count)
                    {
                        distance = path.turns[nextTurn].distance;
                        setPivot(transform.position, distance);
                    }
                    else
                    {
                        distance = float.MaxValue;
                    }
                }
                else
                {
                    // Continue turn
                    distance -= d;
                    float angle = Mathf.PI / 2.0f - distance / radius;
                    transform.position = pivot + radius * (Mathf.Sin(angle) * direction - Mathf.Cos(angle) * nextDirection);
                    break;
                }
            }
            else
            {
                if (distance < d)
                {
                    // Begin turning
                    d -= distance;
                    float radius = path.turns[nextTurn].radius;
                    distance = radius * Mathf.PI / 2.0f; // quarter turn
                    turning = true;
                }
                else
                {
                    // Continue straight
                    distance -= d;
                    transform.position += d * direction;
                    break;
                }
            }
        }
    }
}
