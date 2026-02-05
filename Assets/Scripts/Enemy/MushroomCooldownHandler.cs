using UnityEngine;
using System.Collections.Generic;

public class MushroomCooldownHandler : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    CooldownTimer cooldownHandler;
    EnemyController controller;
    ProjectileLauncher projectileLauncher;
    MushroomPathfinding pathfindingScript;

    private void Awake()
    {
        // Grabs all linked scripts + components
        cooldownHandler = GetComponent<CooldownTimer>();
        controller = GetComponent<EnemyController>();
        projectileLauncher = GetComponent<ProjectileLauncher>();
        pathfindingScript = GetComponent<MushroomPathfinding>();
    }

    private void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "attackCooldown",
                                                  "cliffDetectionInterval",
                                                  "attackLockTime",
                                                  "invulnerableOnHitTime",
                                                  "runAwayTime",
                                                  "projectileFireDelay",
                                                  "deathDelay",
                                                  "isAttacking",
                                                  "sufferingKnockback"
                                                   };
        List<float> lengthList = new List<float> { controller.attackCooldown,
                                                   controller.cliffDetectionInterval,
                                                   controller.attackLockTime,
                                                   controller.invulnerableOnHitTime,
                                                   controller.runAwayTime,
                                                   controller.projectileFireDelay,
                                                   controller.deathDelay,
                                                   1f, // Filler for approx how long attack lasts
                                                   0.8f // Filler for how long affected by knockback
                                                   };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    // Allows specific processes to be coded in to happen once a cooldown ends
    public void CooldownEndProcess(string key)
    {
        if (key == "attackLockTime")
        {
            pathfindingScript.CanMove = true;
        }
        if (key == "invulnerableOnHitTime")
        {
            controller.IsInvulnerable = false;
        }
        if (key == "runAwayTime")
        {
            pathfindingScript.RunAwayTracking = false;
        }
        if (key == "projectileFireDelay")
        {
            projectileLauncher.SpawnProjectile();
        }
        if (key == "deathDelay")
        {
            Destroy(this.gameObject);
        }
        if (key == "isAttacking")
        {
            pathfindingScript.isAttacking = false;
            pathfindingScript.CanMove = true;
        }
        if (key == "sufferingKnockback")
        {
            pathfindingScript.IsSufferingKnockback = false;
        }
    }
}