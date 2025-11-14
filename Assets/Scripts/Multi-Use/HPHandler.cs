using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class HPHandler : MonoBehaviour
{
    // Script + Component Links
    UniversalController controller;
    Animator animator;

    void Awake()
    {
        // Grabs all linked scripts + components
        controller = GetComponent<UniversalController>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damageDealt) 
    {
        controller.CurrentHealth = controller.CurrentHealth - damageDealt;
        animator.SetTrigger("damaged");
        controller.IsInvulnerable = true;
    }

    public void HealDamage(int damageHealed) { }
    public void ResetHealth() { }
    public void StartDrain(int drainDamageAmount, int drainTimeLeft) { }
    public void ChangeDrainAmount(int newDrainDamageAmount, int newDrainTimeLeft) { }
    public void EndDrain() { }
}
