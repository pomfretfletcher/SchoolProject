using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class HPHandler : MonoBehaviour
{
    // Script + Component Links
    UniversalController controller;
    LogicScript logicScript;
    CooldownTimer cooldownHandler;
    Animator animator;

    void Awake()
    {
        // Grabs all linked scripts + components
        controller = GetComponent<UniversalController>();
        logicScript = GetComponent<LogicScript>();
        animator = GetComponent<Animator>();
        cooldownHandler = GetComponent<CooldownTimer>();
    }

    public void TakeDamage(float damageDealt) 
    {
        float lastHealth = controller.CurrentHealth;
        if (controller.IsInvulnerable) { return; }
        controller.CurrentHealth = controller.CurrentHealth - damageDealt;
        animator.SetTrigger("damaged");
        controller.IsInvulnerable = true;
        cooldownHandler.timerStatusDict["invulnerableOnHitTime"] = 1;

        if (controller.CurrentHealth <= 0 && lastHealth > 0)
        {
            animator.SetTrigger("dead");
            animator.SetBool("inDeathState", true);

            if (gameObject.tag == "Enemy")
            {
                cooldownHandler.timerStatusDict["deathDelay"] = 1;
                logicScript.Deactivate();
            }
        }
    }

    public void HealDamage(float damageHealed) 
    {
        controller.CurrentHealth += damageHealed;
        if (controller.CurrentHealth > controller.FullHealth)
        {
            controller.CurrentHealth = controller.FullHealth;   
        }
    }

    public void ResetHealth() { }
    public void StartDrain(int drainDamageAmount, int drainTimeLeft) { }
    public void ChangeDrainAmount(int newDrainDamageAmount, int newDrainTimeLeft) { }
    public void EndDrain() { }
}
