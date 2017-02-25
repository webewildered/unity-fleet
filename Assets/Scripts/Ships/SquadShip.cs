using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ship that flies to a position after spawning, then fires when it gets there.
// If joined to a squadron, all ships begin firing simultaneously when all of them arrive.
public class SquadShip : EnemyShip
{
    private Squad squad;    // for synchronizing with other ships
    private bool ready;

    public Vector3 target;
    private float speed;        // Used for acceleration limit

    // constants, child class can override
    protected float targetGain = 5.0f;
    protected float maxSpeed = 20.0f;
    protected float maxAccel = 20.0f;

    // Use this for initialization
    virtual protected void Start ()
    {
        if (squad == null)
        {
            squad = new Squad();
            Join(squad);
        }
    }

    public void Join(Squad squad)
    {
        this.squad = squad;
        squad.active++;
        squad.waiting++;
    }

    virtual protected void Update()
    {
        // Move towards the target position
        Vector3 position = transform.position;
        Util.Move(ref position, ref speed, target, targetGain, maxSpeed, maxAccel);
        transform.position = position;
    }

    void LateUpdate()
    {
        const float readySpeed = 0.1f;
        if (!ready && speed < readySpeed)
        {
            ready = true;
            squad.waiting--;
        }
    }

    void OnDestroy()
    {
        squad.active--;
        if (!ready)
        {
            squad.waiting--;
        }
    }

    protected bool SquadReady()
    {
        return squad.waiting == 0;
    }
}
