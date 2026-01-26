using UnityEngine;

public class HealthupPowerup : MonoBehaviour, IsConsumable
{
    // Script + Component Links
    PlayerController controller;

    // Interface Cast Variables
    public bool PickedUp { get => pickedUp; set => pickedUp = value; }

    // Customizable Values
    public float healthupAmount;
    public float healthupPercent;

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
        if (healthupAmount > 0)
        {
            controller.currentHealth += healthupAmount;
            controller.fullHealth += healthupAmount;
        }
        // If a percentage value increase
        else
        {
            controller.currentHealth *= (1 + healthupPercent);
            controller.fullHealth *= (1 + healthupPercent);
        }

        // Tells the generic pickup script that has been picked up and to delete this object
        pickedUp = true;
        return true;
    }
}
