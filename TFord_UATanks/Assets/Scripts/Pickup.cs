using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Powerup powerup;
    public AudioClip feedback;
    public float defaultSoundEffectsVolume = 0.5f;

    private Transform tf;
    private string soundEffectsVolumeKey = "SoundEffectsVolume";

    // Use this for initialization
    void Start ()
    {
        tf = gameObject.GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnTriggerEnter(Collider other)
    {
        //variable to store other object's PowerupController - if it has one
        PowerupController powCon = other.gameObject.GetComponent<PowerupController>();

        //If the other object has a PowerupController
        if (powCon != null)
        {
            //Add the powerup
            powCon.Add(powerup);

            //Play feedback(if it is set)
            if (feedback != null)
            {
                if (!PlayerPrefs.HasKey(soundEffectsVolumeKey))
                {
                    PlayerPrefs.SetFloat(soundEffectsVolumeKey, defaultSoundEffectsVolume);
                }

                float volume = Mathf.Clamp(PlayerPrefs.GetFloat(soundEffectsVolumeKey), 0.0f, 1.0f);
                AudioSource.PlayClipAtPoint(feedback, tf.position, volume);
            }

            //Destroy this pickup
            Destroy(gameObject);
        }
    }
}
