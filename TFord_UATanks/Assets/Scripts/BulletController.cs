using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    //Declare Variables
    public GameObject bullet;
    public TankData firingTankData;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        //destroy the bullet wheneve rit collides with anything
        Destroy(bullet);
    }
}