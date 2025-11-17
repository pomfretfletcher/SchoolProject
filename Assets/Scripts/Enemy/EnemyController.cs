using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour, UniversalController
{
    // Interface Cast Variables
    public int FullHealth { get => fullHealth; set => fullHealth = value; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int MeleeDamageAmount { get => meleeDamage; set => meleeDamage = value; }
    public bool IsInvulnerable { get => isInvulnerable; set => isInvulnerable = value; }

    // Health Variables
    public int fullHealth;
    public int currentHealth;

    // Speed Variables
    public int maxSpeed;
    public float currentSpeed;

    // Attack Variables
    public float attackCooldown;
    public int meleeDamage;
    public int rangedDamage;
    public float attackLockTime;

    // Waypoint variables
    public Transform nextWaypoint;
    public List<Transform> waypointList;

    // Player Reference Variables
    public int playerRequiredProximity;

    // Misc Variables
    public bool isInvulnerable = false;
    public float invulnerableOnHitTime;

    void Awake()
    {
        // Sets current values of variables to default values
        currentHealth = fullHealth;
    }
}
