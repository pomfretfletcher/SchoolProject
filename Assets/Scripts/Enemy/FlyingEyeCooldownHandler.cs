using UnityEngine;
using System.Collections.Generic;

public class FlyingEyeCooldownHandler : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    CooldownTimer cooldownHandler;
    EnemyController controller;
    FlyingEyePathfinding pathfindingScript;

    private void Awake()
    {
        // Grabs all linked scripts + components
        cooldownHandler = GetComponent<CooldownTimer>();
        controller = GetComponent<EnemyController>();
        pathfindingScript = GetComponent<FlyingEyePathfinding>();
    }

    private void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "attackCooldown",
                                                  "attackLockTime",
                                                  "invulnerableOnHitTime",
                                                  "deathDelay",
                                                  "isAttacking",
                                                  "sufferingKnockback"};
        List<float> lengthList = new List<float> { controller.attackCooldown,
                                                   controller.attackLockTime,
                                                   controller.invulnerableOnHitTime,
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