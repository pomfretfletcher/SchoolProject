using UnityEngine;

public class PlayerController : MonoBehaviour, UniversalController
{
    // Interface Cast Variables
    public float FullHealth { get => fullHealth; set => fullHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float MeleeDamageAmount { get => currentMeleeDamage; set => currentMeleeDamage = value; }
    public bool IsInvulnerable { get => isInvulnerable; set => isInvulnerable = value; }

    // Health Variables
    public float fullHealth;
    public float currentHealth;

    // Speed Variables
    public float maxSpeed;
    public float currentSpeed;

    // Melee Attack Variables
    public float minMeleeDamage;
    public float defaultMeleeDamage;
    public float currentMeleeDamage;
    public float meleeAttackCooldown;
    public float comboTime;

    // Ranged Attack Variables
    public float minRangedDamage;
    public float defaultRangedDamage;
    public float currentRangedDamage;
    public float rangedAttackCooldown;
    public float projectileFireDelay;

    // Movement
    public float jumpCooldown;
    public float dodgeCooldown;
    public float jumpImpulse;
    public float dashImpulse;
    public float dashLockTime;

    // Misc Variables
    public bool isInvulnerable = false;
    public float invulnerableOnHitTime;

    private void Awake()
    {
        // Sets current values of variables to default values
        currentMeleeDamage = defaultMeleeDamage;
        currentRangedDamage = defaultRangedDamage;
        currentHealth = fullHealth;
    }
}
