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

    private void Awake()
    {
        // Grabs all linked scripts + components
        cooldownHandler = GetComponent<CooldownTimer>();
        controller = GetComponent<PlayerController>();
        projectileLauncher = GetComponent<ProjectileLauncher>();
        inputHandler = GetComponent<PlayerInputHandler>();
        abilityHandler = GetComponent<AbilityHandler>();
    }

    private void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "dodgeCooldown",
                                                  "dashLockTime",
                                                  "meleeAttackCooldown",
                                                  "rangedAttackCooldown",
                                                  "invulnerableOnHitTime",
                                                  "projectileFireDelay",
                                                  "comboTime",
                                                  "deathDelay",
                                                  "abilityOneCooldown",
                                                  "abilityTwoCooldown",
                                                  "abilityThreeCooldown",
                                                  "isMeleeAttacking",
                                                  "isRangedAttacking",
                                                  "sufferingKnockback"
                                                  };
        List<float> lengthList = new List<float> { controller.dodgeCooldown,
                                                   controller.dashLockTime,
                                                   controller.meleeAttackCooldown,
                                                   controller.rangedAttackCooldown,
                                                   controller.invulnerableOnHitTime,
                                                   controller.projectileFireDelay,
                                                   controller.comboTime,
                                                   controller.deathDelay,
                                                   0,   // Filler for ability cooldowns
                                                   0,   // Filler for ability cooldowns
                                                   0,   // Filler for ability cooldowns
                                                   1,   // Filler for how long melee attack is going on
                                                   1.2f,// Filler for how long ranged attack is going on
                                                   1.5f // Filler for how long affected by knockback
                                                   };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    // Allows specific processes to be coded in to happen once a cooldown ends
    public void CooldownEndProcess(string key)
    {
        if (key == "dashLockTime")
        {
            inputHandler.IsDashing = false;
            if (!inputHandler.invulnerableFromAnotherSource)
            {
                controller.IsInvulnerable = false;
            }
        }
        if (key == "invulnerableOnHitTime")
        {
            // Only reset to invulnerable if not dashing which also keeps us invulnerable
            if (!inputHandler.IsDashing && !inputHandler.invulnerableFromAnotherSource)
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
            inputHandler.CanMove = false;
            inputHandler.CanAttack = false;
            inputHandler.CanUseAbilities = false;
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
        if (key == "isMeleeAttacking")
        {
            // Used for preventing melee or ranged attacking while in the other
            inputHandler.isMeleeAttacking = false;
            inputHandler.CanMove = true;
        }
        if (key == "isRangedAttacking")
        {
            // Used for preventing melee or ranged attacking while in the other
            inputHandler.isRangedAttacking = false;
            inputHandler.CanMove = true;
        }
        if (key == "sufferingKnockback")
        {
            inputHandler.IsSufferingKnockback = false;
        }
    }
}