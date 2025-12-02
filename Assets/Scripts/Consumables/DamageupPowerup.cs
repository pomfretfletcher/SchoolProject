using UnityEngine;

public class DamageupPowerup : MonoBehaviour, IsConsumable
{
    // Script + Component Links
    PlayerController controller;

    // Amount the player's health will be increased by at pickup
    public float damageupAmount;
    public float damageupPercent;

    private void Awake()
    {
        // Grabs all linked scripts + components
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Called by pickup script when this pickup is collected
    public bool OnPickup()
    {
        Debug.Log((controller.currentMeleeDamage, controller.currentRangedDamage));
        if (damageupAmount > 0)
        {
            controller.currentMeleeDamage += damageupAmount;
            controller.currentRangedDamage += damageupAmount;
        }
        else
        {
            controller.currentMeleeDamage *= (1 + damageupPercent);
            controller.currentRangedDamage *= (1 + damageupPercent);
        }

        // Tells the generic pickup script to delete this object
        return true;
    }
}
