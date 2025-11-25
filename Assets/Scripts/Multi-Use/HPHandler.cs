using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class HPHandler : MonoBehaviour
{
    // Script + Component Links
    UniversalController controller;
    CooldownTimer cooldownHandler;
    Animator animator;

    void Awake()
    {
        // Grabs all linked scripts + components
        controller = GetComponent<UniversalController>();
        animator = GetComponent<Animator>();
        cooldownHandler = GetComponent<CooldownTimer>();
    }

    public void TakeDamage(int damageDealt) 
    {
        int lastHealth = controller.CurrentHealth;
        if (controller.IsInvulnerable) { return; }
        controller.CurrentHealth = controller.CurrentHealth - damageDealt;
        animator.SetTrigger("damaged");
        controller.IsInvulnerable = true;
        cooldownHandler.timerStatusDict["invulnerableOnHitTime"] = 1;

        if (controller.CurrentHealth <= 0 && lastHealth > 0)
        {
            animator.SetTrigger("dead");
            animator.SetBool("inDeathState", true);
        }
    }

    public void HealDamage(int damageHealed) { }
    public void ResetHealth() { }
    public void StartDrain(int drainDamageAmount, int drainTimeLeft) { }
    public void ChangeDrainAmount(int newDrainDamageAmount, int newDrainTimeLeft) { }
    public void EndDrain() { }
}
