using System.Collections.Generic;
using UnityEngine;
using System;

public class MushroomPathfinding : MonoBehaviour, LogicScript
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
    UniversalEnemyFunctions universalEnemyFunctions;

    // Internal Logic Variables
    private float distanceToPlayer;
    private float yVelocity;
    
    // States
    [Header("Movement States")]
    [SerializeField] private bool isMoving = false; 
    public bool IsMoving { get { return isMoving; } set { isMoving = value; animator.SetBool("isMoving", value); } }
    [SerializeField] private bool canMove = true; 
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    [SerializeField] private bool isSufferingKnockback = false;
    public bool IsSufferingKnockback { get { return isSufferingKnockback; } set { isSufferingKnockback = value; } }

    [Header("Directions")]
    [SerializeField] private int lookDirection = 1;
    public int LookDirection { get { return lookDirection; } set { lookDirection = value; } }
    [SerializeField] private int moveDirection = 1;
    public int MoveDirection { get { return moveDirection; } set { moveDirection = value; } }
    
    [Header("Combat States")]
    [SerializeField] private bool canAttack = true;
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    
    public bool isAttacking = false;

    [Header("Tracking States")]
    public bool CurrentlyTrackingPlayer = false;
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
        universalEnemyFunctions = GetComponent<UniversalEnemyFunctions>();
    }

    private void FixedUpdate()
    {
        // Checks envionmental collisions
        touchingDirections.CheckCollisions();

        // Makes player look in correct direction
        universalEnemyFunctions.LookingDirection();

        if (controller.currentHealth > 0)
        {
            // Choose how to make enemy move
            DecidePathfinding();

            yVelocity = ProcessPathfinding();

            // Apply decided upon velocity
            universalEnemyFunctions.ApplyVelocity(yVelocity);

            // Update ismoving variable and animator parameter based on current movement
            universalEnemyFunctions.UpdateIsMoving();
        }
        else
        {
            rigidbody.linearVelocityX = 0;
        }
    }

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
    }

    public void DecidePathfinding()
    {
        // Calculate the distance to the player with vector math
        Vector2 offset = transform.position - player.transform.position;
        distanceToPlayer = offset.magnitude;

        // If the player is within the enemy's tracking proximity
        if (distanceToPlayer <= controller.playerRequiredProximity)
        {
            // If not curently tracking player, the enemy is now tracking the player
            CurrentlyTrackingPlayer = true;
        }

        // Once in a set x distance of the player, continue tracking them but stand still
        if (Math.Abs(transform.position.x - player.transform.position.x) <= controller.runAwayTrackingProximity && RunAwayTracking == false)
        {
            RunAwayTracking = true;
            cooldownHandler.timerStatusDict["runAwayTime"] = 1;
        }
        else if (cooldownHandler.timerStatusDict["runAwayTime"] == 0)
        {
            RunAwayTracking = false;
        }

        // If player leaves proximity, the enemy stops tracking them
        if (distanceToPlayer > controller.playerRequiredProximity)
        {
            CurrentlyTrackingPlayer = false;
            RunAwayTracking = false;
        }
    }

    public float ProcessPathfinding() 
    {
        if (cooldownHandler.timerStatusDict["attackLockTime"] == 1) { moveDirection = 0; return rigidbody.linearVelocityY; }

        if (!CanMove) { moveDirection = 0; return rigidbody.linearVelocityY; }

        // Tracking player movement decisions
        if (CurrentlyTrackingPlayer && touchingDirections.IsGrounded)
        {
            // Prevents running into a wall repeteably while tracking player
            if (RunAwayTracking && (touchingDirections.IsOnWall || touchingDirections.WallStop))
            {
                moveDirection = 0;
                yVelocity = 0;
            }
            else if (RunAwayTracking)
            {
                moveDirection = -1 * (player.transform.position.x > selfCollider.transform.position.x ? 1 : -1);
                lookDirection = moveDirection;
            }
            else
            {
                moveDirection = 0;
                lookDirection = player.transform.position.x > selfCollider.transform.position.x ? 1 : -1;
            }

            if (!RunAwayTracking)
            {
                ExecuteEnemyAttack();
            }
        }

        // Non tracking movement decisions
        else
        {
            // Flip direction at collision with wall
            if (touchingDirections.IsGrounded && (touchingDirections.IsOnWall || touchingDirections.WallStop) && cooldownHandler.timerStatusDict["wallDetectionInterval"] == 0)
            {
                moveDirection = -1 * lookDirection;
                yVelocity = 0;
                cooldownHandler.timerStatusDict["wallDetectionInterval"] = 1;
            }
            // Default walk cycle movement
            else if (touchingDirections.IsGrounded)
            {
                moveDirection = lookDirection;
                yVelocity = 0;
            }
            // Not move if in air, allow gravity to affect
            else if (!touchingDirections.IsGrounded)
            {
                moveDirection = 0;
                yVelocity = rigidbody.linearVelocityY;
            }
        }

        return yVelocity;
    }

    public void ExecuteEnemyAttack()
    {
        if (cooldownHandler.timerStatusDict["attackCooldown"] == 0 && CanAttack && controller.CurrentHealth > 0 
            && player.transform.position.y > transform.position.y
            && ((LookDirection == -1 && player.transform.position.x < transform.position.x)
               || LookDirection == 1 && player.transform.position.x > transform.position.x))
        {
            cooldownHandler.timerStatusDict["attackCooldown"] = 1;
            cooldownHandler.timerStatusDict["attackLockTime"] = 1;
            cooldownHandler.timerStatusDict["projectileFireDelay"] = 1;
            animator.SetTrigger("attacked");
            CanMove = false;
            isAttacking = true;
        }
    }
}