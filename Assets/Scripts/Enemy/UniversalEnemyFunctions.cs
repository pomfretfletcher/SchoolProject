using UnityEngine;

public class UniversalEnemyFunctions : MonoBehaviour
{
    LogicScript ls;
    EnemyController controller;
    Rigidbody2D rigidbody;

    private void Awake()
    {
        ls = GetComponent<LogicScript>();
        controller = GetComponent<EnemyController>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void LookingDirection()
    {
        // If enemy can't move, don't change their orientation
        if (!ls.CanMove) { return; }

        if (ls.MoveDirection == 1 && ls.LookDirection != 1)
        {
            // Changes look direction to 1 (right)
            ls.LookDirection = 1;
            // Flips transform
            transform.localScale *= new Vector2(-1, 1);
        }
        if (ls.MoveDirection == -1 && ls.LookDirection != -1)
        {
            // Changes look direction to -1 (left)
            ls.LookDirection = -1;
            // Flips transform
            transform.localScale *= new Vector2(-1, 1);
        }

        // Keeps movement direction, look direction and localscale in parity
        if (ls.LookDirection == -1 && transform.localScale.x > 0) { transform.localScale *= new Vector2(-1, 1); }
        if (ls.LookDirection == 1 && transform.localScale.x < 0) { transform.localScale *= new Vector2(-1, 1); }
    }

    public void ApplyVelocity(float yVelocity)
    {
        // Don't apply decided velocity if being knockbacked, continue with knockback velocity
        if (ls.IsSufferingKnockback) { return; }

        // Limit enemy movement to maxspeed
        if (controller.currentSpeed <= controller.maxSpeed)
        {
            rigidbody.linearVelocity = new Vector2(controller.currentSpeed * ls.MoveDirection, yVelocity);
        }
        else
        {
            rigidbody.linearVelocity = new Vector2(controller.maxSpeed * ls.MoveDirection, yVelocity);
        }
    }

    public void UpdateIsMoving()
    {
        if (ls.MoveDirection == 0)
        {
            ls.IsMoving = false;
        }
        else
        {
            ls.IsMoving = true;
        }
    }
}
