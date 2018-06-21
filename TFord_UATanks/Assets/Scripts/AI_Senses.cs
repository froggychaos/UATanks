using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Senses : MonoBehaviour
{
    public float fieldOfView = 45;
    private Transform tf;
    public float viewDistance = 100;
    public float hearDistance = 50;

    public void Awake()
    {
        //Get the transform of the tank
        tf = gameObject.GetComponent<Transform>();
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public bool CanSee(GameObject target)
    {
        if (target)
        {
            //Find angle to target
            Vector3 vectorToTarget = target.transform.position - tf.position;
            float angleToTarget = Vector3.Angle(tf.forward, vectorToTarget);

            //If it is less than my FOV, then object is inside my FOV
            if (angleToTarget < fieldOfView)
            {
                //Check for line of sight
                Ray myRay = new Ray();
                RaycastHit hitInfo = new RaycastHit();
                myRay.origin = tf.position;
                myRay.direction = vectorToTarget;
                if (Physics.Raycast(myRay, out hitInfo, viewDistance))
                {
                    if (hitInfo.collider.gameObject == target)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //Else
                //  I can't see it, it is outside my FOV
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool CanHear(GameObject target)
    {
        if (target)
        {
            if (Vector3.Distance(target.transform.position, tf.position) < hearDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
