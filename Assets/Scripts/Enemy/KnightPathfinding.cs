using System.Collections.Generic;
using UnityEngine;

public class KnightPathfinding : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    EnemyController controller;
    GameObject player;
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
    private float _invulnerableOnHitTime;
    private float _attackLockTime;

    // Movement Variables
    private int _maxSpeed;
    private float _currentSpeed;
    private float yVelocity;
    [SerializeField]
    private int lookDirection = 1;
    [SerializeField]
    private int moveDirection;
    public int trackButNotMoveProximity;

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
    public bool trackingButNotMove;
    public bool trackingOffCliff;

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
        player = GameObject.Find("Player");
        cliffDetectionZone = GameObject.Find("CliffDetectionZone").GetComponent<DetectionZone>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grabs variables from controller
        _nextWaypoint = controller.nextWaypoint;
        _waypointList = controller.waypointList;
        _playerRequiredProximity = controller.playerRequiredProximity;
        _maxSpeed = controller.maxSpeed;
        _currentSpeed = controller.currentSpeed;
        _attackCooldown = controller.attackCooldown;
        _invulnerableOnHitTime = controller.invulnerableOnHitTime;
        _attackLockTime = controller.attackLockTime;

        List<string> keyList = new List<string> { "attackCooldown", "cliffDetectionInterval", "attackLockTime", "invulnerableOnHitTime" };
        List<float> lengthList = new List<float> { _attackCooldown, cliffDetectionInterval, _attackLockTime, _invulnerableOnHitTime };
        cooldownHandler.SetupTimers(keyList, lengthList, self);
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


        // If the player is within the enemy's tracking proximity
        if (distanceToPlayer <= _playerRequiredProximity)
        {
            // If not curently tracking player, the enemy is now tracking the player
            CurrentlyTrackingPlayer = true;
        }
        // If player leaves proximity, the enemy stops tracking them
        else
        {
            CurrentlyTrackingPlayer = false;
            trackingOffCliff = false;
            trackingButNotMove = false;
            // If stop tracking player but still at cliff edge, flip direction
            if (cliffDetectionZone.detectedColliders.Count == 0)
            {
                lookDirection = -1 * lookDirection;
                moveDirection = lookDirection;
                transform.localScale *= new Vector2(-1, 1);
            }
        }
        if (trackingOffCliff && cliffDetectionZone.detectedColliders.Count > 0) { trackingOffCliff = false; }


        // Tracking player movement decisions
        if (CurrentlyTrackingPlayer && touchingDirections.IsGrounded)
        {
            // Stops enemy from moving if they are still tracking, but at cliff edge
            if (cliffDetectionZone.detectedColliders.Count == 0)
            {
                moveDirection = 0;
                yVelocity = 0;
            }
            // Stops enemy from moving if they are within a small proximity of the player
            else if (trackingButNotMove && touchingDirections.IsGrounded)
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
        if (_currentSpeed <= _maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(_currentSpeed * moveDirection, yVelocity);
        }
        else if (_currentSpeed > _maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(_maxSpeed * moveDirection, yVelocity);
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

        // Once in a set distance of the player, continue tracking them but stand still
        if (distanceToPlayer <= trackButNotMoveProximity)
        {
            trackingButNotMove = true;
        }
        else
        {
            trackingButNotMove = false;
        }
    }

    public void FindOptimalMovePath() { }
    public void ExecuteEnemyAttack() 
    {
        if (cooldownHandler.timerStatusDict["attackCooldown"] == 0 && CanAttack)
        {
            cooldownHandler.timerStatusDict["attackCooldown"] = 1;
            cooldownHandler.timerStatusDict["attackLockTime"] = 1;
            animator.SetTrigger("attacked");
            CanMove = false;
        }
    }
}
