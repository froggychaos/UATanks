using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    //Declare Variables
    public TankData data;
    public GameObject tank;
    public Healer healer;
    public AudioClip tankDeath;
    public AudioClip bulletHit;
    public float defaultSoundEffectsVolume = 0.5f;
    
    private Manager manager;
    private string soundEffectsVolumeKey = "SoundEffectsVolume";
    private Transform tf;
    public float minY;

    // Use this for initialization
    void Start()
    {
        tf = gameObject.GetComponent<Transform>();

        //Get the manager class from the game manager object
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<Manager>();

        //Check if tank data is null
        if (data == null)
        {
            //get the tank data from the game object
            data = gameObject.GetComponent<TankData>();
        }

        minY = tf.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (tf.position.y < minY)
        {
            tf.position = new Vector3(tf.position.x, minY, tf.position.z);
        }

        if (!PlayerPrefs.HasKey(soundEffectsVolumeKey))
        {
            PlayerPrefs.SetFloat(soundEffectsVolumeKey, defaultSoundEffectsVolume);
        }

        if (data.health <= 0)
        {
            if (data.type == TankData.TankType.NPC)
            {
                switch (data.whoShotMeLast)
                {
                    case TankData.TankType.Player_1:
                        //use the game manager to increment the score
                        manager.IncrementPlayerOneScore(data.pointValue);
                        break;
                    case TankData.TankType.Player_2:
                        //use the game manager to increment the score
                        manager.IncrementPlayerTwoScore(data.pointValue);
                        break;
                }

                float volume = Mathf.Clamp(PlayerPrefs.GetFloat(soundEffectsVolumeKey), 0.0f, 1.0f);
                AudioSource.PlayClipAtPoint(tankDeath, tf.position, volume);
            }
            else if (data.type == TankData.TankType.Player_1)
            {
                if (!manager.CanPlayerRespawn(1))
                {
                    manager.playerOneGameOverImage.SetActive(true);
                }
            }
            else if (data.type == TankData.TankType.Player_2)
            {
                if (!manager.CanPlayerRespawn(2))
                {
                    manager.playerTwoGameOverImage.SetActive(true);
                }
            }

            //destroy tank
            Destroy(tank);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (data.type != TankData.TankType.NPC)
        {
            //check the tags of the object we collided with to see if its a bullet
            if (other.gameObject.tag == "EnemyBullet")
            {
                float volume = Mathf.Clamp(PlayerPrefs.GetFloat(soundEffectsVolumeKey), 0.0f, 1.0f);
                AudioSource.PlayClipAtPoint(bulletHit, tf.position, volume);

                data.whoShotMeLast = other.gameObject.GetComponent<BulletController>().firingTankData.type;

                healer.SetLastHit(Time.time);

                //reduce the health
                data.health -= data.bulletDmg;
            }
        }
        else
        {
            //check the tags of the object we collided with to see if its a bullet
            if (other.gameObject.tag == "PlayerBullet")
            {
                float volume = Mathf.Clamp(PlayerPrefs.GetFloat(soundEffectsVolumeKey), 0.0f, 1.0f);
                AudioSource.PlayClipAtPoint(bulletHit, tf.position, volume);

                data.whoShotMeLast = other.gameObject.GetComponent<BulletController>().firingTankData.type;

                healer.SetLastHit(Time.time);

                //reduce the health
                data.health -= data.bulletDmg;
            }
        }
    }
}