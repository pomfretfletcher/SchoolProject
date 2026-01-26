using UnityEngine;
using System.Collections.Generic;

public class BallScript : MonoBehaviour, ProjectileScript, UsesCooldown
{
    // Script + Component Links
    EnemyController enemyController;
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
        cooldownHandler = GetComponent<CooldownTimer>();

        // Sets up filter for collisions with walls
        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        int layerIndex = LayerMask.NameToLayer("Collidable");
        filter.layerMask = 1 << layerIndex;

        // Setup cooldown for how long ball will last
        List<string> keyList = new List<string> { "travelTime" };
        List<float> lengthList = new List<float> { travelTime };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
        cooldownHandler.timerStatusDict["travelTime"] = 1;
    }

    private void FixedUpdate()
    {
        // Make sure speed is always set to the move speed
        rigidbody.linearVelocityX = moveSpeed;

        // Detects if colliding will wall, if so, deletes self
        int count = selfCollider.Overlap(filter, results);
        if (count > 0)
        {
            Debug.Log("destroying1");
            Destroy(this.gameObject);
        }
    }

    public void AssignOwner(GameObject owner)
    {
        // Need to set owner of the projectile for enemies, as we can't just search for "player"
        enemyController = owner.GetComponent<EnemyController>();
    }

    public void FlipDirection()
    {
        moveSpeed *= -1;
    }

    public void CooldownEndProcess(string key)
    {
        // Destroys self after a set period of time being alive
        Destroy(this.gameObject);
    }

    // Stores every collision within the collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Calculate knockback
        Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

        GameObject collisionParent = collision.transform.root.gameObject;

        // If player
        if (collisionParent.gameObject.tag == "Player")
        {
            // Get hp handler component
            HPHandler hpHandler = collisionParent.GetComponent<HPHandler>();
            // Deal damage through hp handler component
            hpHandler.TakeDamage(enemyController.rangedDamage);
        }

        // Destroy self
        Destroy(this.gameObject);
    }
}