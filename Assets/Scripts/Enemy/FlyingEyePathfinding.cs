using UnityEngine;

public class FlyingEyePathfinding : MonoBehaviour, LogicScript
{
    // Script + Component Links
    EnemyController controller;
    GameObject player;
    Rigidbody2D rigidbody;
    Animator animator;
    TouchingDirections touchingDirections;
    CooldownTimer cooldownHandler;
    UniversalEnemyFunctions universalEnemyFunctions;

    // Internal Logic Variables
    private float distanceToPlayer;
    LayerMask collidableMask;

    // States
    [Header("Movement States")]
    public bool IsMoving { get { return isMoving; } set { isMoving = value; animator.SetBool("isMoving", value); } }
    [SerializeField] private bool isMoving = false;
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    [SerializeField] private bool canMove = true;
    public bool IsSufferingKnockback { get { return isSufferingKnockback; } set { isSufferingKnockback = value; } }
    [SerializeField] private bool isSufferingKnockback = false;

    [Header("Directions")]
    public int LookDirection { get { return lookDirection; } set { lookDirection = value; } }
    [SerializeField] private int lookDirection = 1;
    public int MoveDirection { get { return xMoveDirection; } set { xMoveDirection = value; } }
    private int xMoveDirection = 1;

    [Header("Combat States")]
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    [SerializeField] private bool canAttack = true;
    public bool isAttacking = false;

    [Header("Tracking States")]
    public bool CurrentlyTrackingPlayer = false;

    private void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        controller = GetComponent<EnemyController>();
        cooldownHandler = GetComponent<CooldownTimer>();
        player = GameObject.Find("Player");
        universalEnemyFunctions = GetComponent<UniversalEnemyFunctions>();
        collidableMask = LayerMask.GetMask("Collidable");
    }

    private void FixedUpdate()
    {
        // Checks envionmental collisions
        touchingDirections.CheckCollisions();

        // Makes player look in correct direction
        universalEnemyFunctions.LookingDirection();

        // If not dead, and not currently experiencing knockback, decide logic and act on it
        if (controller.currentHealth > 0 && !isSufferingKnockback)
        {
            // Choose how to make enemy move
            DecidePathfinding();

            // Pathfind based on waypoints and player
            RaytraceAndMove();
        }
        else
        {
            // Set vals to default
            rigidbody.linearVelocityX = 0;
            IsMoving = false;
        }
    }

    public void DecidePathfinding()
    {
        // Calculate the distance to the player with vector math
        Vector2 offset = transform.position - player.transform.position;
        distanceToPlayer = offset.magnitude;

        // Start tracking player
        if (distanceToPlayer < controller.playerRequiredProximity && cooldownHandler.timerStatusDict["trackIntervalTime"] == 0)
        {
            CurrentlyTrackingPlayer = true;
        }
        // Go to waypoints
        else
        {
            CurrentlyTrackingPlayer = false;
        }
    }

    public void RaytraceAndMove() 
    {
        // Decide where pathfinding to
        Transform targetNode;
        if (!CurrentlyTrackingPlayer) { targetNode = controller.nextWaypoint; }
        else { targetNode = player.transform; }

        if (CanMove)
        {
            // Calculate direction to target node, setup data for calculating the best path to move to get closest to direction
            Vector2 direction = ((Vector2)targetNode.position - (Vector2)transform.position).normalized;
            Vector2 bestDir = direction;
            float bestScore = -Mathf.Infinity;

            // For every few degress within a cone in front of the enemy
            for (int i = 0; i < 7; i++)
            {
                // Calculate specific angle to check, as well as direction
                float t = i / (float)(6);
                float angle = Mathf.Lerp(-60f, 60f, t);
                float score = 0f;
                Vector2 dir = Quaternion.Euler(0, 0, angle) * direction;

                // See if the direction fired ray would collide, if it does, we don't follow it
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 3f, collidableMask);

                if (!hit)
                {
                    // Prefer directions closer to target direction
                    score = Vector2.Dot(dir, direction);
                }

                // If closest, choose this point to move to
                if (score > bestScore)
                {
                    bestScore = score;
                    bestDir = dir;
                }
            }

            // If close enough to player, do not move
            if (CurrentlyTrackingPlayer && (targetNode.transform.position - transform.position).magnitude < 0.2f)
            {
                bestDir = Vector3.zero;
            }

            // Apply movement based on direction
            transform.position += (Vector3)(bestDir * controller.currentSpeed * Time.deltaTime);

            // Reduce amount of time enemy spends flying into ceiling
            if (touchingDirections.IsOnCeiling)
            {
                transform.position += new Vector3 (bestDir.x * controller.currentSpeed * Time.deltaTime * 0.5f, 0, 0);
            }

            // Reduce time enemy spends flying near ground
            if (touchingDirections.IsGrounded)
            {
                transform.position += new Vector3(0, controller.currentSpeed * Time.deltaTime, 0);
            }

            // Based on the direction moving, look said direction. Needed as enemy does not actually have velocity
            if (((Vector3)(bestDir * controller.currentSpeed * Time.deltaTime)).x >= 0 && LookDirection < 0) 
            { 
                LookDirection = 1; 
                MoveDirection = 1; 
            }
            else if (((Vector3)(bestDir * controller.currentSpeed * Time.deltaTime)).x < 0 && LookDirection > 0) 
            { 
                LookDirection = -1; 
                MoveDirection = -1; 
            }

            IsMoving = true;
        }

        // If close enough to waypoint, cycle list and start moving to next
        float distanceToWaypoint = (transform.position - controller.nextWaypoint.position).magnitude;
        if (distanceToWaypoint < 0.5f)
        {
            controller.waypointIndex++;
            controller.nextWaypoint = controller.waypointList[controller.waypointIndex % 3];
        }

        rigidbody.linearVelocity = Vector2.zero;
    }

    public void ExecuteEnemyAttack()
    {
        if (cooldownHandler.timerStatusDict["attackCooldown"] == 0 && CanAttack && controller.CurrentHealth > 0)
        {
            // Start cooldowns, tell animator an attack has occured and lock movement temporarily
            cooldownHandler.timerStatusDict["attackCooldown"] = 1;
            cooldownHandler.timerStatusDict["attackLockTime"] = 1;
            cooldownHandler.timerStatusDict["isAttacking"] = 1;
            animator.SetTrigger("attacked");
        }
    }
}