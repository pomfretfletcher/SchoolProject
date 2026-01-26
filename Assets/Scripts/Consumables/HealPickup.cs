using UnityEngine;

public class HealPickup : MonoBehaviour, IsConsumable
{
    // Script + Component Links
    PlayerController controller;
    HPHandler hpHandler;

    // Interface Cast Variables
    public bool PickedUp { get => pickedUp; set => pickedUp = value; }

    // Customizable Values
    public float healAmount;

    // Internal Logic Variables
    private bool pickedUp = false;

    private void Awake()
    {
        // Grabs all linked scripts + components
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
        hpHandler = GameObject.Find("Player").GetComponent<HPHandler>();
    }

    // Called by pickup script when this pickup is collected
    public bool OnPickup()
    {
        // Only picked up if the player has health to gain
        if (controller.currentHealth < controller.fullHealth)
        {
            hpHandler.HealDamage(healAmount);

            // Returns true if picked up
            pickedUp = true;
            return true;
        }
        else
        {
            // Returns false if not picked up
            return false;
        }
    }
}
