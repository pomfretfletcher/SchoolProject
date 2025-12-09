using UnityEngine;
using System.Collections.Generic;

public class FireBallScript : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    FireRain callingScript;
    Rigidbody2D rigidbody;
    CooldownTimer cooldownHandler;

    // Movement Variables
    public float moveSpeed;
    public float lifeLength;

    void Awake()
    {
        // Grabs all linked scripts + components
        rigidbody = GetComponent<Rigidbody2D>();
        cooldownHandler = GetComponent<CooldownTimer>();
        
        // Setup cooldown for how long fire ball will last
        List<string> keyList = new List<string> { "lifeLength" };
        List<float> lengthList = new List<float> { lifeLength };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
        cooldownHandler.timerStatusDict["lifeLength"] = 1;
    }

    void FixedUpdate()
    {
        rigidbody.linearVelocityY = -moveSpeed;
        cooldownHandler.CheckCooldowns();
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
