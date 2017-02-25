using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainPath
{
    public struct Turn
    {
        public float distance;
        public float radius;
        public float direction;
    }

    public Vector3 startPosition;
    public Vector3 startDirection;
    public List<Turn> turns;

    static float calcClearance(Vector3 position, Vector3 direction, Vector3 min, Vector3 max)
    {
        return Mathf.Max(Vector3.Dot(min - position, direction), Vector3.Dot(max - position, direction));
    }

    public static ChainPath Random(Rng rng, Vector3 min, Vector3 max)
    {
        const float margin = 3.0f;
        ChainPath path = new ChainPath();
        Vector3 half = (max + min) / 2.0f;

        // Enter from left, right or top
        int entry = rng.Range(0, 3);
        float nextDirection = 0;
        if (entry < 2)
        {
            if (entry == 0)
            {
                // left
                path.startDirection.x = 1.0f;
                path.startPosition.x = min.x - margin;
                nextDirection = -1.0f;
            }
            else
            {
                // right
                path.startDirection.x = -1.0f;
                path.startPosition.x = max.x + margin;
                nextDirection = 1.0f;
            }
            path.startPosition.z = rng.Range(half.z - margin, max.z + margin);
        }
        else
        {
            // top
            path.startDirection.z = -1.0f;
            path.startPosition.z = max.z + margin;
            path.startPosition.x = rng.Range(min.x + margin, max.x - margin);
            nextDirection = rng.PlusOrMinusOne();
        }

        // Make a few turns, then exit
        Vector3 position = path.startPosition;
        Vector3 direction = path.startDirection;
        int numTurns = rng.Range(2, 6);
        for (int i = 0; i < numTurns; i++)
        {
            // Choose radius
            float radius = 8.0f; // TODO: randomize this?

            // Choose distance
            float clearance = calcClearance(position, direction, min, max) - radius - margin;
            const float minDistance = 10.0f;
            if (clearance < minDistance)
            {
                break;
            }
            float distance = rng.Range(minDistance, clearance);

            // Add the turn
            Turn turn = new Turn();
            turn.direction = nextDirection;
            turn.radius = radius;
            turn.distance = distance;
            path.turns.Add(turn);

            // Move and turn
            position += direction * (distance + radius);
            direction = nextDirection * new Vector3(-direction.z, 0.0f, direction.x);
            float nextClearance = calcClearance(position, direction, min, max) - radius - margin;
            const float minClearance = 20.0f;
            if (nextClearance < minClearance)
            {
                direction = -direction;
            }
            position += direction * radius;

            // Choose the next turn direction randomly
            nextDirection = rng.PlusOrMinusOne();
        }

        return path;
    }

    public ChainPath()
    {
        turns = new List<Turn>();
    }
}
