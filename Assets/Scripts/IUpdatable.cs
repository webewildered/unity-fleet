using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdatable
{
    void CombinedUpdate(float dt);
    int Handle { get; set; }
}
