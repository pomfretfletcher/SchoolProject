using UnityEngine;
using System.Collections.Generic;

public class KnightCooldownHandler : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    CooldownTimer cooldownHandler;
    EnemyController controller;
    KnightPathfinding pathfindingScript;

    void Awake()
    {
        // Grabs all linked scripts + components
        cooldownHandler = GetComponent<CooldownTimer>();
        controller = GetComponent<EnemyController>();
        pathfindingScript = GetComponent<KnightPathfinding>();
    }

    void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "attackCooldown",
                                                  "cliffDetectionInterval",
                                                  "attackLockTime",
                                                  "invulnerableOnHitTime",
                                                  "deathDelay"};
        List<float> lengthList = new List<float> { controller.attackCooldown,
                                                   controller.cliffDetectionInterval,
                                                   controller.attackLockTime,
                                                   controller.invulnerableOnHitTime,
                                                   controller.deathDelay };
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
    }
}
