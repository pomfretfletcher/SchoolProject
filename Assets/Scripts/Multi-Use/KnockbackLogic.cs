using UnityEngine;

public class KnockbackLogic : MonoBehaviour
{
    LogicScript entityLogicScript;
    UniversalController entityController;
    Rigidbody2D rigidbody;
    CooldownTimer cooldownHandler;

    private void Awake()
    {
        entityLogicScript = GetComponent<LogicScript>();
        entityController = GetComponent<UniversalController>();
        rigidbody = GetComponent<Rigidbody2D>();
        cooldownHandler = GetComponent<CooldownTimer>();
    }

    public void ExperienceKnockback(Vector2 deliveredKnockback)
    {
        if (entityController.CurrentHealth <= 0) { return; }
        // Reset current velocity
        rigidbody.linearVelocity = Vector2.zero;
        rigidbody.AddForce(deliveredKnockback, ForceMode2D.Impulse);

        entityLogicScript.IsSufferingKnockback = true;
        cooldownHandler.timerStatusDict["sufferingKnockback"] = 1;
    }
}
