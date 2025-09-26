using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, UniversalController
{
    // Health Variables
    public int fullHealth;
    public int currentHealth;
    // These following variables are used with the universal controller script to allow the hp handler script to work for either the player or enemy controller
    public int _fullHealth => fullHealth;
    public int _currentHealth => currentHealth;

    // Speed Variables
    public int maxSpeed;
    public float currentSpeed;

    // Attack Variables
    public int attackCooldown;
    public int meleeDamage;
    public int rangedDamage;
    
    // Waypoint variables
    public Transform nextWaypoint;
    public List<Transform> waypointList;

    // Player Reference Variables
    public int playerRequiredProximity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
