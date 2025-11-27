using UnityEngine;
using System.Collections.Generic;

public class SpeedPickup : MonoBehaviour, IsPickup, UsesCooldown
{
    // Script + Component Links
    PlayerController controller;
    SpriteRenderer renderer;
    Collider2D collider;
    CooldownTimer cooldownHandler;

    // Amount the player will be healed on pickup
    public float speedBoostPercentage;
    public float speedBoostLength;
    private float _prevSpeed;

    private void Awake()
    {
        // Grabs all linked scripts + components
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        cooldownHandler = GetComponent<CooldownTimer>();
    }

    private void Start()
    {
        // Setup cooldown for how long boost lasts
        List<string> keyList = new List<string> { "speedBoostLength" };
        List<float> lengthList = new List<float> { speedBoostLength };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    // Called by pickup script when this pickup is collected
    public bool OnPickup()
    {
        // Stores initial speed variable to reset to after buff finishes
        _prevSpeed = controller.currentSpeed;
        // Increase player speed
        controller.currentSpeed *= (1 + speedBoostPercentage);
        renderer.enabled = false;
        collider.enabled = false;
        cooldownHandler.timerStatusDict["speedBoostLength"] = 1;
        // Tells general pickup script to not delete the pickup
        return false;
    }
    
    public void FixedUpdate() 
    {
        cooldownHandler.CheckCooldowns();
    }

    public void CooldownEndProcess(string key)
    {
        // Reset speed
        controller.currentSpeed = _prevSpeed;
        // Destroy self
        Destroy(this.gameObject);
    }
}
