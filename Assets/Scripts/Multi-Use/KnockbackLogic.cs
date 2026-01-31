using UnityEngine;

public class KnockbackLogic : MonoBehaviour
{
    LogicScript entityLogicScript;
    Rigidbody2D rigidbody;
    CooldownTimer cooldownHandler;

    private void Awake()
    {
        entityLogicScript = GetComponent<LogicScript>();
        rigidbody = GetComponent<Rigidbody2D>();
        cooldownHandler = GetComponent<CooldownTimer>();
    }

    public void ExperienceKnockback(Vector2 deliveredKnockback)
    {

        Debug.Log(deliveredKnockback);
        // Reset current velocity
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(deliveredKnockback, ForceMode2D.Impulse);

        entityLogicScript.IsSufferingKnockback = true;
        cooldownHandler.timerStatusDict["sufferingKnockback"] = 1;
    }
}
