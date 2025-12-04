using UnityEngine;
using System.Collections.Generic;

public class GoblinCooldownHandler : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    CooldownTimer cooldownHandler;
    EnemyController controller;
    GoblinPathfinding pathfindingScript;

    void Awake()
    {
        // Grabs all linked scripts + components
        cooldownHandler = GetComponent<CooldownTimer>();
        controller = GetComponent<EnemyController>();
        pathfindingScript = GetComponent<GoblinPathfinding>();
    }

    void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "attackCooldown",
                                                  "cliffDetectionInterval",
                                                  "attackLockTime",
                                                  "invulnerableOnHitTime",
                                                  "deathDelay",
                                                  "runAwayTime" };
        List<float> lengthList = new List<float> { controller.attackCooldown,
                                                   controller.cliffDetectionInterval,
                                                   controller.attackLockTime,
                                                   controller.invulnerableOnHitTime,
                                                   controller.deathDelay,
                                                   controller.runAwayTime };
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
    }
}
