using UnityEngine;

public class DamageupPowerup : MonoBehaviour, IsConsumable
{
    // Script + Component Links
    PlayerController controller;

    // Interface Cast Variables
    public bool PickedUp { get => pickedUp; set => pickedUp = value; }

    // Cusytomizable Values
    public float damageupAmount;
    public float damageupPercent;

    // Internal Logic Variables
    private bool pickedUp = false;

    private void Awake()
    {
        // Grabs all linked scripts + components
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Called by pickup script when this pickup is collected
    public bool OnPickup()
    {
        // If a set value increase
        if (damageupAmount > 0)
        {
            controller.currentMeleeDamage += damageupAmount;
            controller.currentRangedDamage += damageupAmount;
            controller.prePickupCurrentMeleeDamage += damageupAmount;
            controller.prePickupCurrentRangedDamage += damageupAmount;
        }
        // If a percentage value increase
        else
        {
            controller.currentMeleeDamage *= (1 + damageupPercent);
            controller.currentRangedDamage *= (1 + damageupPercent);
            controller.prePickupCurrentMeleeDamage *= (1 + damageupPercent);
            controller.prePickupCurrentRangedDamage *= (1 + damageupPercent);
        }

        // Tells the generic pickup script that has been picked up and to delete this object
        pickedUp = true;
        return true;
    }
}
