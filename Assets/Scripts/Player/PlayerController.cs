using UnityEngine;

public class PlayerController : MonoBehaviour, UniversalController
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

    // Melee Attack Variables
    public int minMeleeDamage;
    public int defaultMeleeDamage;
    public int currentMeleeDamage;
    public int meleeAttackCooldown;

    // Ranged Attack Variables
    public int minRangedDamage;
    public int defaultRangedDamage;
    public int currentRangedDamage;
    public int rangedAttackCooldown;

    // Movement
    public int jumpCooldown;
    public int dodgeCooldown;
    public int jumpImpulse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
