using System.Collections.Generic;
using UnityEngine;
using System;

public class MushroomPathfinding : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    EnemyController controller;
    GameObject player;
    Rigidbody2D rigidbody;
    Animator animator;
    TouchingDirections touchingDirections;
    Collider2D selfCollider;
    DetectionZone cliffDetectionZone;
    CooldownTimer cooldownHandler;


    private float distanceToPlayer;
    private float yVelocity;
    private int lookDirection = 1;
    private int moveDirection;

    // States
    public bool IsMoving { get { return isMoving; } private set { isMoving = value; animator.SetBool("isMoving", value); } }
    [SerializeField]
    private bool isMoving = false;
    public bool CanMove { get { return canMove; } private set { canMove = value; } }
    [SerializeField]
    private bool canMove = true;
    public bool CanAttack { get { return canAttack; } private set { canAttack = value; } }
    [SerializeField]
    private bool canAttack = true;
    public bool CurrentlyTrackingPlayer;
    public bool runAwayTracking;

    void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        controller = GetComponent<EnemyController>();
        selfCollider = GetComponent<Collider2D>();
        cooldownHandler = GetComponent<CooldownTimer>();
        player = GameObject.Find("Player");
        cliffDetectionZone = GameObject.Find("CliffDetectionZone").GetComponent<DetectionZone>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //
        List<string> keyList = new List<string> { "attackCooldown", "cliffDetectionInterval", "attackLockTime", "invulnerableOnHitTime", "runAwayTime" };
        List<float> lengthList = new List<float> { controller.attackCooldown, controller.cliffDetectionInterval, controller.attackLockTime, controller.invulnerableOnHitTime, controller.runAwayTime  };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    // Fixed Update is called every set interval (about every 0.02 seconds)
    void FixedUpdate()
    {
        // Checks envionmental collisions
        touchingDirections.CheckCollisions();

        // Cooldown Timer logic
        cooldownHandler.CheckCooldowns();

        // Makes player look in correct direction
        LookingDirection();

        // Choose how to make enemy move
        CalcDistanceToPlayer();


        // If the player is within the enemy's tracking proximity and at a higher y value, stops tracking through walls as much
        if (distanceToPlayer <= controller.playerRequiredProximity && player.transform.position.y > selfCollider.transform.position.y)
        {
            // If not curently tracking player, the mushroom is now tracking the player, they will stand stil and begin to fire projectiles
            CurrentlyTrackingPlayer = true;
        }
        // If player leaves proximity, the enemy stops tracking them
        else
        {
            CurrentlyTrackingPlayer = false;
            runAwayTracking = false;
            // flip away from player
        }

        // Tracking player movement decisions
        if (CurrentlyTrackingPlayer && touchingDirections.IsGrounded)
        {
            if (runAwayTracking)
            {
                moveDirection = -1 * (player.transform.position.x > selfCollider.transform.position.x ? 1 : -1);
                lookDirection = moveDirection;
            }
            else 
            {
                moveDirection = 0;
                lookDirection = player.transform.position.x > selfCollider.transform.position.x ? 1 : -1;
            }
        }

        // Non tracking movement decisions
        else
        {
            // Default walk cycle movement
            if (touchingDirections.IsGrounded)
            {
                moveDirection = lookDirection;
                yVelocity = 0;
            }
            // Flip direction at collision with wall
            if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
            {
                moveDirection = -1 * lookDirection;
                yVelocity = 0;
            }
            // Not move if in air, allow gravity to affect
            if (!touchingDirections.IsGrounded)
            {
                moveDirection = 0;
                yVelocity = rigidbody.linearVelocityY;
            }
        }

        // Ovveride previous decisions if can't move
        if (!CanMove)
        {
            moveDirection = 0;
        }

        // Keeps movement direction, look direction and localscale in parity
        if (lookDirection == -1 && transform.localScale.x > 0) { transform.localScale *= new Vector2(-1, 1); }
        if (lookDirection == 1 && transform.localScale.x < 0) { transform.localScale *= new Vector2(-1, 1); }

        // Limit enemy movement to maxspeed
        if (controller.currentSpeed <= controller.maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(controller.currentSpeed * moveDirection, yVelocity);
        }
        else if (controller.currentSpeed > controller.maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(controller.maxSpeed * moveDirection, yVelocity);
        }

        // Update ismoving variable and animator parameter based on current movement
        if (moveDirection == 0)
        {
            IsMoving = false;
        }
        else
        {
            IsMoving = true;
        }
    }

    // Allows specific processes to be coded in to happen once a cooldown ends
    public void CooldownEndProcess(string key)
    {
        if (key == "attackLockTime")
        {
            CanMove = true;
        }
        if (key == "invulnerableOnHitTime")
        {
            controller.IsInvulnerable = false;
        }
        if (key == "runAwayTime")
        {
            runAwayTracking = false;
        }
    }

    public void LookingDirection()
    {
        if (moveDirection == 1 && lookDirection != 1)
        {
            // Changes look direction to 1 (right)
            lookDirection = 1;
            // Flips transform
            transform.localScale *= new Vector2(-1, 1);
        }
        if (moveDirection == -1 && lookDirection != -1)
        {
            // Changes look direction to -1 (left)
            lookDirection = -1;
            // Flips transform
            transform.localScale *= new Vector2(-1, 1);
        }
    }

    // Detection Methods
    public void OnCliffDetected()
    {
        // Flip enemy if a cliff is detected, but only every few seconds to avoid erratic and repeated flipping
        if (!CurrentlyTrackingPlayer && touchingDirections.IsGrounded && cooldownHandler.timerStatusDict["cliffDetectionInterval"] == 0)
        {
            lookDirection = -1 * lookDirection;
            moveDirection = lookDirection;
            transform.localScale *= new Vector2(-1, 1);
            cooldownHandler.timerStatusDict["cliffDetectionInterval"] = 1;
        }
    }

    public void CalcDistanceToPlayer()
    {
        // Calculate the distance to the player with vector math
        Vector2 offset = transform.position - player.transform.position;
        distanceToPlayer = offset.magnitude;

        // Once in a set x distance of the player, continue tracking them but stand still
        if (Math.Abs(transform.position.x - player.transform.position.x) <= controller.runAwayTrackingProximity && runAwayTracking == false)
        {
            runAwayTracking = true;
            cooldownHandler.timerStatusDict["runAwayTime"] = 1;
        }
    }

    public void FindOptimalMovePath() { }

    public void ExecuteEnemyAttack()
    {
    }
}
