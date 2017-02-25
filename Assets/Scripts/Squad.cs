using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to synchronize a group of enemy ships
public class Squad
{
    public int waiting; // Number of ships that are not ready to fire yet
    public int active;  // Number of ships that haven't been destroyed or left the scene
    public bool ready;  // Set true at the end of the first frame when all ships are ready to fire
}
