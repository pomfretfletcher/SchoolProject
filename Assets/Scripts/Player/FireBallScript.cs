using UnityEngine;
using System.Collections.Generic;

public class FireBallScript : MonoBehaviour, UsesCooldown
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
    
    public void CooldownEndProcess(string key)
    {
        // Destroys self after a set period of time being alive
        Destroy(this.gameObject);
    }

    // Stores every collision within the collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionParent = collision.transform.root.gameObject;

        // If player
        if (collisionParent.gameObject.tag == "Enemy")
        {
            // Get hp handler component
            HPHandler hpHandler = collisionParent.GetComponent<HPHandler>();
            // Deal damage through hp handler component
            hpHandler.TakeDamage(callingScript.damage);
        }
        // Destroy self
        Destroy(this.gameObject);
    }
}