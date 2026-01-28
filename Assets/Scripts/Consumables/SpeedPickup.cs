using UnityEngine;
using System.Collections.Generic;

public class SpeedPickup : MonoBehaviour, IsConsumable, UsesCooldown
{
    // Script + Component Links
    PlayerController controller;
    SpriteRenderer renderer;
    Collider2D collider;
    CooldownTimer cooldownHandler;

    // Interface Cast Variables
    public bool PickedUp { get => pickedUp; set => pickedUp = value; }

    // Customizable Values
    public float speedBoostPercentage;
    public float speedBoostLength;

    // Internal Logic Variables
    private bool pickedUp = false;

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
        // Increase player speed
        controller.currentSpeed *= (1 + speedBoostPercentage);

        // Make sprite invisible
        renderer.enabled = false;
        collider.enabled = false;

        // Starts timer for how long effect will last
        cooldownHandler.timerStatusDict["speedBoostLength"] = 1;

        // Tells general pickup script it has been picked up and to not delete the pickup
        pickedUp = true;
        return false;
    }

    public void CooldownEndProcess(string key)
    {
        // Reset speed to pre pickup value
        controller.currentSpeed = controller.prePickupCurrentSpeed;
        // Destroy self
        Destroy(this.gameObject);
    }
}