using UnityEngine;
using System.Collections.Generic;

public class FireBallScript : MonoBehaviour, UsesCooldown, ProjectileScript
{
    // Script + Component Links
    FireRain callingScript;
    Rigidbody2D rigidbody;
    Collider2D selfCollider;
    CooldownTimer cooldownHandler;

    // Customizable Values
    public float moveSpeed;
    public float travelTime;

    // Private variables/objects for filter
    private ContactFilter2D filter;
    private Collider2D[] results = new Collider2D[16];
    private float damage;

    private void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        cooldownHandler = GetComponent<CooldownTimer>();
        selfCollider = GetComponent<Collider2D>();

        // Sets up filter for collisions with walls
        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        int layerIndex = LayerMask.NameToLayer("Collidable");
        filter.layerMask = 1 << layerIndex;

        // Setup cooldown for how long fire ball will last
        List<string> keyList = new List<string> { "travelTime" };
        List<float> lengthList = new List<float> { travelTime };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
        cooldownHandler.timerStatusDict["travelTime"] = 1;
    }

    private void FixedUpdate()
    {
        // Sets speed to move speed each frame
        rigidbody.linearVelocityY = -moveSpeed;

        // Detects if colliding will wall, if so, deletes self
        int count = selfCollider.Overlap(filter, results);
        if (count > 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetDamage(float givenDamage)
    {
        damage = givenDamage;
    }
    
    public void CooldownEndProcess(string key)
    {
        // Destroys self after a set period of time being alive
        Destroy(this.gameObject);
    }

    // Used for enemy projectile scripts but needs to be here for the interface to work properly
    public void AssignOwner(GameObject owner) { }

    // Used for enemy projectile scripts but needs to be here for the interface to work properly
    public void FlipDirection() { }

    // Stores every collision within the collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform collisionParent = collision.transform;

        // If player
        if (collisionParent.gameObject.tag == "Enemy")
        {
            Debug.Log("SEEN ENEMY");
            Debug.Log(collisionParent);
            // Get hp handler component
            HPHandler hpHandler = collisionParent.GetComponent<HPHandler>();
            // Deal damage through hp handler component
            hpHandler.TakeDamage(damage);
        }
        // Destroy self
        Destroy(this.gameObject);
    }
}