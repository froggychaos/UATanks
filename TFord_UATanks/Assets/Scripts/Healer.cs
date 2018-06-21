using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public TankData data;
    private float lastHit;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SetLastHit(float _lastHit)
    {
        lastHit = _lastHit;

        if (Time.time - lastHit > data.healCooldown)
        {
            Mathf.Clamp(data.health, data.health + data.healValue, data.maxHealth);
        }
    }
}
