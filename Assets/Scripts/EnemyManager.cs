using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public GameObject dronePrefab;
    public GameObject spreadPrefab;
    public GameObject chainPrefab;
    public GameObject launcherPrefab;

    void Start ()
    {
        // Make a few enemy ships
        /*
        {
            Squad squad = new Squad();
            for (int i = 0; i < 5; i++)
            {
                GameObject shipObj = GameObject.Instantiate(dronePrefab);
                shipObj.transform.position = new Vector3(i * 5.0f, 0.0f, World.Top + 10.0f);

                DroneShip ship = shipObj.GetComponent<DroneShip>();
                ship.target = ship.transform.position + new Vector3(0.0f, 0.0f, -40.0f + i * 3.0f);
                ship.Join(squad);
            }
        }*/

        {
            Squad squad = new Squad();
            for (int i = 0; i < 1; i++)
            {
                GameObject shipObj = GameObject.Instantiate(launcherPrefab);
                shipObj.transform.position = new Vector3(World.Left - 10.0f, 0.0f, World.Top / 2.0f - i * 5.0f);

                LauncherShip ship = shipObj.GetComponent<LauncherShip>();
                ship.target = new Vector3(World.Left + 30.0f + i * 10.0f, 0.0f, World.Top / 2.0f - 5.0f);
                ship.Join(squad);
            }
        }

        /*
        {
            Squad squad = new Squad();
            for (int i = 0; i < 3; i++)
            {
                GameObject shipObj = GameObject.Instantiate(spreadPrefab);
                shipObj.transform.position = new Vector3(World.Left - 10.0f, 0.0f, World.Top / 2.0f - i * 5.0f);

                SpreadShip ship = shipObj.GetComponent<SpreadShip>();
                ship.target = new Vector3(World.Left + 10.0f + i * 10.0f, 0.0f, World.Top / 2.0f - 5.0f);
                ship.Join(squad);
            }
        }
        */

        /*
        {
            int seed = 380;// System.DateTime.Now.Millisecond;
            UnityEngine.Debug.Log("seed " + seed);
            Rng rng = new Rng(seed);
            ChainPath path = ChainPath.Random(rng, World.Min, World.Max);
            for (int i = 0; i < 10; i++)
            {
                GameObject shipObj = GameObject.Instantiate(chainPrefab);
                ChainShip ship = shipObj.GetComponent<ChainShip>();
                ship.SetPath(path, i);
            }
        }
        */
    }
	
	void Update ()
    {
	
	}
}
