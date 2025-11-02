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

public class PlayerInputHandler : MonoBehaviour
{
    // Player Controller and Rigidbody Links
    public PlayerController controller;
    Rigidbody2D rigidbody;
    Animator animator;

    // Speeds
    private int _maxSpeed;
    private float _currentSpeed;
    private float yVelocity;

    // Private cooldown variables to grab from Player Controller
    private int _jumpCooldown;
    private int _dodgeCooldown;
    private float _dashLockTime;
    private int _meleeAttackCooldown;
    private int _rangedAttackCooldown;
    
    // Dictionaries storing information necessary for cooldown functions. Each cooldown has a shared key among these three dicts, with the cooldown length,
    // current time progressed and timer status stored in their respective dicts.
    Dictionary<string, float> cooldownDict = new Dictionary<string, float>();
    Dictionary<string, float> timerDict = new Dictionary<string, float>();
    Dictionary<string, int> timerStatusDict = new Dictionary<string, int>();

    // Additional variables used for handling cooldown timers
        // Used for knowing how many cooldowns to loop through
    private int cooldownCount;
        // Stores the keys of the dictionaries, in order to be able to loop through
    public List<string> cooldownKeys;
        // Stores the current key being used to access dictionaries
    private string currentKey;

    // Input Variables
    Vector2 moveInput;

    // -------
    private int _jumpImpulse;
    private int _dashImpulse;
    private int currentDashDirection = 1;

    // Touching direction variables
    public ContactFilter2D castFilter;
    public CapsuleCollider2D selfCollider;
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float ceilingDistance = 0.05f;
    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    // Used to decide which wall direction to check
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    // ----
    private bool _IsInvulnerable;
    private int lookDirection = 1;

    // States for animator component. Sets in script value and value inside the animator
        // Touching Direction states
    public bool IsGrounded { get { return isGrounded; } private set { isGrounded = value; animator.SetBool("isGrounded", value); } }
    [SerializeField]
    private bool isGrounded = false;
    public bool IsOnWall { get { return isOnWall; } private set { isOnWall = value; animator.SetBool("isOnWall", value);  } }
    [SerializeField]
    private bool isOnWall = false;
    public bool IsOnCeiling { get { return isOnCeiling; } private set { isOnCeiling = value; animator.SetBool("isOnCeiling", value);  } }
    [SerializeField]
    private bool isOnCeiling = false;

        // Movement states
    public bool IsMoving { get { return isMoving; } private set { isMoving = value; animator.SetBool("isMoving", value); } }
    [SerializeField]
    private bool isMoving = false;
    public bool IsDashing { get { return isDashing; } private set { isDashing = value; animator.SetBool("isDashing", value); } }
    [SerializeField]
    private bool isDashing = false;
    public bool CanMove { get; private set; }
    public bool CanAttack { get; private set; }

    // Awake is called once the MonoBehaviour is created
    void Awake()
    {
       rigidbody = GetComponent<Rigidbody2D>();
       selfCollider = GetComponent<CapsuleCollider2D>();
       animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grabs variables from controller
            // Movement Variables
        _maxSpeed = controller.maxSpeed;
        _jumpImpulse = controller.jumpImpulse;
        _dashImpulse = controller.dashImpulse;
        _currentSpeed = controller.currentSpeed;
        _dashLockTime = controller.dashLockTime;
            // Cooldown Variables
        _jumpCooldown = controller.jumpCooldown;
        _dodgeCooldown = controller.dodgeCooldown;
        _meleeAttackCooldown = controller.meleeAttackCooldown;
        _rangedAttackCooldown = controller.rangedAttackCooldown;
            // Misc Variables
        _IsInvulnerable = controller.IsInvulnerable;

        // Add cooldown variables to list and creates corresponding timer
        cooldownDict.Add("jumpCooldown", _jumpCooldown); timerDict.Add("jumpCooldown", 0); timerStatusDict.Add("jumpCooldown", 0); cooldownKeys.Add("jumpCooldown"); cooldownCount += 1;
        cooldownDict.Add("dodgeCooldown", _dodgeCooldown); timerDict.Add("dodgeCooldown", 0); timerStatusDict.Add("dodgeCooldown", 0); cooldownKeys.Add("dodgeCooldown"); cooldownCount += 1;
        cooldownDict.Add("dashLockTime", _dashLockTime); timerDict.Add("dashLockTime", 0); timerStatusDict.Add("dashLockTime", 0); cooldownKeys.Add("dashLockTime"); cooldownCount += 1;
        cooldownDict.Add("meleeAttackCooldown", _meleeAttackCooldown); timerDict.Add("meleeAttackCooldown", 0); timerStatusDict.Add("meleeAttackCooldown", 0); cooldownKeys.Add("meleeAttackCooldown"); cooldownCount += 1;
        cooldownDict.Add("rangedAttackCooldown", _rangedAttackCooldown); timerDict.Add("rangedAttackCooldown", 0); timerStatusDict.Add("rangedAttackCooldown", 0); cooldownKeys.Add("rangedAttackCooldown"); cooldownCount += 1;

        // Set bools to default state
        IsMoving = false;
        IsDashing = false;
        CanMove = true;
        CanAttack = true;
    }

