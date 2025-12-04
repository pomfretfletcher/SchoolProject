using UnityEngine;
using System.Collections.Generic;

public class PlayerCooldownHandler : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    CooldownTimer cooldownHandler;
    PlayerController controller;
    ProjectileLauncher projectileLauncher; 
    PlayerInputHandler inputHandler;
    AbilityHandler abilityHandler;

    void Awake()
    {
        // Grabs all linked scripts + components
        cooldownHandler = GetComponent<CooldownTimer>();
        controller = GetComponent<PlayerController>();
        projectileLauncher = GetComponent<ProjectileLauncher>();
        inputHandler = GetComponent<PlayerInputHandler>();
        abilityHandler = GetComponent<AbilityHandler>();
    }

    void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "jumpCooldown",
                                                  "dodgeCooldown",
                                                  "dashLockTime",
                                                  "meleeAttackCooldown",
                                                  "rangedAttackCooldown",
                                                  "invulnerableOnHitTime",
                                                  "projectileFireDelay",
                                                  "comboTime",
                                                  "deathDelay",
                                                  "abilityOneCooldown",
                                                  "abilityTwoCooldown",
                                                  "abilityThreeCooldown" };
        List<float> lengthList = new List<float> { controller.jumpCooldown,
                                                   controller.dodgeCooldown,
                                                   controller.dashLockTime,
                                                   controller.meleeAttackCooldown,
                                                   controller.rangedAttackCooldown,
                                                   controller.invulnerableOnHitTime,
                                                   controller.projectileFireDelay,
                                                   controller.comboTime,
                                                   controller.deathDelay,
                                                   0,   // Filler for ability cooldowns
                                                   0,   // Filler for ability cooldowns
                                                   0    // Filler for ability cooldowns
                                                   };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    void FixedUpdate()
    {
        // Updates cooldown progress
        cooldownHandler.CheckCooldowns();
    }

    // Allows specific processes to be coded in to happen once a cooldown ends
    public void CooldownEndProcess(string key)
    {
        if (key == "dashLockTime")
        {
            inputHandler.IsDashing = false;
            controller.IsInvulnerable = false;
        }
        if (key == "invulnerableOnHitTime")
        {
            // Only reset to invulnerable if not dashing which also keeps us invulnerable
            if (!inputHandler.IsDashing)
            {
                controller.IsInvulnerable = false;
            }

        }
        if (key == "projectileFireDelay")
        {
            projectileLauncher.SpawnProjectile();
        }
        if (key == "deathDelay")
        {
            // Kill player
        }
        if (key == "abilityOneCooldown")
        {
            // Used for setting ability sprite to full brightness once timer finished
            abilityHandler.SetTimerProgression(1);
        }
        if (key == "abilityTwoCooldown")
        {
            // Used for setting ability sprite to full brightness once timer finished
            abilityHandler.SetTimerProgression(2);
        }
        if (key == "abilityThreeCooldown")
        {
            // Used for setting ability sprite to full brightness once timer finished
            abilityHandler.SetTimerProgression(3);
        }
    }
}
