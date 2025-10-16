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

    // Cooldowns
    private int _jumpCooldown;
    private int _dodgeCooldown;
    private float _dashLockTime;
    private int _meleeAttackCooldown;
    private int _rangedAttackCooldown;
    public List<float> cooldownList;

    // Ongoing Timers
    private float timeSinceJump;
    private float timeSinceDodge;
    private float timeSinceMeleeAttack;
    private float timeSinceRangedAttack;
    public List<float> timerList;
    public List<int> timerStatusList;

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
    public float wallDistance = 0.05f;
    public float ceilingDistance = 0.05f;
    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];

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
    public bool IsDashing { get; private set; }
    public bool CanMove { get; private set; }

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
        _maxSpeed = controller.maxSpeed;
        _jumpImpulse = controller.jumpImpulse;
        _dashImpulse = controller.dashImpulse;
        _currentSpeed = controller.currentSpeed;
        _jumpCooldown = controller.jumpCooldown;
        _dodgeCooldown = controller.dodgeCooldown;
        _dashLockTime = controller.dashLockTime;
        _meleeAttackCooldown = controller.meleeAttackCooldown;
        _rangedAttackCooldown = controller.rangedAttackCooldown;
        _IsInvulnerable = controller.IsInvulnerable;

        // Add cooldown variables to list and creates corresponding timer
        cooldownList.Add(_jumpCooldown); timerList.Add(0); timerStatusList.Add(0);
        cooldownList.Add(_dodgeCooldown); timerList.Add(0); timerStatusList.Add(0);
        cooldownList.Add(_dashLockTime); timerList.Add(0); timerStatusList.Add(0);
        cooldownList.Add(_meleeAttackCooldown); timerList.Add(0); timerStatusList.Add(0);
        cooldownList.Add(_rangedAttackCooldown); timerList.Add(0); timerStatusList.Add(0);

        // Set bools to default state
        IsMoving = false;
        IsGrounded = false;
        IsDashing = false;
        CanMove = true;
    }

    // Fixed Update is called every set interval (about every 0.2 seconds)
    void FixedUpdate()
    {
       // Checks contacted directions
        // Checks if on ground. If number of ground hits > 0, we are on ground
        IsGrounded = selfCollider.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        int hitCount = selfCollider.Cast(new Vector2(lookDirection, 1), castFilter, wallHits, wallDistance);
        //IsOnWall = false;
        //for (int i = 0; i < hitCount; i++)
        //{
        //    Vector2 normal = wallHits[i].normal;

        //    // Check if the normal is mostly horizontal (i.e. wall)
        //    if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
        //    {
        //        IsOnWall = true;
        //        break;
        //    }
        //}
        IsOnCeiling = selfCollider.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;

        // Cooldown and Timers
        checkCooldowns();

        // Makes player look in correct direction
        lookingDirection();

        // --- Left + Right Movement
        // Only move in response to input if not dashing and you can move
        if (IsDashing && CanMove)
        {
            _currentSpeed = _dashImpulse * currentDashDirection;
            //if (IsOnWall) { _currentSpeed = 0; }
            // Updates the velocity of the player while limiting it to the player's max speed
            if (_currentSpeed <= _maxSpeed)
            {
                rigidbody.linearVelocity = new Vector2(_currentSpeed, 0);
            }
            if (_currentSpeed > _maxSpeed)
            {
                rigidbody.linearVelocity = new Vector2(_maxSpeed, 0);
            }
        }
        // Regular movement based input
        if (!IsDashing && CanMove)
        {
            _currentSpeed = controller.currentSpeed;
            //if (IsOnWall) { _currentSpeed = 0; }
            // Updates the velocity of the player while limiting it to the player's max speed
            if (_currentSpeed <= _maxSpeed)
            {
                rigidbody.linearVelocity = new Vector2(moveInput.x * _currentSpeed, rigidbody.linearVelocityY);
            }
            if (_currentSpeed > _maxSpeed)
            {
                rigidbody.linearVelocity = new Vector2(moveInput.x * _maxSpeed, rigidbody.linearVelocityY);
            }
        }

        // Updates the animator's yVelocity value for air state animations
        animator.SetFloat("yVelocity", rigidbody.linearVelocityY);
    }

    public void checkCooldowns() { 
        // Checks each timer, if they are currently ticking up and running (thus greater than zero), checks if the timer has been running longer than its
        // cooldown. If so, it sets the status of that cooldown to 0, so it will no longer tickup as well as reseting the timer value itself.
        for (var i = 0; i< timerList.Count; i++)
        {
            // Grabs the currently  being checked cooldown, timer and status variables
            float cooldown = cooldownList[i];
            float duration = timerList[i];
            int status = timerStatusList[i];
            // If the timer is active, tick it up by the time since last frame
            if (status == 1)
            {
                timerList[i] = duration + Time.deltaTime;
            }
            // If the timer has reached past the relevant cooldown, the timer is reset, the active status is set to 0 (false). This allows the relevant ability/action to be used again
            if (duration + Time.deltaTime >= cooldown)
            {
                timerList[i] = 0;
                timerStatusList[i] = 0;
                // Some cooldowns ending have processes to complete, the following function completes these
                cooldownEndProcess(i);
            }
        }
    }

    public void cooldownEndProcess(int i) {
        // Dash Lock Time
        if (i == 2)
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
        if (IsGrounded && timerStatusList[0] == 0 && CanMove)
        {
            rigidbody.linearVelocityY = _jumpImpulse;

            // Sets the status of the jump timer to 1, flagging that it should start ticking
            timerStatusList[0] = 1;

            // Tells the animator that the player jumped
            animator.SetTrigger("jumped");
        }
    }

    public void ExecutePlayerDodge(InputAction.CallbackContext context) {
        // Checks if the player's dash is available
        if (timerStatusList[1] == 0 && CanMove)
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
                timerStatusList[1] = 1;
                // Sets status of dash lock timer to 1, so player can't input movement
                timerStatusList[2] = 1;
                IsDashing = true;
                _IsInvulnerable = true;
            }
        }
    }
    public void ExecutePlayerAttack() { }
    public void ExecutePlayerAbility() { }
}
