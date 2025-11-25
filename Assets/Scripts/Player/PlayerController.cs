using UnityEngine;

public class PlayerController : MonoBehaviour, UniversalController
{
    // Interface Cast Variables
    public int FullHealth { get => fullHealth; set => fullHealth = value; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int MeleeDamageAmount { get => currentMeleeDamage; set => currentMeleeDamage = value; }
    public bool IsInvulnerable { get => isInvulnerable; set => isInvulnerable = value; }

    // Health Variables
    public int fullHealth;
    public int currentHealth;

    // Speed Variables
    public int maxSpeed;
    public float currentSpeed;

    // Melee Attack Variables
    public int minMeleeDamage;
    public int defaultMeleeDamage;
    public int currentMeleeDamage;
    public float meleeAttackCooldown;
    public float comboTime;

    // Ranged Attack Variables
    public int minRangedDamage;
    public int defaultRangedDamage;
    public int currentRangedDamage;
    public float rangedAttackCooldown;

    // Movement
    public float jumpCooldown;
    public float dodgeCooldown;
    public int jumpImpulse;
    public int dashImpulse;
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
