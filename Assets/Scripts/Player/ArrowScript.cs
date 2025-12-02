using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    // Script + Component Links
    PlayerController playerController;
    Rigidbody2D rigidbody;

    // Attack Variables
    public Vector2 knockback = Vector2.zero;

    // Movement Variables
    public float moveSpeed;

    void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        rigidbody.linearVelocityX = moveSpeed;
    }

    // Stores every collision within the collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Calculate knockback
        Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);


        GameObject collisionParent = collision.transform.root.gameObject;

        // If enemy
        if (collisionParent.gameObject.tag == "Enemy")
        {
            // Get hp handler component
            HPHandler hpHandler = collisionParent.GetComponent<HPHandler>();

            // Deal damage through hp handler component
            hpHandler.TakeDamage(playerController.currentRangedDamage);
        }
        // Destroy self
        Destroy(this.gameObject);
    }
}
