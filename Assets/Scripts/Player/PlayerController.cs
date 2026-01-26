using UnityEngine;

public class PlayerController : MonoBehaviour, UniversalController
{
    // Interface Cast Variables
    public float FullHealth { get => fullHealth; set => fullHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float MeleeDamageAmount { get => currentMeleeDamage; set => currentMeleeDamage = value; }
    public bool IsInvulnerable { get => isInvulnerable; set => isInvulnerable = value; }

    [Header("Health Variables")]
    public float fullHealth;
    public float currentHealth;

    [Header("Speed and Impulse Variables")]
    public float maxSpeed;
    public float currentSpeed;
    public float jumpImpulse;
    public float dashImpulse;

    [Header("Melee Damage Variables")]
    public float minMeleeDamage;
    public float defaultMeleeDamage;
    public float currentMeleeDamage;

    [Header("Ranged Damage Variables")]
    public float minRangedDamage;
    public float defaultRangedDamage;
    public float currentRangedDamage;

    [Header("Movement Cooldowns")]
    public float dodgeCooldown;
    public float dashLockTime;

    [Header("Attack Cooldowns and Delays")]
    public float meleeAttackCooldown;
    public float comboTime;
    public float rangedAttackCooldown;
    public float projectileFireDelay;

    [Header("Misc Variables")]
    public bool isInvulnerable = false;
    public float invulnerableOnHitTime;
    public float deathDelay;

    private void Start()
    {
        // Sets current values of variables to default values
        currentMeleeDamage = defaultMeleeDamage;
        currentRangedDamage = defaultRangedDamage;
        currentHealth = fullHealth;
    }
}