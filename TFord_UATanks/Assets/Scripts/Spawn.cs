using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public List<GameObject> powerups;
    public float spawnDelay;

    private float nextSpawnTime;
    private Transform tf;
    private GameObject spawnedPickup;

	// Use this for initialization
	void Start ()
    {
        tf = gameObject.GetComponent<Transform>();
        nextSpawnTime = Time.time + spawnDelay;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //If it is there nothing spawns
        if (spawnedPickup == null)
        {
            //And it is time to spawn
            if (Time.time > nextSpawnTime)
            {
                //Spawn it and set the next time
                GameObject powerup = powerups[UnityEngine.Random.Range(0, powerups.Count)];
                spawnedPickup = Instantiate(powerup, tf.position, Quaternion.identity) as GameObject;
                spawnedPickup.transform.position += new Vector3(0, 1, 0);
                nextSpawnTime = Time.time + spawnDelay;
            }
        }
        else
        {
            //Otherwise, the object still exists, so postpone the spawn
            nextSpawnTime = Time.time + spawnDelay;
        }
	}
}
