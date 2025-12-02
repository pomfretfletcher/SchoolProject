using UnityEngine;

public class HealthupPowerup : MonoBehaviour, IsConsumable
{
    // Script + Component Links
    PlayerController controller;

    // Amount the player's health will be increased by at pickup
    public float healthupAmount;
    public float healthupPercent;

    private void Awake()
    {
        // Grabs all linked scripts + components
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Called by pickup script when this pickup is collected
    public bool OnPickup()
    {
        if (healthupAmount > 0)
        {
            controller.currentHealth += healthupAmount;
            controller.fullHealth += healthupAmount;
        }
        else
        {
            controller.currentHealth *= (1 + healthupPercent);
            controller.fullHealth *= (1 + healthupPercent);
        }

        // Tells the generic pickup script to delete this object
        return true;
    }
}
