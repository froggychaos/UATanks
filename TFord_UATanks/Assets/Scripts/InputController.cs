using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    //Declare Variables
    public enum InputScheme { WASD, arrowKeys };
    public InputScheme input = InputScheme.WASD;
    public TankMotor motor;
    public TankData data;
    public Shooter shooter;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		switch (input)
        {
            case InputScheme.arrowKeys:
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    //move the tank forward
                    motor.Move(data.moveSpeed);
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    //move the tank backward
                    motor.Move(-data.moveSpeed);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    //turn the tank to the right
                    motor.Rotate(data.turnSpeed);
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    //turn the tank to the left
                    motor.Rotate(-data.turnSpeed);
                }
                if (Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    //fire cannon
                    shooter.Fire();
                }
                break;
            case InputScheme.WASD:
                if (Input.GetKey(KeyCode.W))
                {
                    //move the tank forward
                    motor.Move(data.moveSpeed);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    //move the tank backward
                    motor.Move(-data.moveSpeed);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    //turn the tank to the right
                    motor.Rotate(data.turnSpeed);
                }
                if (Input.GetKey(KeyCode.A))
                {
                    //turn the tank to the left
                    motor.Rotate(-data.turnSpeed);
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //fire cannon
                    shooter.Fire();
                }
                break;
            default:
                break;
        }
	}
}