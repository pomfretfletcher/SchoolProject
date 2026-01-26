using UnityEngine;

public class SpeedupPowerup : MonoBehaviour, IsConsumable
{
    // Script + Component Links
    PlayerController controller;

    // Interface Cast Variables
    public bool PickedUp { get => pickedUp; set => pickedUp = value; }

    // Customizable Values
    public float speedupAmount;
    public float speedupPercent;

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
        if (speedupAmount > 0)
        {
            controller.currentSpeed += speedupAmount;
        }
        // If a percentage value increase
        else
        {
            controller.currentSpeed *= (1 + speedupPercent);
        }

        // Tells the generic pickup script it has been picked up and to delete this object
        pickedUp = true;
        return true;
    }
}