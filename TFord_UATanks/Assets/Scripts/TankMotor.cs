using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]

public class TankMotor : MonoBehaviour
{
    //Declare Variables
    private CharacterController characterController; //Character Controller Component
    private Transform tf;
    private Shooter shooter;

    public void Awake()
    {
        //Get the transform of the tank
        tf = gameObject.GetComponent<Transform>();
    }

    // Use this for initialization
    void Start ()
    {
        // Store the CharacterController in our variable
        characterController = gameObject.GetComponent<CharacterController>();
        shooter = gameObject.GetComponent<Shooter>();
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    // Move(): This function moves our tank forward.
    public void Move(float speed)
    {
        // Call SimpleMove() and send it our vector
        // Note that SimpleMove() will apply Time.deltaTime, and convert to meters per second for us!
        characterController.SimpleMove(transform.forward * speed);
    }

    // Rotate: This function rotates our tank
    public void Rotate(float speed)
    {
        // Now, rotate our tank by this value - we want to rotate in our local space (not in world space).
        tf.Rotate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }

    //RotateTowards (Target, Speed) - rotates towards the target (if possible).
    //if we rotate, then returns true. If we canm't rotate (because we are already facing the target) return false.
    public bool RotateTowards(Vector3 target, float speed)
    {
        Vector3 vectorToTarget;

        //The vector to our target is the DIFFERENCE between the target position and our position.
        //How would our position need to be different to reach the target? "Difference is subtraction!
        vectorToTarget = target - tf.position;

        //Find the quaternion that looks down the vector
        Quaternion targetRotation = Quaternion.LookRotation(vectorToTarget);
        if (targetRotation == tf.rotation)
        {
            return false;
        }

        //Otherwise:
        //Change our rotation so that we are closer to our target rotation, but never turn faster than our Turn Speed
        //Note that we use Time.deltaTime because we want to turn in "Degrees per Second" not "Degrees per Framedraw"
        tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRotation, speed * Time.deltaTime);

        // We rotated, so return true
        return true;
    }

    public void Shoot()
    {
        shooter.Fire();
    }
}