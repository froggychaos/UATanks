using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{
    public enum AI_Personality
    {
        Aggressive,
        Cautious,
        Patrol,
        Sentry,
        Kamikaze
    }

    public enum AI_State
    {
        Idle,
        Search,
        Patrol,
        Chase,
        Attack,
        Hide
    }

    public enum LoopType
    {
        Stop,
        Loop,
        PingPong
    };

    //Declare Variables
    public TankMotor motor;
    public TankData data;
    public Shooter shooter;
    public AI_Senses senses;
    public AI_Personality personality;
    public AI_State state;
    public float cautiousHideHealthThreshold = 0.60f;
    public float cautiousStopHidingHealthThreshold = 0.80f;
    public float kamikazeSuicideHealthThreshold = 0.50f;
    public float fieldOfView = 45;
    public GameObject target;
    public float attackRange = 5000;
    public List<Transform> waypoints;
    public LoopType loopType;
    public float closeEnough = 1.0f;
    public float fleeDistance = 1.0f;
    public GameObject room;

    private bool isPatrolForward = true;
    private int currentWaypoint = 0;
    private Transform tf;
    private GameObject gameManager;
    private Manager manager;

    public void Awake()
    {
        //Get the transform of the tank
        tf = gameObject.GetComponent<Transform>();

        gameManager = GameObject.FindGameObjectWithTag("GameController");
        manager = gameManager.GetComponent<Manager>();
    }

    // Use this for initialization
    void Start ()
    {
        foreach (GameObject waypoint in room.GetComponent<Room>().waypoints)
        {
            waypoints.Add(waypoint.transform);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        FindTarget();

        switch (personality)
        {
            case AI_Personality.Aggressive:
                Aggressive_AI();
                break;
            case AI_Personality.Cautious:
                Cautious_AI();
                break;
            case AI_Personality.Kamikaze:
                Kamikaze_AI();
                break;
            case AI_Personality.Patrol:
                Patrol_AI();
                break;
            case AI_Personality.Sentry:
                Sentry_AI();
                break;
        }
	}

    void Aggressive_AI()
    {
        switch (state)
        {
            case AI_State.Idle:
                //Perform Behaviors
                //This is the deault state for this personality
                //Do Nothing

                //Check for transitions
                if (senses.CanSee(target) || senses.CanHear(target))
                {
                    Debug.Log("Player spotted switching to chase mode");
                    state = AI_State.Chase;
                }
                else
                {
                    Debug.Log("player not found switching to patrol mode");
                    state = AI_State.Patrol;
                }
                break;
            case AI_State.Search:
                //Perform Behaviors
                //We should never be in this state
                //Do Nothing

                //Check for transitions
                //We shouldn't be here so Switch back to idle
                state = AI_State.Idle;
                break;
            case AI_State.Patrol:
                //Perform Behaviors
                Patrol();

                //Check for transitions
                if (senses.CanSee(target) || senses.CanHear(target))
                {
                    Debug.Log("Player spotted switching to chase mode");
                    state = AI_State.Chase;
                }
                break;
            case AI_State.Chase:
                //Perform Behaviors
                ChasePlayer();

                //Check for transitions
                if (!senses.CanHear(target) && !senses.CanSee(target))
                {
                    Debug.Log("player not found switching to patrol mode");
                    state = AI_State.Patrol;
                }
                else if (IsPlayerWithinRange())
                {
                    Debug.Log("Player spotted switching to attack mode");
                    state = AI_State.Attack;
                }

                break;
            case AI_State.Attack:
                //Perform Behaviors
                Attack();

                //Check for transitions
                if (!senses.CanSee(target) && !senses.CanHear(target))
                {
                    Debug.Log("player not found switching to patrol mode");
                    state = AI_State.Patrol;
                }
                break;
            case AI_State.Hide:
                //Perform Behaviors
                //We Should Never Be In This State
                //Do Nothing

                //Check for transitions
                //We shouldn't be here so Switch back to idle
                state = AI_State.Idle;
                break;
        }
    }

    void Cautious_AI()
    {
        switch (state)
        {
            case AI_State.Idle:
                //Perform Behaviors
                //We should never be in this state
                //Do Nothing

                //Check for transitions
                if (data.health <= data.maxHealth * cautiousHideHealthThreshold)
                {
                    state = AI_State.Hide;
                }
                else if (senses.CanSee(target) || senses.CanHear(target))
                {
                    state = AI_State.Chase;
                }
                else
                {
                    state = AI_State.Patrol;
                }
                break;
            case AI_State.Search:
                //Perform Behaviors
                //We should never be in this state
                //Do Nothing

                //Check for transitions
                //We shouldn't be here so Switch back to idle
                state = AI_State.Idle;
                break;
            case AI_State.Patrol:
                //Perform Behaviors
                Patrol();

                //Check for transitions
                if (data.health <= data.maxHealth * cautiousHideHealthThreshold)
                {
                    state = AI_State.Hide;
                }
                else if (senses.CanSee(target) || senses.CanHear(target))
                {
                    state = AI_State.Chase;
                }
                break;
            case AI_State.Chase:
                //Perform Behaviors
                ChasePlayer();

                //Check for transitions
                if (data.health <= data.maxHealth * cautiousHideHealthThreshold)
                {
                    state = AI_State.Hide;
                }
                else if (!senses.CanHear(target) && !senses.CanSee(target))
                {
                    state = AI_State.Patrol;
                }
                else if (IsPlayerWithinRange())
                {
                    state = AI_State.Attack;
                }
                break;
            case AI_State.Attack:
                //Perform Behaviors
                Attack();

                //Check for transitions
                if (data.health <= data.maxHealth * cautiousHideHealthThreshold)
                {
                    state = AI_State.Hide;
                }
                else if (!senses.CanSee(target) && !senses.CanHear(target))
                {
                    state = AI_State.Patrol;
                }
                break;
            case AI_State.Hide:
                //Perform Behaviors
                Hide();

                //Check for transitions
                if (data.health >= data.maxHealth * cautiousStopHidingHealthThreshold)
                {
                    if (senses.CanSee(target) || senses.CanHear(target))
                    {
                        ChasePlayer();
                    }
                    else
                    {
                        Patrol();
                    }
                }
                break;
        }
    }

    void Kamikaze_AI()
    {
        switch (state)
        {
            case AI_State.Idle:
                //Perform Behaviors
                //Default State
                //Do Nothing

                //Check for transitions
                if (senses.CanSee(target) || senses.CanHear(target))
                {
                    state = AI_State.Chase;
                }
                else
                {
                    state = AI_State.Patrol;
                }
                break;
            case AI_State.Search:
                //Perform Behaviors
                //We should never be in this state
                //Do Nothing

                //Check for transitions
                //We shouldn't be here so Switch back to idle
                state = AI_State.Idle;
                break;
            case AI_State.Patrol:
                //Perform Behaviors
                Patrol();

                //Check for transitions
                if (senses.CanSee(target) || senses.CanHear(target))
                {
                    state = AI_State.Chase;
                }
                break;
            case AI_State.Chase:
                //Perform Behaviors
                ChasePlayer();

                //Check for transitions
                if (senses.CanSee(target) || senses.CanHear(target))
                {
                    if (data.health > data.maxHealth * kamikazeSuicideHealthThreshold && IsPlayerWithinRange())
                    {
                        state = AI_State.Attack;
                    }
                }
                else
                {
                    state = AI_State.Patrol;
                }
                break;
            case AI_State.Attack:
                //Perform Behaviors
                Attack();

                //Check for transitions
                if (senses.CanSee(target) || senses.CanHear(target))
                {
                    if (data.health < data.maxHealth * kamikazeSuicideHealthThreshold || !IsPlayerWithinRange())
                    {
                        ChasePlayer();
                    }
                }
                else
                {
                    Patrol();
                }
                break;
            case AI_State.Hide:
                //Perform Behaviors
                //We should never be in this state
                //Do Nothing

                //Check for transitions
                //We shouldn't be here so Switch back to idle
                state = AI_State.Idle;
                break;
        }
    }

    void Patrol_AI()
    {
        switch (state)
        {
            case AI_State.Idle:
                //Perform Behaviors
                //Default State
                //Do Nothing

                //Check for transitions
                if (senses.CanSee(target) || senses.CanHear(target))
                {
                    if (IsPlayerWithinRange())
                    {
                        state = AI_State.Attack;
                    }
                }
                else
                {
                    state = AI_State.Patrol;
                }
                break;
            case AI_State.Search:
                //Perform Behaviors
                //We should never be in this state
                //Do Nothing

                //Check for transitions
                //We shouldn't be here so Switch back to idle
                state = AI_State.Idle;
                break;
            case AI_State.Patrol:
                //Perform Behaviors
                Patrol();

                //Check for transitions
                if (senses.CanSee(target) || senses.CanHear(target))
                {
                    if (IsPlayerWithinRange())
                    {
                        state = AI_State.Attack;
                    }
                }
                break;
            case AI_State.Chase:
                //Perform Behaviors
                //We should never be in this state
                //Do Nothing

                //Check for transitions
                //We shouldn't be here so Switch back to idle
                state = AI_State.Idle;
                break;
            case AI_State.Attack:
                //Perform Behaviors
                Attack();

                //Check for transitions
                if (!senses.CanSee(target) && !senses.CanHear(target))
                {
                    state = AI_State.Patrol;
                }
                else
                {
                    if (!IsPlayerWithinRange())
                    {
                        state = AI_State.Patrol;
                    }
                }
                break;
            case AI_State.Hide:
                //Perform Behaviors
                //We should never be in this state
                //Do Nothing

                //Check for transitions
                //We shouldn't be here so Switch back to idle
                state = AI_State.Idle;
                break;
        }
    }

    void Sentry_AI()
    {
        switch (state)
        {
            case AI_State.Idle:
                //Perform Behaviors
                //Default State
                //Do Nothing

                //Check for transitions
                state = AI_State.Search;
                break;
            case AI_State.Search:
                //Perform Behaviors
                Search();

                //Check for transitions
                if (senses.CanSee(target) || senses.CanHear(target))
                {
                    if (IsPlayerWithinRange())
                    {
                        state = AI_State.Attack;
                    }
                }
                break;
            case AI_State.Patrol:
                //Perform Behaviors
                //We should never be in this state
                //Do Nothing

                //Check for transitions
                //We shouldn't be here so Switch back to idle
                state = AI_State.Idle;
                break;
            case AI_State.Chase:
                //Perform Behaviors
                //We should never be in this state
                //Do Nothing

                //Check for transitions
                //We shouldn't be here so Switch back to idle
                state = AI_State.Idle;
                break;
            case AI_State.Attack:
                //Perform Behaviors
                Attack();

                //Check for transitions
                if (!senses.CanSee(target) && !senses.CanHear(target))
                {
                    state = AI_State.Search;
                }
                else
                {
                    if (!IsPlayerWithinRange())
                    {
                        state = AI_State.Search;
                    }
                }
                break;
            case AI_State.Hide:
                //Perform Behaviors
                //We should never be in this state
                //Do Nothing

                //Check for transitions
                //We shouldn't be here so Switch back to idle
                state = AI_State.Idle;
                break;
        }
    }

    void Patrol()
    {
        if (waypoints.Count > 0 && target != null)
        {
            if (motor.RotateTowards(waypoints[currentWaypoint].position, data.turnSpeed))
            {
                //Do Nothing!
            }
            else
            {
                //Move Forward
                motor.Move(data.moveSpeed);
            }

            //If we are close to the waypoint
            if (Vector3.SqrMagnitude(waypoints[currentWaypoint].position - tf.position) < (closeEnough * closeEnough))
            {
                if (loopType == LoopType.Stop)
                {
                    //Advance to the next waypoint;
                    if (currentWaypoint < waypoints.Count - 1)
                    {
                        currentWaypoint++;
                    }
                }
                else if (loopType == LoopType.Loop)
                {
                    //Advance to the next waypoint, if we are still in range
                    if (currentWaypoint < waypoints.Count - 1)
                    {
                        currentWaypoint++;
                    }
                    else
                    {
                        currentWaypoint = 0;
                    }

                }
                else if (loopType == LoopType.PingPong)
                {
                    if (isPatrolForward)
                    {
                        //Advance to next waypoint, if we are still in range
                        if (currentWaypoint < waypoints.Count - 1)
                        {
                            currentWaypoint++;
                        }
                        else
                        {
                            //otherwise reverse direction and decrement our current waypoint
                            isPatrolForward = false;
                            currentWaypoint--;
                        }
                    }
                    else
                    {
                        //Advance to the next waypoint, if we are still in range
                        if (currentWaypoint > 0)
                        {
                            currentWaypoint--;
                        }
                        else
                        {
                            //otherwise reverse direction and increment our current waypoint
                            isPatrolForward = true;
                            currentWaypoint++;
                        }
                    }
                }
            }
        }
    }

    void ChasePlayer()
    {
        if (senses.CanSee(target))
        {
            //turn towards player
            motor.RotateTowards(target.transform.position, data.turnSpeed);

            //move towards player
            motor.Move(data.moveSpeed);
        }
        else if (senses.CanHear(target))
        {
            //turn towards player
            motor.RotateTowards(target.transform.position, data.turnSpeed);
        }
    }

    bool IsPlayerWithinRange()
    {
        if (Vector3.Distance(target.transform.position, tf.position) < attackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Attack()
    {
        if (senses.CanSee(target))
        {
            //turn towards player
            bool notFacingPlayer = motor.RotateTowards(target.transform.position, data.turnSpeed);

            //Are we facing the player?
            if (!notFacingPlayer)
            {
                motor.Shoot();
            }
        }
        else if (senses.CanHear(target))
        {
            motor.RotateTowards(target.transform.position, data.turnSpeed);
        }
    }

    void Hide()
    {
        //The vector from AI to target is target position minus our position
        Vector3 vectorToTarget = target.transform.position - tf.position;

        //We can flip this vector by -1 to get a vector AWAY from our target
        Vector3 vectorAwayFromTarget = -1 * vectorToTarget;

        //Now we can normalize our vector to give it a maginitude of 1
        vectorAwayFromTarget.Normalize();

        //A normalized vector can be multiplied by a length to make a vector that length
        vectorAwayFromTarget *= fleeDistance;

        //We can find the position in space we want to move to by adding our vector away from our AI to our AI's position
        //This gives us a point that is "that vector away" from our current position
        Vector3 fleePosition = vectorAwayFromTarget + tf.position;
        motor.RotateTowards(fleePosition, data.turnSpeed);
        motor.Move(data.moveSpeed);
    }

    void Search()
    {
        motor.Rotate(data.turnSpeed);
    }

    private void FindTarget()
    {
        foreach(GameObject player in manager.players)
        {
            if (target == null)
            {
                target = player;
            }
            else
            {
                if (Vector3.Distance(tf.position, player.transform.position) < Vector3.Distance(tf.position, target.transform.position))
                {
                    target = player;
                }
            }
        }
    }
}