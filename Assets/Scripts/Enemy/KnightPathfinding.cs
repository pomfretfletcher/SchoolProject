using System.Collections.Generic;
using UnityEngine;

public class KnightPathfinding : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    EnemyController controller;
    Object player;
    Rigidbody2D rigidbody;
    Animator animator;
    TouchingDirections touchingDirections;
    Collider2D selfCollider;
    DetectionZone cliffDetectionZone;
    KnightPathfinding self;
    CooldownTimer cooldownHandler;

    // Waypoint Variables
    private Transform _nextWaypoint;
    private List<Transform> _waypointList;

    // Distance Variables
    public float distanceToPlayer;
    private int _playerRequiredProximity;

    // Cooldown Variables
    private float _attackCooldown;
    private int cliffDetectionInterval = 3;

    // Movement Variables
    private int _maxSpeed;
    private float _currentSpeed;
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

    void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        controller = GetComponent<EnemyController>();
        selfCollider = GetComponent<Collider2D>();
        self = GetComponent<KnightPathfinding>();
        cooldownHandler = GetComponent<CooldownTimer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grabs variables from controller
        // Movement Variables
        _nextWaypoint = controller.nextWaypoint;
        _waypointList = controller.waypointList;
        _playerRequiredProximity = controller.playerRequiredProximity;
        _maxSpeed = controller.maxSpeed;
        _currentSpeed = controller.currentSpeed;
        // Cooldown Variables
        _attackCooldown = controller.attackCooldown;

        List<string> keyList = new List<string> { "attackCooldown", "cliffDetectionInterval" };
        List<float> lengthList = new List<float> { _attackCooldown, cliffDetectionInterval };
        cooldownHandler.SetupTimers(keyList, lengthList, self);
    }

    // Fixed Update is called every set interval (about every 0.02 seconds)
    void FixedUpdate()
    {
        touchingDirections.CheckCollisions();

        // Cooldown Timer logic
        cooldownHandler.CheckCooldowns();

        // Makes player look in correct direction
        LookingDirection();

        // Choose how to make enemy move
        CalcDistanceToPlayer();
        distanceToPlayer = 1000000; // filler

        // If the player is within the enemy's tracking proximity
        if (distanceToPlayer <= _playerRequiredProximity)
        {
            // If not curently tracking player, the enemy is now tracking the player
            CurrentlyTrackingPlayer = true;
        }
        // If player leaves proximity, the enemy stops tracking them
        if (CurrentlyTrackingPlayer && distanceToPlayer > _playerRequiredProximity)
        {
            CurrentlyTrackingPlayer = false;
        }

        if (CurrentlyTrackingPlayer)
        {
            FindOptimalMovePath();
        }
        else
        {
            if (touchingDirections.IsGrounded)
            {
                moveDirection = lookDirection;
                yVelocity = 0;
            }
            if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
            {
                moveDirection = -1 * lookDirection;
                yVelocity = 0;
            }
            if (!touchingDirections.IsGrounded)
            {
                moveDirection = 0;
                yVelocity = rigidbody.linearVelocityY;
            }
            else
            {
                yVelocity = rigidbody.linearVelocityY;
            }
        }
        if (_currentSpeed <= _maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(_currentSpeed * moveDirection, yVelocity);
        }
        else if (_currentSpeed > _maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(_maxSpeed * moveDirection, yVelocity);
        }
    }

    // Allows specific processes to be coded in to happen once a cooldown ends
    public void CooldownEndProcess(string key)
    {

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
        // Flip enemy if a cliff is detected, but only every few seconds
        if (touchingDirections.IsGrounded && cooldownHandler.timerStatusDict["cliffDetectionInterval"] == 0)
        {
            lookDirection = -1 * lookDirection;
            moveDirection = lookDirection;
            transform.localScale *= new Vector2(-1, 1);
            cooldownHandler.timerStatusDict["cliffDetectionInterval"] = 1;
        }
    }

    public void CalcDistanceToPlayer() { }
    public void FindOptimalMovePath() { }
    public void ExecuteEnemyAttack() { }
}
