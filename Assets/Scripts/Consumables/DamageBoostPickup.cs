using UnityEngine;
using System.Collections.Generic;

public class DamageBoostPickup : MonoBehaviour, IsConsumable, UsesCooldown
{
    // Script + Component Links
    PlayerController controller;
    SpriteRenderer renderer;
    Collider2D collider;
    CooldownTimer cooldownHandler;

    // Interface Cast Variables
    public bool PickedUp { get => pickedUp; set => pickedUp = value; }
    private bool pickedUp = false;

    // Customizable Values
    public float damageBoostPercentage;
    public float damageBoostLength;

    // Internal Logic Variables
    private float _prevMeleeDamage;
    private float _prevRangedDamage;

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
        List<string> keyList = new List<string> { "damageBoostLength" };
        List<float> lengthList = new List<float> { damageBoostLength };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    // Called by pickup script when this pickup is collected
    public bool OnPickup()
    {
        // Stores initial damage variables to reset to after buff finishes
        _prevMeleeDamage = controller.currentMeleeDamage;
        _prevRangedDamage = controller.currentRangedDamage;

        // Increase player damages
        controller.currentMeleeDamage *= (1 + damageBoostPercentage);
        controller.currentRangedDamage *= (1 + damageBoostPercentage);

        // Make sprite invisible
        renderer.enabled = false;
        collider.enabled = false;

        // Starts timer for how long effect will last
        cooldownHandler.timerStatusDict["damageBoostLength"] = 1;
        
        // Tells general pickup script that it has been picked up and to not delete the pickup
        pickedUp = true;
        return false;
    }

    public void CooldownEndProcess(string key)
    {
        // Reset damage values
        controller.currentMeleeDamage = _prevMeleeDamage;
        controller.currentRangedDamage = _prevRangedDamage;
        // Destroy self
        Destroy(this.gameObject);
    }
}
