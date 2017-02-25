using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour
{
    static int enemyBulletLayer;
    public static int EnemyBulletLayer { get { return enemyBulletLayer; } }

    static int playerBulletLayer;
    public static int PlayerBulletLayer { get { return playerBulletLayer; } }

    void Start()
    {
        enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");
        playerBulletLayer = LayerMask.NameToLayer("PlayerBullet");
    }
}