    // Fixed Update is called every set interval (about every 0.02 seconds)
    void FixedUpdate()
    {
        // Checks contacted directions
        // Checks if on ground. If number of ground hits > 0, we are on ground
        IsGrounded = selfCollider.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnWall = selfCollider.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
        IsOnCeiling = selfCollider.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
        // Edge cases
        if (IsOnCeiling) { IsGrounded = false; }

        // Cooldown Timer logic
        checkCooldowns();

        // Makes player look in correct direction
        lookingDirection();

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
        if (IsOnWall)
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

    public void checkCooldowns()
    {
        // Checks each timer, if they are currently ticking up and running (thus greater than zero), checks if the timer has been running longer than its
        // cooldown. If so, it sets the status of that cooldown to 0, so it will no longer tickup as well as reseting the timer value itself.
        for (var i = 0; i < cooldownCount; i++)
        {
            // Grabs the name of the currently checked cooldown from the cooldowns string list
            currentKey = cooldownKeys[i];
            // Grabs the currently  being checked cooldown, timer and status variables
            float cooldown = cooldownDict[currentKey];
            float duration = timerDict[currentKey];
            int status = timerStatusDict[currentKey];
            // If the timer is active, tick it up by the time since last frame
            if (status == 1)
            {
                timerDict[currentKey] = duration + Time.deltaTime;
            }
            // If the timer has reached past the relevant cooldown, the timer is reset, the active status is set to 0 (false). This allows the relevant ability/action to be used again
            if (duration >= cooldown)
            {
                timerDict[currentKey] = 0;
                timerStatusDict[currentKey] = 0;
                // Some cooldowns ending have processes to complete, the following function completes these
                cooldownEndProcess();
            }
        }
    }

    // Allows specific processes to be coded in to happen once a cooldown ends
    public void cooldownEndProcess() {
        // Dash Lock Time
        if (currentKey == "dashLockTime")
        {
            IsDashing = false;
            _IsInvulnerable = false;
        }
    }

    public void lookingDirection()
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
        if (IsGrounded && timerStatusDict["jumpCooldown"] == 0 && CanMove)
        {
            rigidbody.linearVelocityY = _jumpImpulse;

            // Sets the status of the jump timer to 1, flagging that it should start ticking
            timerStatusDict["jumpCooldown"] = 1;
            //timerStatusList[0] = 1;

            // Tells the animator that the player jumped
            animator.SetTrigger("jumped");
        }
    }

    public void ExecutePlayerDodge(InputAction.CallbackContext context) {
        // Checks if the player's dash is available
        //if (timerStatusList[1] == 0 && CanMove)
        if (timerStatusDict["dodgeCooldown"] == 0 && CanMove)
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
                timerStatusDict["dashCooldown"] = 1;
                //timerStatusList[1] = 1;
                // Sets status of dash lock timer to 1, so player can't input movement
                timerStatusDict["dashLockTime"] = 1;
                //timerStatusList[2] = 1;
                IsDashing = true;
                _IsInvulnerable = true;
            }
        }
    }

    public void ExecutePlayerMeleeAttack(InputAction.CallbackContext context) {
        //if (timerStatusList[3] == 0 && CanAttack)
        if (timerStatusDict["meleeAttackCooldown"] == 0 && CanAttack)
        {
            //timerStatusList[3] = 1;
            timerStatusDict["meleeAttackCooldown"] = 1;
            animator.SetTrigger("meleeAttacked");
        }
    }

    public void ExecutePlayerRangedAttack(InputAction.CallbackContext context)
    {
        //if (timerStatusList[3] == 0 && CanAttack)
        if (timerStatusDict["rangedAttackCooldown"] == 0 && CanAttack)
        {
            //timerStatusList[3] = 1;
            timerStatusDict["rangedAttackCooldown"] = 1;
            animator.SetTrigger("rangedAttacked");
        }
    }

    public void ExecutePlayerAbility() { }
}
