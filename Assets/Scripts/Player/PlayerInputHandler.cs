using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // Player Controller and Rigidbody Links
    public PlayerController controller;
    Rigidbody2D rigidbody;

    // Speeds
    private int _maxSpeed;
    private float _currentSpeed;

    // Cooldowns
    private int _jumpCooldown;
    private int _dodgeCooldown;
    private int _meleeAttackCooldown;
    private int _rangedAttackCooldown;
    public List<int> cooldownList;

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

    // Touching direction variables
    public ContactFilter2D castFilter;
    public CapsuleCollider2D selfCollider;
    public float groundDistance = 0.05f;
    RaycastHit2D[] groundHits = new RaycastHit2D[5];


    // States for animator component
    public bool IsMoving { get; private set; }
    public bool IsGrounded { get; private set; }

    // Awake is called once the MonoBehaviour is created
    void Awake()
    {
       rigidbody = GetComponent<Rigidbody2D>();
       selfCollider = GetComponent<CapsuleCollider2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grabs variables from controller
        _maxSpeed = controller.maxSpeed;
        _jumpImpulse = controller.jumpImpulse;
        _currentSpeed = controller.currentSpeed;
        _jumpCooldown = controller.jumpCooldown;
        _dodgeCooldown = controller.dodgeCooldown;
        _meleeAttackCooldown = controller.meleeAttackCooldown;
        _rangedAttackCooldown = controller.rangedAttackCooldown;

        // Add cooldown variables to list and creates corresponding timer
        cooldownList.Add(_jumpCooldown); timerList.Add(0); timerStatusList.Add(0);
        cooldownList.Add(_dodgeCooldown); timerList.Add(0); timerStatusList.Add(0);
        cooldownList.Add(_meleeAttackCooldown); timerList.Add(0); timerStatusList.Add(0);
        cooldownList.Add(_rangedAttackCooldown); timerList.Add(0); timerStatusList.Add(0);
    }

    // Fixed Update is called every set interval (about every 0.2 seconds)
    void FixedUpdate()
    {
        // Checks contacted directions
        // Checks if on ground. If number of ground hits > 0, we are on ground
        IsGrounded = selfCollider.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;

        // Cooldown and Timers
        checkCooldowns();

        // --- Left + Right Movement
        // Updates any variables that need updating
        _currentSpeed = controller.currentSpeed;

        // Updates the velocity of the player while limiting it to the player's max speed
        if (_currentSpeed <= _maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(moveInput.x * _currentSpeed, rigidbody.linearVelocityY);
        }
        else 
        {
            rigidbody.linearVelocity = new Vector2(moveInput.x * _maxSpeed, rigidbody.linearVelocityY);
        }
    }

    public void checkCooldowns() { 
        // Checks each timer, if they are currently ticking up and running (thus greater than zero), checks if the timer has been running longer than its
        // cooldown. If so, it sets the status of that cooldown to 0, so it will no longer tickup as well as reseting the timer value itself.
        for (var i = 0; i< timerList.Count; i++)
        {
            // Grabs the currently  being checked cooldown, timer and status variables
            int cooldown = cooldownList[i];
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
            }
        }
    }

    public void MovePlayer(InputAction.CallbackContext context) {
        // Changes context to the readable move input
        moveInput = context.ReadValue<Vector2>();

        // Sets IsMoving to true or false dependent on input being zero or not
        IsMoving = moveInput != Vector2.zero;
    }

    public void ExecutePlayerJump(InputAction.CallbackContext context) {
        // Checks if player is grounded before jumping
        if (IsGrounded && timerStatusList[0] == 0)
        {
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocityX, _jumpImpulse);

            // Sets the status of the jump timer to 1, flagging that it should start ticking
            timerStatusList[0] = 1;
        }
    }

    public void ExecutePlayerDodge() { }
    public void ExecutePlayerAttack() { }
    public void ExecutePlayerAbility() { }
}
