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

    [Header("Waypoint and Cliff Detection Variables")]
    public Transform nextWaypoint;
    public List<Transform> waypointList;
    public int cliffDetectionInterval;

    [Header("Player Reference Variables")]
    public int playerRequiredProximity;
    public int trackButNotMoveProximity;
    public int runAwayTrackingProximity;
    public float runAwayTime;

    [Header("Invulnerability Variables")]
    public bool isInvulnerable = false;
    public float invulnerableOnHitTime;

    void Start()
    {
        // Sets current values of variables to default values
        currentHealth = fullHealth;
    }
}
