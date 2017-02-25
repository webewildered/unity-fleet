using UnityEngine;
using System.Collections;

public abstract class Ship : MonoBehaviour
{
    public abstract bool Hit(float damage);
}
