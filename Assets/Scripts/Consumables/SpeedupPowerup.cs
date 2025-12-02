using UnityEngine;

public class SpeedupPowerup : MonoBehaviour, IsConsumable
{
    // Script + Component Links
    PlayerController controller;

    //
    public float speedupAmount;
    public float speedupPercent;

    private void Awake()
    {
        // Grabs all linked scripts + components
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Called by pickup script when this pickup is collected
    public bool OnPickup()
    {
        if (speedupAmount > 0)
        {
            controller.currentSpeed += speedupAmount;
        }
        else
        {
            controller.currentSpeed *= (1 + speedupPercent);
        }

        // Tells the generic pickup script to delete this object
        return true;
    }
}
