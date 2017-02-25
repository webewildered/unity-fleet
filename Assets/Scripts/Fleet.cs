using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fleet : MonoBehaviour
{
    // Maximum number of ships allowed
    public const int MaxShips = 21;

    public GameObject shipPrefab;

    List<FleetShip> ships;  // List of all ships in the fleet, no null entries
    FleetShip[] slots;      // Ship in each slot of the formation, null entries for empty slots
    Formation formation;    // Current formation
    int rotation;           // Rotates the ships' position in the formation

    Vector3 target;         // Target fleet position
    Vector3 velocity;       // Fleet velocity, used to apply max acceleration

    static Fleet instance;
    public static Fleet Instance { get { return instance; } }

    public Vector3 Center { get { return formation.center; } }

    public Vector3 Velocity { get { return velocity; } }

    void Start()
    {
        // Singleton
        instance = this;

        // Set up initial state
        {
            ships = new List<FleetShip>();
            slots = new FleetShip[MaxShips];

            // Build the initial ships
            formation = Formation.Get(Formation.V, 0);
            const int defaultSize = 1;
            for (int i = 0; i < defaultSize; i++)
            {
                addShip();
            }

            // Start in default formation
            foreach (FleetShip ship in ships)
            {
                ship.gameObject.transform.localPosition = ship.target;
            }
            formCurrent();
        }
    }

	void Update ()
    {
        //
        // Change formation
        //

	    if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.V))
        {
            form(Formation.V);
        }
        else if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.C))
        {
            form(Formation.C);
        }
        else if (Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.X))
        {
            form(Formation.X);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            addShip();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            removeShip();
            formCurrent();
        }

        //
        // Movement input
        //

        Vector3 targetVelocity = Vector3.zero;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            targetVelocity.z -= 1.0f;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            targetVelocity.z += 1.0f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            targetVelocity.x -= 1.0f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            targetVelocity.x += 1.0f;
        }
        
        const float speed = 20.0f;
        targetVelocity *= speed;

        //
        // Movement
        //

        // Limit acceleration
        const float maxAccel = 150.0f;
        float maxSpeedChange = maxAccel * Time.deltaTime;
        Vector3 velocityChange = (targetVelocity - velocity);
        if (velocityChange.magnitude > maxSpeedChange)
        {
            velocityChange *= maxSpeedChange / velocityChange.magnitude;
        }

        // Apply
        velocity += velocityChange;
        transform.position += velocity * Time.deltaTime;
    }

    void LateUpdate()
    {
    }
    
    void form(int newFormationType)
    {
        if (ships.Count == 0)
        {
            return;
        }

        if (newFormationType == formation.type)
        {
            rotation = (rotation + 1) % ships.Count;
        }

        formation = Formation.Get(newFormationType, ships.Count);
        formCurrent();
    }

    void setSlot(FleetShip ship, int slotIndex)
    {
        Formation.Slot slot = formation.slots[slotIndex];
        ship.target.x = slot.x;
        ship.target.z = slot.z;
        ship.fire = false; // Call updateFiring()
        ship.slot = slotIndex;
        slots[slotIndex] = ship;
    }

    void formCurrent()
    {
        formation = Formation.Get(formation.type, ships.Count);

        for (int i = 0; i < ships.Count; i++)
        {
            setSlot(ships[i], (i + rotation) % ships.Count);
        }

        updateFiring();
    }

    void updateFiring()
    {
        for (int i = 0; i < ships.Count; i++)
        {
            int slot = ships[i].slot;
            while (true)
            {
                slot = formation.slots[slot].forward;
                if (slot == -1)
                {
                    ships[i].fire = true;
                    break;
                }
                else if (slots[slot] != null)
                {
                    ships[i].fire = false;
                    break;
                }
            }
        }
    }

    void addShip()
    {
        if (ships.Count < MaxShips)
        {
            // Create the ship
            GameObject shipObj = GameObject.Instantiate(shipPrefab);
            FleetShip ship = shipObj.GetComponent<FleetShip>();
            ship.fleet = this;
            ship.name = "Ship" + ships.Count;
            ship.transform.parent = transform;
            ships.Add(ship);

            // If there are any empty slots in the current formation, take the first one
            for (int i = 0; i < formation.slots.Length; i++)
            {
                if (slots[i] == null)
                {
                    setSlot(ship, i);
                    updateFiring();
                    return;
                }
            }

            // If no empty slot was found, choose a larger formation
            {
                formation = Formation.Get(formation.type, ships.Count);

                // Position the new ship
                setSlot(ship, formation.newSlot);

                // Re-position the old ships
                for (int i = 0; i < ships.Count - 1; i++)
                {
                    setSlot(ships[i], formation.slots[ships[i].slot].newIndex);
                }
            }

            updateFiring();
        }
    }

    void removeShip()
    {
        if (ships.Count > 0)
        {
            FleetShip ship = ships[ships.Count - 1];
            RemoveShip(ship);
        }
    }

    public void RemoveShip(FleetShip ship)
    {
        // Remove ship from the list
        bool success = ships.Remove(ship);
        System.Diagnostics.Debug.Assert(success);
        if (!success)
        {
            return;
        }

        // If something was behind the ship, see if it can fire now
        slots[ship.slot] = null;
        int backward = formation.slots[ship.slot].backward;
        if (backward >= 0)
        {
            FleetShip backShip = slots[backward];
            if (backShip != null)
            {
                int forward = formation.slots[ship.slot].forward;
                while (true)
                {
                    if (forward == -1)
                    {
                        // Shot is clear
                        backShip.fire = true;
                        break;
                    }
                    else if (slots[forward] != null)
                    {
                        break;
                    }

                    forward = formation.slots[forward].forward;
                }
            }
        }

        // Destroy the ship
        GameObject.Destroy(ship.gameObject);
    }
}
