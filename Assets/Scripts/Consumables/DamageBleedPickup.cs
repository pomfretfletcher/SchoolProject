using UnityEngine;
using System.Collections.Generic;

public class DamageBleedPickup : MonoBehaviour, IsConsumable, UsesCooldown
{
    // Script + Component Links
    PlayerController controller;
    SpriteRenderer renderer;
    Collider2D collider;
    CooldownTimer cooldownHandler;
    PlayerInputHandler inputHandler;

    // Interface Cast Variables
    public bool PickedUp { get => pickedUp; set => pickedUp = value; }
    private bool pickedUp = false;

    // Customizable Values
    public float damageBleedAmount;
    public float drainLength;
    public float pickupEffectLength;
    public GameObject bleedEffect;

    private void Awake()
    {
        // Grabs all linked scripts + components
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        cooldownHandler = GetComponent<CooldownTimer>();
        inputHandler = GameObject.Find("Player").GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        // Setup cooldown for how long boost lasts
        List<string> keyList = new List<string> { "pickupEffectLength" };
        List<float> lengthList = new List<float> { pickupEffectLength };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    public bool OnPickup()
    {
        // Tell player effect has begun
        inputHandler.SetCombatEffect("bleedPickup", damageBleedAmount, bleedEffect, drainLength);

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
        // Reset damage values to pre pickup state
        inputHandler.specialCombatEffectActive = false;
        // Destroy self
        Destroy(this.gameObject);
    }
}
