using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    List<IUpdatable> update;
    List<IUpdatable> remove;
    float fixedDt;
    float updateDt;

    void FixedUpdate()
    {
        float currentDt = Time.fixedDeltaTime - updateDt;
        fixedDt += currentDt;
        updateDt = 0.0f;
        doUpdates(currentDt);
    }

    // Update is called once per frame
    void Update ()
    {
        updateDt = Time.deltaTime - fixedDt;
        doUpdates(updateDt);
        fixedDt = 0.0f;
	}

    void doUpdates(float dt)
    {
        // Clear out removed entries
        for (int i = 0; i < remove.Count; i++)
        {
            IUpdatable move = update[update.Count - i - 1];
            int handle = remove[i].Handle;
            update[handle] = move;
            move.Handle = handle;
        }
        update.RemoveRange(update.Count - remove.Count, remove.Count);
        remove.Clear();

        // Perform updates
        foreach (IUpdatable u in update)
        {
            u.CombinedUpdate(dt);
        }
    }

    void Add(IUpdatable u)
    {
        u.Handle = update.Count;
        update.Add(u);
    }

    void Remove(IUpdatable u)
    {
        remove.Add(u);
    }
}
