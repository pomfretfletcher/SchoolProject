using UnityEngine;

public class HealthupPickup : MonoBehaviour, IsPickup
{
    // Script + Component Links
    PlayerController controller;

    // Amount the player's health will be increased by at pickup
    public float healthupAmount;

    private void Awake()
    {
        // Grabs all linked scripts + components
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Called by pickup script when this pickup is collected
    public bool OnPickup()
    {
        // Increases both current and full health by the value
        controller.currentHealth += healthupAmount;
        controller.fullHealth += healthupAmount;

        // Tells the generic pickup script to delete this object
        return true;
    }
}
