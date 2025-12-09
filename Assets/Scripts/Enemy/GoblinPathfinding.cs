using System.Collections.Generic;
using UnityEngine;
using System;

public class GoblinPathfinding : MonoBehaviour, LogicScript
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

    // Private variables for internal logic
    private float distanceToPlayer;
    private float yVelocity;
    [SerializeField]
    private int lookDirection = 1;
    [SerializeField]
    private int moveDirection;

    // States
    public bool IsMoving { get { return isMoving; } set { isMoving = value; animator.SetBool("isMoving", value); } }
    [Header("States")]
    [SerializeField]
    private bool isMoving = false;
    public bool CanMove = true;
    public bool CanAttack = true;
    public bool CurrentlyTrackingPlayer = false;
    public bool TrackingButNotMove = false;
    public bool TrackingOffCliff = false;
    public bool RunAwayTracking = false;

    private void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        controller = GetComponent<EnemyController>();
        selfCollider = GetComponent<Collider2D>();
        cooldownHandler = GetComponent<CooldownTimer>();
        player = GameObject.Find("Player");
        cliffDetectionZone = transform.Find("CliffDetectionZone").GetComponent<DetectionZone>();
    }

    // Fixed Update is called every set interval (about every 0.02 seconds)
    void FixedUpdate()
    {
        // Checks envionmental collisions
        touchingDirections.CheckCollisions();

        // Makes player look in correct direction
        LookingDirection();

        // Choose how to make enemy move
        CalcDistanceToPlayer();

        // If the player is within the enemy's tracking proximity
        if (distanceToPlayer <= controller.playerRequiredProximity)
        {
            // If not curently tracking player, the enemy is now tracking the player
            CurrentlyTrackingPlayer = true;
        }
        // If player leaves proximity, the enemy stops tracking them
        else
        {
            CurrentlyTrackingPlayer = false;
            TrackingOffCliff = false;
            TrackingButNotMove = false;
            // If stop tracking player but still at cliff edge, flip direction
            if (cliffDetectionZone.detectedColliders.Count == 0 && touchingDirections.IsGrounded)
            {
                lookDirection = -1 * lookDirection;
                moveDirection = lookDirection;
                transform.localScale *= new Vector2(-1, 1);
            }
        }
        // Turns off tracking off cliff if there is no longer a cliff detected, used when turning the enemy away from cliff
        if (TrackingOffCliff && cliffDetectionZone.detectedColliders.Count > 0)
        {
            TrackingOffCliff = false;
        }
        // Keep enemy looking at player while in track but not move zone
        if (TrackingButNotMove && CanMove && ((transform.position.x - player.transform.position.x > 0 && lookDirection != -1) || (transform.position.x - player.transform.position.x < 0 && lookDirection != 1)))
        {
            lookDirection *= -1;
        }

        // Tracking player movement decisions
        if (CurrentlyTrackingPlayer && touchingDirections.IsGrounded && cooldownHandler.timerStatusDict["attackLockTime"] == 0)
        {
            // Stops enemy from moving if they are still tracking, but at cliff edge
            if (cliffDetectionZone.detectedColliders.Count == 0)
            {
                moveDirection = 0;
                yVelocity = 0;
            }
            // Moves enemy away from player if they just attacked and are still running away
            else if (RunAwayTracking)
            {
                moveDirection = -1 * (player.transform.position.x > selfCollider.transform.position.x ? 1 : -1);
                lookDirection = moveDirection;
            }
            // Stops enemy from moving if they are within a small proximity of the player
            else if (TrackingButNotMove && touchingDirections.IsGrounded)
            {
                moveDirection = 0;
                yVelocity = 0;
            }
            // Prevents running into a wall repeteably while tracking player
            else if (touchingDirections.IsOnWall)
            {
                moveDirection = 0;
                yVelocity = 0;
            }
            else
            {
                // Decides what direction to move when tracking towards to player
                lookDirection = player.transform.position.x > transform.position.x ? 1 : -1;
                moveDirection = lookDirection;
            }
        }

        // Non tracking movement decisions
        else if (cooldownHandler.timerStatusDict["attackLockTime"] == 0)
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
        if (!CurrentlyTrackingPlayer && touchingDirections.IsGrounded && cooldownHandler.timerStatusDict["cliffDetectionInterval"] == 0 && cliffDetectionZone.detectedColliders.Count == 0)
        {
            lookDirection = -1 * lookDirection;
            moveDirection = lookDirection;
            transform.localScale *= new Vector2(-1, 1);
            cooldownHandler.timerStatusDict["cliffDetectionInterval"] = 1;
        }
        else if (CurrentlyTrackingPlayer)
        {
            TrackingOffCliff = true;
        }
    }

    public void CalcDistanceToPlayer()
    {
        // Calculate the distance to the player with vector math
        Vector2 offset = transform.position - player.transform.position;
        distanceToPlayer = offset.magnitude;

        // Once in a set x distance of the player, continue tracking them but stand still
        if (Math.Abs(transform.position.x - player.transform.position.x) <= controller.trackButNotMoveProximity)
        {
            TrackingButNotMove = true;
        }
        else
        {
            TrackingButNotMove = false;
        }
    }

    public void FindOptimalMovePath() { }

    public void ExecuteEnemyAttack()
    {
        if (cooldownHandler.timerStatusDict["attackCooldown"] == 0 && CanAttack)
        {
            // Start cooldowns, tell animator an attack has occured and lock movement temporarily
            cooldownHandler.timerStatusDict["attackCooldown"] = 1;
            cooldownHandler.timerStatusDict["attackLockTime"] = 1;
            animator.SetTrigger("attacked");
            CanMove = false;

            // Make goblin runaway immediately after attack
            cooldownHandler.timerStatusDict["runAwayTime"] = 1;
            RunAwayTracking = true;
        }
    }

    public void Deactivate()
    {
        this.enabled = false;
    }
}
