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

    // Ongoing Timers
    private float timeSinceJump;
    private float timeSinceDodge;
    private float timeSinceMeleeAttack;
    private float timeSinceRangedAttack;

    // Input Variables
    Vector2 moveInput;

    // States for animator component
    public bool IsMoving { get; private set; }

    // Awake is called once the MonoBehaviour is created
    void Awake()
    {
       rigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grabs variables from controller
        _maxSpeed = controller.maxSpeed;
        _currentSpeed = controller.currentSpeed;
        _jumpCooldown = controller.jumpCooldown;
        _dodgeCooldown = controller.dodgeCooldown;
        _meleeAttackCooldown = controller.meleeAttackCooldown;
        _rangedAttackCooldown = controller.rangedAttackCooldown;
    }

    // Fixed Update is called every set interval (about every 0.2 seconds)
    void FixedUpdate()
    {
        // Updates any variables that need updating
        _currentSpeed = controller.currentSpeed;

        // Updates the velocity of the player while limiting it to the player's max speed
        if (_currentSpeed <= _maxSpeed)
            rigidbody.linearVelocity = new Vector2(moveInput.x * _currentSpeed, rigidbody.linearVelocityY);

        else
            rigidbody.linearVelocity = new Vector2(moveInput.x * _maxSpeed, rigidbody.linearVelocityY);
    }

    public void MovePlayer(InputAction.CallbackContext context) {
        // Changes context to the readable move input
        moveInput = context.ReadValue<Vector2>();

        // Sets IsMoving to true or false dependent on input being zero or not
        IsMoving = moveInput != Vector2.zero;
    }

    public void ExecutePlayerJump() { }
    public void ExecutePlayerDodge() { }
    public void ExecutePlayerAttack() { }
    public void ExecutePlayerAbility() { }
}
