using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    //Declare Variables
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float shotCooldown = 2;
    public float bulletSpeed = 6;
    public TankData firingTankData;
    public AudioClip tankFire;
    public float defaultSoundEffectsVolume = 0.5f;

    private float lastShot;
    private string soundEffectsVolumeKey = "SoundEffectsVolume";

    // Use this for initialization
    void Start ()
    {
        //set the last shot to be the currewnt time minus the cooldown so we cna fire immediately
        lastShot = Time.time - shotCooldown;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!PlayerPrefs.HasKey(soundEffectsVolumeKey))
        {
            PlayerPrefs.SetFloat(soundEffectsVolumeKey, defaultSoundEffectsVolume);
        }
    }

    public void Fire()
    {
        //have we reached the cooldown
        if (Time.time - lastShot > shotCooldown)
        {
            //Play Audio Clip
            AudioSource.PlayClipAtPoint(tankFire, bulletSpawn.position, PlayerPrefs.GetFloat(soundEffectsVolumeKey));

            // Create the Bullet from the Bullet Prefab
            var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

            // Add velocity to the bullet
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

            //Set the tank data fo the bullet
            bullet.GetComponent<BulletController>().firingTankData = firingTankData;

            // Destroy the bullet after 2 seconds
            Destroy(bullet, 10.0f);

            //set lastshot to the current time
            lastShot = Time.time;
        }
    }
}