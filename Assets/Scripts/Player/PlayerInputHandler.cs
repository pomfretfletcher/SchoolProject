using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, LogicScript
{
    // Script + Component Links
    PlayerController controller;
    Rigidbody2D rigidbody;
    Animator animator;
    TouchingDirections touchingDirections;
    CooldownTimer cooldownHandler;
    MenuNavigation menuNav;

    // Internal Logic Variables
    private int currentDashDirection = 1;
    private int attackCombo = 0;
    private float xVelocity;
    private float yVelocity;
    private Vector2 moveInput;

    // States
    [Header("Movement States")]
    [SerializeField] private bool isMoving = false;
    public bool IsMoving { get { return isMoving; } set { isMoving = value; animator.SetBool("isMoving", value); } }
    [SerializeField] private bool isDashing = false;
    public bool IsDashing { get { return isDashing; } set { isDashing = value; animator.SetBool("isDashing", value); } }
    [SerializeField] private bool canMove = true;
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    [SerializeField] private bool isSufferingKnockback = false;
    public bool IsSufferingKnockback { get { return isSufferingKnockback; } set { isSufferingKnockback = value; } }

    [Header("Directions")]
    [SerializeField] private int lookDirection = 1;
    public int LookDirection { get { return lookDirection; } set { lookDirection = value; } }
    [SerializeField] private int moveDirection = 1;
    public int MoveDirection { get { return moveDirection; } set { moveDirection = value; } }

    [Header("Combat States")]
    [SerializeField] private bool canAttack = true;
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    public bool isMeleeAttacking = false;
    private int meleeAttackDirection;
    public bool isRangedAttacking = false;

    [Header("Ability States")]
    public bool CanUseAbilities = true;

    [Header("Combat Effects")]
    public bool specialCombatEffectActive = false;
    public string currentSpecialCombatEffect;
    public float currentEffectDamage;
    public GameObject currentEffectPrefab;
    public float currentEffectLength;

    [Header("Misc States")]
    public bool movingThroughRooms = false;
    public bool invulnerableFromAnotherSource = false;
    public bool inPauseMenu = false;
    public bool inInventoryMenu = false;

    private void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        touchingDirections = GetComponent<TouchingDirections>();
        cooldownHandler = GetComponent<CooldownTimer>();
        menuNav = GameObject.Find("GameHandler").GetComponent<MenuNavigation>();
    }

    private void FixedUpdate()
    {
        // Checks envionmental collisions
        touchingDirections.CheckCollisions();

        // Makes player look in correct direction
        LookingDirection();

        Vector2 velocity = DecideInputLogic();
        xVelocity = velocity.x; yVelocity = velocity.y;

        // Apply decided upon velocity
        ApplyVelocity();

        UpdateAnimatorParameters();
    }

    public void LookingDirection()
    {
        // If player can't move, don't change their orientation
        if (!CanMove) { return; }
        // Can't change direction if melee attacking
        if (isMeleeAttacking) { return; }

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
        moveDirection = (int)moveInput.x;

        // Sets IsMoving to true or false dependent on input being zero or not
        IsMoving = moveInput.x != 0;
    }

    public Vector2 DecideInputLogic()
    {
        if (!CanMove) { return new Vector2(0, rigidbody.linearVelocityY); }
        if (movingThroughRooms) { return Vector2.zero; }

        // Can move forward if attacking, but only if in air
        if (isMeleeAttacking && !touchingDirections.IsGrounded)
        {
            xVelocity = meleeAttackDirection == moveInput.x ? moveInput.x * controller.currentSpeed : 0;
            yVelocity = rigidbody.linearVelocityY;
        }
        else if (isMeleeAttacking && touchingDirections.IsGrounded)
        {
            xVelocity = 0;
            yVelocity = 0;
        }
        // If dashing, keep velocity horizontally and stay in air
        else if (IsDashing && CanMove)
        {
            xVelocity = controller.dashImpulse * currentDashDirection;
            yVelocity = 0;
        }
        // Regular movement based input based on current speed
        else if (!IsDashing && CanMove)
        {
            xVelocity = controller.currentSpeed * moveInput.x;
            yVelocity = rigidbody.linearVelocityY;
        }
        // Stops horizontal movement if on wall
        if (touchingDirections.IsOnWall)
        {
            xVelocity = 0;
            yVelocity = rigidbody.linearVelocityY;
        }

        return new Vector2(xVelocity, yVelocity);
    }

    public void ApplyVelocity()
    {
        // Don't apply decided velocity if being knockbacked, continue with knockback velocity
        if (isSufferingKnockback) { return; }

        // Updates the velocity of the player while limiting it to the player's max speed
        if (xVelocity <= controller.maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(xVelocity, yVelocity);
        }
        if (xVelocity > controller.maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(controller.maxSpeed, yVelocity);
        }
    }

    public void UpdateAnimatorParameters()
    {
        // Updates the animator's yVelocity value for air state animations
        animator.SetFloat("yVelocity", rigidbody.linearVelocityY);
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
        if (!CanAttack || isRangedAttacking || isMeleeAttacking || cooldownHandler.timerStatusDict["comboEndAttackDelay"] == 1) { return; }
        
        // Tell animator to start melee attack animation and starts melee attack cooldown
        if (cooldownHandler.timerStatusDict["comboTime"] == 1 && touchingDirections.IsGrounded)
        {
            // Combo attacks
            if (attackCombo == 1)
            {
                // Starts timer
                cooldownHandler.timerStatusDict["meleeAttackCooldown"] = 1;
                cooldownHandler.timerStatusDict["isMeleeAttacking"] = 1;
                // Resets combotime
                cooldownHandler.timerDict["comboTime"] = 0;
                animator.SetTrigger("comboAttack1");
                // Iterates to next combo stage
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
                // Iterates to next combo stage
                attackCombo = 3;
                isMeleeAttacking = true;
            }
            else
            {
                // Finish combo, reset timer and turn it off
                cooldownHandler.timerDict["comboTime"] = 0;
                cooldownHandler.timerStatusDict["comboTime"] = 0;
                attackCombo = 0;
                cooldownHandler.timerStatusDict["comboEndAttackDelay"] = 1;
            }
        }

        // Regular melee attack
        if (cooldownHandler.timerStatusDict["comboTime"] == 0 && touchingDirections.IsGrounded)
        {
            cooldownHandler.timerStatusDict["meleeAttackCooldown"] = 1;
            cooldownHandler.timerStatusDict["isMeleeAttacking"] = 1;
            cooldownHandler.timerStatusDict["comboTime"] = 1;
            animator.SetTrigger("meleeAttacked");
            // Starts melee combo
            attackCombo = 1;
            isMeleeAttacking = true;
        }

        // Inair melee attack
        if (!touchingDirections.IsGrounded)
        {
            cooldownHandler.timerStatusDict["meleeAttackCooldown"] = 1;
            cooldownHandler.timerStatusDict["isMeleeAttacking"] = 1;
            // Resets combo if needed
            attackCombo = 0;
            animator.SetTrigger("inAirMeleeAttacked");
            isMeleeAttacking = true;
            // Allow player to move in the direction they're attacking, but not any other
            meleeAttackDirection = lookDirection;
        }
    }

    public void ExecutePlayerRangedAttack(InputAction.CallbackContext context)
    {
        // Tell animator to start ranged attack animation and starts ranged attack cooldown
        if (touchingDirections.IsGrounded && !isRangedAttacking && !isMeleeAttacking)
        {
            cooldownHandler.timerStatusDict["rangedAttackCooldown"] = 1;
            cooldownHandler.timerStatusDict["isRangedAttacking"] = 1;
            animator.SetTrigger("rangedAttacked");
            cooldownHandler.timerStatusDict["projectileFireDelay"] = 1;
            CanMove = false;
            isRangedAttacking = true;
        }
    }

    public void OpenPauseMenu(InputAction.CallbackContext context) 
    {
        if (menuNav.tutorialScreen.activeSelf == true) { return; }
        if (!inInventoryMenu && !inPauseMenu) { menuNav.OpenPauseScreen(); }
        else if (!inInventoryMenu && inPauseMenu) { menuNav.ClosePauseScreen(); }
    }

    public void OpenInventory(InputAction.CallbackContext context)
    {
        if (menuNav.tutorialScreen.activeSelf == true) { return; }
        if (!inPauseMenu && inInventoryMenu) { menuNav.ContinueRunFromInventory(); }
        else if (!inPauseMenu && !inInventoryMenu) { menuNav.OpenInventoryFromGame(); }
    }

    public void SetCombatEffect(string effectType, float damageAmount, GameObject effectPrefab, float effectLength)
    {
        currentSpecialCombatEffect = effectType;
        specialCombatEffectActive = true;
        currentEffectPrefab = effectPrefab;
        currentEffectDamage = damageAmount;
        currentEffectLength = effectLength;
    }
}