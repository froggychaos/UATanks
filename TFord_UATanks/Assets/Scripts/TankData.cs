﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankData : MonoBehaviour
{
    public enum TankType
    {
        Undefined,
        NPC,
        Player_1,
        Player_2
    }

    //Declare variables that will be used by other classes and designers
    public float moveSpeed = 3;
    public float turnSpeed = 180; // in degrees per second
    public float health = 100;
    public float maxHealth = 100;
    public int bulletDmg = 50;
    public int pointValue = 10;
    public float fireRate = 2;
    public float healValue = 10;
    public float healCooldown = 5;
    public TankType type = TankType.NPC;
    public TankType whoShotMeLast = TankType.Undefined;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}