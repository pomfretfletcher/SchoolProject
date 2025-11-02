using System.Collections.Generic;
using UnityEngine;

public class KnightPathfinding : MonoBehaviour
{
    // Enemy Controller and Player Links
    public EnemyController controller;
    Object player;
    Rigidbody2D rigidbody;
    Animator animator;

    // Waypoint Variables
    private Transform _nextWaypoint;
    private List<Transform> _waypointList;

    // Distance Variables
    public float distanceToPlayer;
    private int _playerRequiredProximity;

    // Cooldown Variables
    private int _attackCooldown;

    // Movement Variables
    private int _maxSpeed;
    private float _currentSpeed;
    private float yVelocity;
    [SerializeField]
    private int lookDirection = 1;
    [SerializeField]
    private int moveDirection;

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

        // Touching Direction states
    public bool IsGrounded { get { return isGrounded; } private set { isGrounded = value; animator.SetBool("isGrounded", value); } }
    [SerializeField]
    private bool isGrounded = false;
    public bool IsOnWall { get { return isOnWall; } private set { isOnWall = value; animator.SetBool("isOnWall", value); } }
    [SerializeField]
    private bool isOnWall = false;
    public bool IsOnCeiling { get { return isOnCeiling; } private set { isOnCeiling = value; animator.SetBool("isOnCeiling", value); } }
    [SerializeField]
    private bool isOnCeiling = false;
        // Movement States
    public bool CanMove { get; private set; }
    public bool CanAttack { get; private set; }
    public bool CurrentlyTrackingPlayer;

    // Touching direction variables
    public ContactFilter2D castFilter;
    public Collider2D selfCollider;
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float ceilingDistance = 0.05f;
    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    // Used to decide which wall direction to check
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    // Prevent enemy walking off 'cliff'
    public DetectionZone cliffDetectionZone;
    private int cliffDetectionInterval = 3;

    // Awake is called once the MonoBehaviour is created
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        selfCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
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

        // Add cooldown variables to list and creates corresponding timer
        cooldownDict.Add("attackCooldown", _attackCooldown); timerDict.Add("attackCooldown", 0); timerStatusDict.Add("attackCooldown", 0); cooldownKeys.Add("attackCooldown"); cooldownCount += 1;
        cooldownDict.Add("cliffDetectionInterval", cliffDetectionInterval); timerDict.Add("cliffDetectionInterval", 0); timerStatusDict.Add("cliffDetectionInterval", 0); cooldownKeys.Add("cliffDetectionInterval"); cooldownCount += 1;
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
            if (IsGrounded)
            {
                moveDirection = lookDirection;
                yVelocity = 0;
            }
            if (IsGrounded && IsOnWall)
            {
                moveDirection = -1 * lookDirection;
                yVelocity = 0;
            }
            if (!IsGrounded)
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
        if (_currentSpeed > _maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(_maxSpeed * moveDirection, yVelocity);
        }
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
    public void cooldownEndProcess()
    {

    }

    public void lookingDirection()
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
        if (IsGrounded && timerStatusDict["cliffDetectionInterval"] == 0)
        {
            lookDirection = -1 * lookDirection;
            moveDirection = lookDirection;
            transform.localScale *= new Vector2(-1, 1);
            timerStatusDict["cliffDetectionInterval"] = 1;
        }
    }

    public void CalcDistanceToPlayer() { }
    public void FindOptimalMovePath() { }
    public void ExecuteEnemyAttack() { }
}
