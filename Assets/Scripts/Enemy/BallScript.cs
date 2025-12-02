using UnityEngine;

public class BallScript : MonoBehaviour, ProjectileScript
{
    // Script + Component Links
    EnemyController enemyController;
    Rigidbody2D rigidbody;

    // Attack Variables
    public Vector2 knockback = Vector2.zero;

    // Movement Variables
    public float moveSpeed;

    void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        //controller = GameObject.Find("").GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        rigidbody.linearVelocityX = moveSpeed;
    }

    public void AssignOwner(GameObject owner)
    {
        enemyController = owner.GetComponent<EnemyController>();
    }

    public void FlipDirection()
    {
        moveSpeed *= -1;
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
