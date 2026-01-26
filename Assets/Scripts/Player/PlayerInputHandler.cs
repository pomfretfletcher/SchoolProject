using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, LogicScript
{
    // Script + Component Links
    PlayerController controller;
    Rigidbody2D rigidbody;
    Animator animator;
    TouchingDirections touchingDirections;
    Collider2D selfCollider;
    CooldownTimer cooldownHandler;
    ProjectileLauncher projectileLauncher;

    // Internal Logic Variables
    private int currentDashDirection = 1;
    private int lookDirection = 1;
    private int attackCombo = 0;
    private float xVelocity;
    private float yVelocity;
    private Vector2 moveInput;

    // Internal/External Logic Variables
    public bool invulnerableFromAnotherSource = false;

    // States
    public bool IsMoving { get { return isMoving; } set { isMoving = value; animator.SetBool("isMoving", value); } }
    public bool IsDashing { get { return isDashing; } set { isDashing = value; animator.SetBool("isDashing", value); } }
    [Header("States")]
    [SerializeField]
    private bool isMoving = false;
    [SerializeField]
    private bool isDashing = false;
    public bool CanMove = true;
    public bool CanAttack = true;
    public bool CanUseAbilities = true;
    public bool isMeleeAttacking = false;
    public bool isRangedAttacking = false;
    
    private void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        touchingDirections = GetComponent<TouchingDirections>();
        selfCollider = GetComponent<Collider2D>();
        cooldownHandler = GetComponent<CooldownTimer>();
        projectileLauncher = GetComponent<ProjectileLauncher>();
    }

    private void FixedUpdate()
    {
        // Checks Collisions with Level
        touchingDirections.CheckCollisions();

        // Makes player look in correct direction
        LookingDirection();

        // Left + Right Movement
            // Only move in response to input if not dashing and you can move
        if (IsDashing && CanMove)
        {
            xVelocity = controller.dashImpulse * currentDashDirection;
            yVelocity = 0;
        }
            // Regular movement based input
        if (!IsDashing && CanMove)
        {
            xVelocity = controller.currentSpeed * moveInput.x;
            yVelocity = rigidbody.linearVelocityY;
        }
            // Stops horizontal movement if on wall
        if (touchingDirections.IsOnWall)
        {
            xVelocity = 0;
        }

        if (!CanMove)
        {
            xVelocity = 0;
            yVelocity = rigidbody.linearVelocityY;
        }

        // Updates the velocity of the player while limiting it to the player's max speed
        if (xVelocity <= controller.maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(xVelocity, yVelocity);
        }
        if (xVelocity > controller.maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(controller.maxSpeed, yVelocity);
        }

        // Updates the animator's yVelocity value for air state animations
        animator.SetFloat("yVelocity", rigidbody.linearVelocityY);
    }

    public void LookingDirection()
    {
        if (moveInput.x > 0 && lookDirection != 1)
        {
            // Changes look direction to 1 (right)
            lookDirection = 1;
            // Flips transform
            transform.localScale *= new Vector2(-1, 1);
        }
        if (moveInput.x < 0 && lookDirection != -1)
        {
            // Changes look direction to -1 (left)
            lookDirection = -1;
            // Flips transform
            transform.localScale *= new Vector2(-1, 1);
        }
    }

    public void MovePlayer(InputAction.CallbackContext context) 
    {
        // Changes context to the readable move input
        moveInput = context.ReadValue<Vector2>();

        // Sets IsMoving to true or false dependent on input being zero or not
        IsMoving = moveInput.x != 0;
    }

    public void ExecutePlayerJump(InputAction.CallbackContext context) 
    {
        // Checks if player is grounded before jumping and the jump is available
        if (touchingDirections.IsGrounded && CanMove)
        {
            rigidbody.linearVelocityY = controller.jumpImpulse;

            // Sets the status of the jump timer to 1, flagging that it should start ticking
            cooldownHandler.timerStatusDict["jumpCooldown"] = 1;

            // Tells the animator that the player jumped
            animator.SetTrigger("jumped");
        }
    }

    public void ExecutePlayerDodge(InputAction.CallbackContext context) 
    {
        // Checks if the player's dash is available
        if (cooldownHandler.timerStatusDict["dodgeCooldown"] == 0 && CanMove && !IsDashing)
        {
            // Currently going right, dash right
            if (rigidbody.linearVelocityX > 0)
            {
                currentDashDirection = 1;
            }
            // Currently going left, dash left
            if (rigidbody.linearVelocityX < 0)
            {
                currentDashDirection = -1;
            }
            // Only use dash if not stood still
            if (rigidbody.linearVelocityX != 0)
            {
                // Sets the status of the dash timer to 1, flagging that it should start ticking
                cooldownHandler.timerStatusDict["dodgeCooldown"] = 1;
                // Sets status of dash lock timer to 1, so player can't input movement
                cooldownHandler.timerStatusDict["dashLockTime"] = 1;
                IsDashing = true;
                controller.IsInvulnerable = true;
            }
        }
    }

    public void ExecutePlayerMeleeAttack(InputAction.CallbackContext context) 
    {
        // Tell animator to start melee attack animation and starts melee attack cooldown
        if (cooldownHandler.timerStatusDict["meleeAttackCooldown"] == 0 && CanAttack && cooldownHandler.timerStatusDict["comboTime"] == 1 && !isRangedAttacking)
        {
            if (attackCombo == 1)
            {
                // Starts timer
                cooldownHandler.timerStatusDict["meleeAttackCooldown"] = 1;
                cooldownHandler.timerStatusDict["isMeleeAttacking"] = 1;
                // Resets combotime
                cooldownHandler.timerDict["comboTime"] = 0;
                animator.SetTrigger("comboAttack1");
                attackCombo = 2;
                isMeleeAttacking = true;
            }
            else if (attackCombo == 2)
            {
                // Starts timer
                cooldownHandler.timerStatusDict["meleeAttackCooldown"] = 1;
                cooldownHandler.timerStatusDict["isMeleeAttacking"] = 1;
                // Resets combotime
                cooldownHandler.timerDict["comboTime"] = 0;
                animator.SetTrigger("comboAttack2");
                attackCombo = 3;
                isMeleeAttacking = true;
            }
            else
            {
                // Finish combo, reset timer and turn it off
                cooldownHandler.timerDict["comboTime"] = 0;
                cooldownHandler.timerStatusDict["comboTime"] = 0;
                attackCombo = 0;
            }
        }

        // Regular melee attack
        if (cooldownHandler.timerStatusDict["meleeAttackCooldown"] == 0 && CanAttack && cooldownHandler.timerStatusDict["comboTime"] == 0 && !isRangedAttacking && touchingDirections.IsGrounded)
        {
            cooldownHandler.timerStatusDict["meleeAttackCooldown"] = 1;
            cooldownHandler.timerStatusDict["isMeleeAttacking"] = 1;
            cooldownHandler.timerStatusDict["comboTime"] = 1;
            animator.SetTrigger("meleeAttacked");
            attackCombo = 1;
            isMeleeAttacking = true;
        }

        // Inair melee attack
        if (cooldownHandler.timerStatusDict["meleeAttackCooldown"] == 0 && CanAttack && !isRangedAttacking && !touchingDirections.IsGrounded)
        {
            cooldownHandler.timerStatusDict["meleeAttackCooldown"] = 1;
            cooldownHandler.timerStatusDict["isMeleeAttacking"] = 1;
            attackCombo = 0;
            animator.SetTrigger("inAirMeleeAttacked");
            isMeleeAttacking = true;
        }
    }

    public void ExecutePlayerRangedAttack(InputAction.CallbackContext context)
    {
        // Tell animator to start ranged attack animation and starts ranged attack cooldown
        if (cooldownHandler.timerStatusDict["rangedAttackCooldown"] == 0 && CanAttack && !isMeleeAttacking && touchingDirections.IsGrounded)
        {
            cooldownHandler.timerStatusDict["rangedAttackCooldown"] = 1;
            cooldownHandler.timerStatusDict["isRangedAttacking"] = 1;
            animator.SetTrigger("rangedAttacked");
            cooldownHandler.timerStatusDict["projectileFireDelay"] = 1;
            CanMove = false;
            isRangedAttacking = true;
        }
    }

    public void Deactivate()
    {
        this.enabled = false;
    }
}