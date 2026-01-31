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
    VisualAndSoundEffectHandling vsfxHandler;
    GameEndHandler gameEndHandler;

    // Sound played when entity hit - assigned in inspector
    public AudioClip hitSound;

    private void Awake()
    {
        // Grabs all linked scripts + components
        controller = GetComponent<UniversalController>();
        logicScript = GetComponent<LogicScript>();
        animator = GetComponent<Animator>();
        cooldownHandler = GetComponent<CooldownTimer>();
        vsfxHandler = GameObject.Find("GameHandler").GetComponent<VisualAndSoundEffectHandling>();
        gameEndHandler = GameObject.Find("GameHandler").GetComponent<GameEndHandler>();
    }

    public void TakeDamage(float damageDealt) 
    {
        // Last health is used so the animator trigger to enter death state isn't repeatably called while the entity is in death state
        float lastHealth = controller.CurrentHealth;

        // Don't do damage if entity is invulnerable
        if (controller.IsInvulnerable) { return; }

        // Take damage and tell animator damage has been taken
        controller.CurrentHealth = controller.CurrentHealth - damageDealt;
        animator.SetTrigger("damaged");

        // Start an invulnerability period to prevent attacks spamming damage dealt
        controller.IsInvulnerable = true;
        cooldownHandler.timerStatusDict["invulnerableOnHitTime"] = 1;

        // If there is an assigned hit sound, it is played
        if (hitSound != null) { vsfxHandler.PlaySound(hitSound, 2f); }

        // If just become dead
        if (controller.CurrentHealth <= 0 && lastHealth > 0)
        {
            // Tell animator entity is in death state
            animator.SetTrigger("dead");
            animator.SetBool("inDeathState", true);

            // Handle the death logic based on entity type
            if (gameObject.tag == "Enemy")
            {
                cooldownHandler.timerStatusDict["deathDelay"] = 1;
                logicScript.Deactivate();
            }
            else if (gameObject.tag == "Player")
            {
                cooldownHandler.timerStatusDict["deathDelay"] = 1;
                gameEndHandler.EndGame();
            }
        }
    }

    public void HealDamage(float damageHealed) 
    {
        // Gain health
        controller.CurrentHealth += damageHealed;

        // Limit health to maximum health
        if (controller.CurrentHealth > controller.FullHealth)
        {
            controller.CurrentHealth = controller.FullHealth;   
        }
    }

    public void ResetHealth() 
    {
        controller.CurrentHealth = controller.FullHealth;
    }

    public void StartDrain(int drainDamageAmount, int drainTimeLeft) { }
    public void ChangeDrainAmount(int newDrainDamageAmount, int newDrainTimeLeft) { }
    public void EndDrain() { }
}