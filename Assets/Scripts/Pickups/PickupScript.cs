using UnityEngine;

public class PickupScript : MonoBehaviour
{
    // Script + Component Links
    IsPickup pickupSpecificScript;

    void Awake()
    {
        // Grabs all linked scripts + components
        pickupSpecificScript = GetComponent<IsPickup>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tells the pickup specific script that it has been picked up
        bool needToDelete = pickupSpecificScript.OnPickup();
        // For instantaenous pickup, we need to delete instantly, for timed buffs, it will handle deletion independetnly 
        if (needToDelete)
        {
            // Destroy self
            Destroy(this.gameObject);
        }
    }
}
