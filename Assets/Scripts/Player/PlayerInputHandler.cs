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

public class PlayerInputHandler : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    PlayerController controller;
    Rigidbody2D rigidbody;
    Animator animator;
    TouchingDirections touchingDirections;
    Collider2D selfCollider;
    PlayerInputHandler self;
    CooldownTimer cooldownHandler;

    // Controller Variables
    private int _maxSpeed;
    private float _currentSpeed;
    private float yVelocity;
    private int _jumpImpulse;
    private int _dashImpulse;
    private int currentDashDirection = 1;
    private int lookDirection = 1;
    private bool _IsInvulnerable;

    // Private cooldown variables to grab from Player Controller
    private float _jumpCooldown;
    private float _dodgeCooldown;
    private float _dashLockTime;
    private float _meleeAttackCooldown;
    private float _rangedAttackCooldown;
    private float _invulnerableOnHitTime;

    // Input Variables
    Vector2 moveInput;

    // States
    public bool IsMoving { get { return isMoving; } private set { isMoving = value; animator.SetBool("isMoving", value); } }
    [SerializeField]
    private bool isMoving = false;
    public bool IsDashing { get { return isDashing; } private set { isDashing = value; animator.SetBool("isDashing", value); } }
    [SerializeField]
    private bool isDashing = false;
    public bool CanMove { get { return canMove;  } private set { canMove = value; } }
    [SerializeField]
    private bool canMove = true;
    public bool CanAttack { get { return canAttack; } private set { canAttack = value; } }
    [SerializeField]
    private bool canAttack = true;

    void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        touchingDirections = GetComponent<TouchingDirections>();
        selfCollider = GetComponent<Collider2D>();
        self = GetComponent<PlayerInputHandler>();
        cooldownHandler = GetComponent<CooldownTimer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grabs variables from controller
        _maxSpeed = controller.maxSpeed;
        _jumpImpulse = controller.jumpImpulse;
        _dashImpulse = controller.dashImpulse;
        _currentSpeed = controller.currentSpeed;
        _dashLockTime = controller.dashLockTime;
        _jumpCooldown = controller.jumpCooldown;
        _dodgeCooldown = controller.dodgeCooldown;
        _meleeAttackCooldown = controller.meleeAttackCooldown;
        _rangedAttackCooldown = controller.rangedAttackCooldown;
        _IsInvulnerable = controller.IsInvulnerable;
        _invulnerableOnHitTime = controller.invulnerableOnHitTime;

        List<string> keyList = new List<string> {"jumpCooldown", "dodgeCooldown", "dashLockTime", "meleeAttackCooldown", "rangedAttackCooldown", "invulnerableOnHitTime"};
        List<float> lengthList = new List<float> { _jumpCooldown, _dodgeCooldown, _dashLockTime, _meleeAttackCooldown, _rangedAttackCooldown, _invulnerableOnHitTime};
        cooldownHandler.SetupTimers(keyList, lengthList, self);
    }

    // Fixed Update is called every set interval (about every 0.02 seconds)
    void FixedUpdate()
    {
        // Checks Collisions with Level
        touchingDirections.CheckCollisions();

        // Cooldown Timer logic
        cooldownHandler.CheckCooldowns();

        // Makes player look in correct direction
        LookingDirection();

        // --- Left + Right Movement
            // Only move in response to input if not dashing and you can move
        if (IsDashing && CanMove)
        {
            _currentSpeed = _dashImpulse * currentDashDirection;
            yVelocity = 0;
        }
            // Regular movement based input
        if (!IsDashing && CanMove)
        {
            _currentSpeed = controller.currentSpeed * moveInput.x;
            yVelocity = rigidbody.linearVelocityY;
        }
            // Stops horizontal movement if on wall
        if (touchingDirections.IsOnWall)
        {
            _currentSpeed = 0;
        }

        // Updates the velocity of the player while limiting it to the player's max speed
        if (_currentSpeed <= _maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(_currentSpeed, yVelocity);
        }
        if (_currentSpeed > _maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(_maxSpeed, yVelocity);
        }

        // Updates the animator's yVelocity value for air state animations
        animator.SetFloat("yVelocity", rigidbody.linearVelocityY);
    }
    
    // Allows specific processes to be coded in to happen once a cooldown ends
    public void CooldownEndProcess(string key) {
        if (key == "dashLockTime")
        {
            IsDashing = false;
            controller.IsInvulnerable = false;
        }
        if (key == "invulnerableOnHitTime")
        {
            controller.IsInvulnerable = false;
        }
    }

    public void LookingDirection()
    {
        if (moveInput.x >= 0 && lookDirection != 1)
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

    public void MovePlayer(InputAction.CallbackContext context) {
        // Changes context to the readable move input
        moveInput = context.ReadValue<Vector2>();

        // Sets IsMoving to true or false dependent on input being zero or not
        IsMoving = moveInput != Vector2.zero;
    }

    public void ExecutePlayerJump(InputAction.CallbackContext context) {
        // Checks if player is grounded before jumping and the jump is available
        //if (IsGrounded && timerStatusList[0] == 0 && CanMove)
        if (touchingDirections.IsGrounded && cooldownHandler.timerStatusDict["jumpCooldown"] == 0 && CanMove)
        {
            rigidbody.linearVelocityY = _jumpImpulse;

            // Sets the status of the jump timer to 1, flagging that it should start ticking
            cooldownHandler.timerStatusDict["jumpCooldown"] = 1;
            //timerStatusList[0] = 1;

            // Tells the animator that the player jumped
            animator.SetTrigger("jumped");
        }
    }

    public void ExecutePlayerDodge(InputAction.CallbackContext context) {
        // Checks if the player's dash is available
        //if (timerStatusList[1] == 0 && CanMove)
        if (cooldownHandler.timerStatusDict["dodgeCooldown"] == 0 && CanMove)
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
                cooldownHandler.timerStatusDict["dashCooldown"] = 1;
                //timerStatusList[1] = 1;
                // Sets status of dash lock timer to 1, so player can't input movement
                cooldownHandler.timerStatusDict["dashLockTime"] = 1;
                //timerStatusList[2] = 1;
                IsDashing = true;
                _IsInvulnerable = true;
            }
        }
    }

    public void ExecutePlayerMeleeAttack(InputAction.CallbackContext context) {
        if (cooldownHandler.timerStatusDict["meleeAttackCooldown"] == 0 && CanAttack)
        {
            cooldownHandler.timerStatusDict["meleeAttackCooldown"] = 1;
            animator.SetTrigger("meleeAttacked");
        }
    }

    public void ExecutePlayerRangedAttack(InputAction.CallbackContext context)
    {
        if (cooldownHandler.timerStatusDict["rangedAttackCooldown"] == 0 && CanAttack)
        {
            cooldownHandler.timerStatusDict["rangedAttackCooldown"] = 1;
            animator.SetTrigger("rangedAttacked");
        }
    }

    public void ExecutePlayerAbility() { }
}
