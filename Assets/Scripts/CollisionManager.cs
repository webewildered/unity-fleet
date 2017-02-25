using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionManager : MonoBehaviour
{
    struct ColliderState
    {
        public Vector3 LastPosition;
        public ICollider Collider;
    }

    List<ColliderState> states;

	void Start ()
    {
        states = new List<ColliderState>();
	}
	
	void Update ()
    {
        for (int i = 0; i < states.Count; i++)
        {

        }
    }

    public void AddCollider(ICollider collider)
    {
        ColliderState state = new ColliderState();
        state.Collider = collider;
        state.LastPosition = collider.Position;
        states.Add(state);
    }
}
