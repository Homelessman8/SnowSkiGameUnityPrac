using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Contains tunable parameters to tweak the player's movement.
    [System.Serializable]
    public struct Stats
    {
        [Header("Movement Settings")]
        [Tooltip("The player's current speed.")]
        public float speed;

        [Tooltip("The player's Fastest speed.")]
        public float speedMaximum;

        [Tooltip("The player's Slowest speed.")]
        public float speedMinimum;

        [Tooltip("The player's Turning left/right speed.")]
        public float turnSpeed;

        [Tooltip("How much speed the player picks up as they are turning towards the center.")]
        public float turnAcceleration;

        [Tooltip("How much speed the player picks up as they are turning towards the sides.")]
        public float turnDeceleration;

    }

    public Stats playerStats;

    [Tooltip("Keyboard controls for steering left and right.")]
    public KeyCode left, right;

    [Tooltip("whather player is moving down hill or not.")]
    public bool isMoving;

    [Tooltip("Child gameobject to check if we are on the ground")]
    public Transform groundCheck;

    [Tooltip("LayerMask to hold layer to check for being grounded")]
    public LayerMask groundLayer;

    private Rigidbody rb;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //grabs references to components
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //If function to check ground and if player is moving
        if(isMoving)
        {
            //LineCast requires a starting position and end position (groundCheck) 
            bool onGround = Physics.Linecast(transform.position, groundCheck.position, groundLayer);
            if(onGround)
            {
                if(Input.GetKey(left))
                {
                    TurnLeft();
                }
                if(Input.GetKey(right))
                {
                    TurnRight();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //Processing Physics
        if(isMoving)
        {
            //increase or decreae the player speed depending on how much they are facing downhill
            float turnAngle = Mathf.Abs(180 - transform.eulerAngles.y);
            playerStats.speed += Remap(0, 90, playerStats.turnAcceleration, -playerStats.turnDeceleration, turnAngle);

            //move the player forward based on whice directiong they are facing
            Vector3 velocity = (transform.forward) * playerStats.speed * Time.fixedDeltaTime;
            velocity.y = rb.linearVelocity.y;
            rb.linearVelocity = velocity;
        }

        //Update the Animator's State depending on our speed;
        animator.SetFloat("playerSpeed", playerStats.speed);

    }

    //----------------------------------------METHODS-----------------------------------------

    private void TurnLeft()
    {
        // rotates the player, limiting them after reaching a certain angle
        if(transform.eulerAngles.y > 91)
        {
            transform.Rotate(new Vector3(0, -playerStats.turnSpeed, 0) * Time.deltaTime, Space.Self);
        }
    }

    private void TurnRight()
    {
        // rotates the player, limiting them after reaching a certain angle
        if (transform.eulerAngles.y < 269)
        {
            transform.Rotate(new Vector3(0, playerStats.turnSpeed, 0) * Time.deltaTime, Space.Self);
        }
    }

    private void ControlSpeed()
    {
        //limits the player's speed when reaching past the speed maximum
        if(playerStats.speed > playerStats.speedMaximum)
        {
            playerStats.speed = playerStats.speedMaximum;
        }

        //limits the player's speed when reaching past the speed maximum
        if (playerStats.speed < playerStats.speedMinimum)
        {
            playerStats.speed = playerStats.speedMinimum;
        }
    }

    //remap a number from a given range into a new range
    private float Remap(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
        return (NewValue);
    }
}
