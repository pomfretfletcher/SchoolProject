using UnityEngine;
using System.Collections.Generic;

public class RegenPickup : MonoBehaviour, IsConsumable, UsesCooldown
{
    // Script + Component Links
    PlayerController controller;
    SpriteRenderer renderer;
    Collider2D collider;
    CooldownTimer cooldownHandler;

    // Interface Cast Variables
    public bool PickedUp { get => pickedUp; set => pickedUp = value; }

    // Customizable Values
    public float regenAmount;
    public float timesToRegen;
    public float regenInterval;

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
        List<string> keyList = new List<string> { "regenInterval" };
        List<float> lengthList = new List<float> { regenInterval };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    // Called by pickup script when this pickup is collected
    public bool OnPickup()
    {
        // Only starts buff if health left to gain
        if (controller.currentHealth < controller.fullHealth)
        {
            // Starts timer to heal every set interval
            cooldownHandler.timerStatusDict["regenInterval"] = 1;

            // Make invisible and uncollidable so act as a background buff
            renderer.enabled = false;
            collider.enabled = false;

            // Tell general script it has been picked up
            pickedUp = true; 
        }
       
        // Tells general pickup script to not delete the pickup
        return false;
    }

    public void CooldownEndProcess(string key)
    {
        // Gain health equal to regen amount
        controller.currentHealth += regenAmount;

        // Default to full health if healed above
        if (controller.currentHealth > controller.fullHealth)
        {
            controller.currentHealth = controller.fullHealth;
        }

        // Restart interval timer
        cooldownHandler.timerStatusDict["regenInterval"] = 1;

        // Decrements how many times left to heal
        timesToRegen--;

        // Destroys self if regened the right amount of times
        if (timesToRegen == 0)
        {
            Destroy(this.gameObject);
        }
    }
}