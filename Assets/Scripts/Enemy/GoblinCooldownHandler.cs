using UnityEngine;
using System.Collections.Generic;

public class GoblinCooldownHandler : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    CooldownTimer cooldownHandler;
    EnemyController controller;
    GoblinPathfinding pathfindingScript;

    private void Awake()
    {
        // Grabs all linked scripts + components
        cooldownHandler = GetComponent<CooldownTimer>();
        controller = GetComponent<EnemyController>();
        pathfindingScript = GetComponent<GoblinPathfinding>();
    }

    private void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "attackCooldown",
                                                  "cliffDetectionInterval",
                                                  "attackLockTime",
                                                  "invulnerableOnHitTime",
                                                  "deathDelay",
                                                  "runAwayTime",
                                                  "isAttacking",
                                                  "sufferingKnockback"};
        List<float> lengthList = new List<float> { controller.attackCooldown,
                                                   controller.cliffDetectionInterval,
                                                   controller.attackLockTime,
                                                   controller.invulnerableOnHitTime,
                                                   controller.deathDelay,
                                                   controller.runAwayTime,
                                                   1f, // Filler for approx how long attack lasts
                                                   1.5f // Filler for how long affected by knockback
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
        if (key == "deathDelay")
        {
            Destroy(this.gameObject);
        }
        if (key == "runAwayTime")
        {
            pathfindingScript.RunAwayTracking = false;
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