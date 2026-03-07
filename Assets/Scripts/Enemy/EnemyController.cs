using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour, UniversalController
{
    // Interface Cast Variables
    public float FullHealth { get => fullHealth; set => fullHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float MeleeDamageAmount { get => meleeDamage; set => meleeDamage = value; }
    public bool IsInvulnerable { get => isInvulnerable; set => isInvulnerable = value; }

    [Header("Health Variables")]
    public float fullHealth;
    public float currentHealth;

    [Header("Speed Variables")]
    public float maxSpeed;
    public float currentSpeed;

    [Header("Damage Variables")]
    public float meleeDamage;
    public float rangedDamage;

    [Header("Attack Cooldowns and Delays")]
    public float attackLockTime;
    public float projectileFireDelay;
    public float attackCooldown;

    [Header("Waypoint and Detection Variables")]
    public Transform nextWaypoint;
    public List<Transform> waypointList;
    public int waypointIndex = 0;
    public float cliffDetectionInterval;
    public float wallDetectionInterval;

    [Header("Player Reference Variables")]
    public float playerRequiredProximity;
    public float trackButNotMoveProximity;
    public float runAwayTrackingProximity;
    public float runAwayTime;
    public float trackIntervalTime;

    [Header("Misc Variables")]
    public bool isInvulnerable = false;
    public float invulnerableOnHitTime;
    public float deathDelay;
    private float ownScale;
    public bool isFlyingEnemy;

    private void Start()
    {
        // Sets current values of variables to default values
        currentHealth = fullHealth;
    }

    public void DifficultyScale(float scale)
    {
        ownScale = scale;
        // Alter variables relative to the given difficulty scale
        meleeDamage *= scale;
        rangedDamage *= scale;
        currentSpeed *= scale;
        fullHealth *= scale;
        currentHealth *= scale;
    }

    public void AlterScale(float newScale)
    {
        meleeDamage *= newScale / ownScale;
        rangedDamage *= newScale / ownScale;
        currentSpeed *= newScale / ownScale;
        fullHealth *= newScale / ownScale;
        currentHealth *= newScale / ownScale;

        ownScale = newScale;
    }

    public void SetWaypoints(Transform spawnNode)
    {
        waypointList.Add(spawnNode.Find("FlyingNodeOne"));
        waypointList.Add(spawnNode.Find("FlyingNodeTwo"));
        waypointList.Add(spawnNode.Find("FlyingNodeThree"));

        nextWaypoint = waypointList[waypointIndex];
    }
}