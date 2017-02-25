using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : Ship
{
    protected float health;
    protected int clearBlink = -1;

    public override bool Hit(float damage)
    {
        if (health < damage)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            health -= damage;
            clearBlink = Time.frameCount + 1;
            setEmission(Color.white);
        }

        return true;
    }

    public void OnRenderObject()
    {
        setEmission(Color.black);
    }

    void setEmission(Color color)
    {
        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material.SetColor("_EmissionColor", color);
        }
    }
}
