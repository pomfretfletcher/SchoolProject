using System.Collections.Generic;
using UnityEngine;

public class AbilityQuickenPickup : MonoBehaviour, UsesCooldown, IsConsumable
{
    // Script + Component Links
    SpriteRenderer renderer;
    Collider2D collider;
    CooldownTimer cooldownHandler;
    CooldownTimer playerCooldownHandler;

    // Interface Cast Variables
    public bool PickedUp { get => pickedUp; set => pickedUp = value; }
    private bool pickedUp = false;

    // Customizable Values
    public float pickupEffectLength;

    private void Awake()
    {
        // Grabs all linked scripts + components
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        cooldownHandler = GetComponent<CooldownTimer>();
        playerCooldownHandler = GameObject.Find("Player").GetComponent<CooldownTimer>();
    }

    private void Start()
    {
        // Setup cooldown for how long boost lasts
        List<string> keyList = new List<string> { "pickupEffectLength" };
        List<float> lengthList = new List<float> { pickupEffectLength };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    private void FixedUpdate()
    {
        if (pickedUp)
        {
            // For each active ability cooldown
            if (playerCooldownHandler.timerStatusDict["abilityOneCooldown"] == 1)
            {
                // Iterate timer by delta time again, essentially doubles timer speed
                playerCooldownHandler.timerDict["abilityOneCooldown"] += Time.deltaTime;
            }
            if (playerCooldownHandler.timerStatusDict["abilityTwoCooldown"] == 1)
            {
                playerCooldownHandler.timerDict["abilityTwoCooldown"] += Time.deltaTime;
            }
            if (playerCooldownHandler.timerStatusDict["abilityThreeCooldown"] == 1)
            {
                playerCooldownHandler.timerDict["abilityThreeCooldown"] += Time.deltaTime;
            }
        }
    }

    public bool OnPickup()
    {
        // Make sprite invisible
        renderer.enabled = false;
        collider.enabled = false;

        // Starts timer for how long effect will last
        cooldownHandler.timerStatusDict["pickupEffectLength"] = 1;

        // Tells general pickup script that it has been picked up and to not delete the pickup
        pickedUp = true;
        return false;
    }

    public void CooldownEndProcess(string key)
    {
        // Destroy self
        Destroy(this.gameObject);
    }
}