using UnityEngine;
using System.Collections.Generic;

public class ArrowScript : MonoBehaviour, ProjectileScript, UsesCooldown
{
    // Script + Component Links
    PlayerController playerController;
    Rigidbody2D rigidbody;
    Collider2D selfCollider;
    CooldownTimer cooldownHandler;

    // Customizable Values
    public Vector2 knockback = Vector2.zero;
    public float moveSpeed;
    public float travelTime;

    // Private variables/objects for filter
    private ContactFilter2D filter;
    private Collider2D[] results = new Collider2D[16];

    private void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        selfCollider = GetComponent<Collider2D>();
        cooldownHandler = GetComponent<CooldownTimer>();

        // Sets up filter for collisions with walls
        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        int layerIndex = LayerMask.NameToLayer("Collidable");
        filter.layerMask = 1 << layerIndex;

        // Setup cooldown for how long arrow will last
        List<string> keyList = new List<string> { "travelTime" };
        List<float> lengthList = new List<float> { travelTime };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
        cooldownHandler.timerStatusDict["travelTime"] = 1;
    }

    private void FixedUpdate()
    {
        // Sets speed to move speed each frame
        rigidbody.linearVelocityX = moveSpeed;

        // Detects if colliding will wall, if so, deletes self
        int count = selfCollider.Overlap(filter, results);
        if (count > 0)
        {
            Destroy(this.gameObject);
        }
    }

    // Used for enemy projectile scripts but needs to be here for the interface to work properly
    public void AssignOwner(GameObject owner) { }

    // Used on creation to align with fired direction
    public void FlipDirection()
    {
        moveSpeed *= -1;
    }

    public void CooldownEndProcess(string key)
    {
        // Destroys self after a set period of time being alive
        Destroy(this.gameObject);
    }

    // When collides with wall or enemy etc, chooses appropiate logic
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Calculate knockback
        Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

        // Grabs parent object of the collision to check tag
        GameObject collisionParent = collision.transform.root.gameObject;

        // If enemy
        if (collisionParent.gameObject.tag == "Enemy")
        {
            // Get hp handler component
            HPHandler hpHandler = collisionParent.GetComponent<HPHandler>();

            // Deal damage through hp handler component
            if (playerController.currentRangedDamage > playerController.minRangedDamage)
            {
                hpHandler.TakeDamage(playerController.currentRangedDamage);
            }
            else
            {
                hpHandler.TakeDamage(playerController.minRangedDamage);
            }
        }
        // Destroy self
        Destroy(this.gameObject);
    }
}