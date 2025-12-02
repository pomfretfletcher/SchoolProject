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

    // Health Variables
    public float fullHealth;
    public float currentHealth;

    // Speed Variables
    public float maxSpeed;
    public float currentSpeed;

    // Attack Variables
    public float attackCooldown;
    public float meleeDamage;
    public float rangedDamage;
    public float attackLockTime;

    // Waypoint Variables
    public Transform nextWaypoint;
    public List<Transform> waypointList;

    // Cliff Detection Zone Variables
    public int cliffDetectionInterval;

    // Player Reference Variables
    public int playerRequiredProximity;
    public int trackButNotMoveProximity;
    public int runAwayTrackingProximity;
    public float runAwayTime;

    // Misc Variables
    public bool isInvulnerable = false;
    public float invulnerableOnHitTime;

    void Start()
    {
        // Sets current values of variables to default values
        currentHealth = fullHealth;
    }
}
