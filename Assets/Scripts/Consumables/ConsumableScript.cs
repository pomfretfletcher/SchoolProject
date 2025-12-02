using UnityEngine;

public class ConsumableScript : MonoBehaviour
{
    // Script + Component Links
    IsConsumable consumableSpecificScript;

    void Awake()
    {
        // Grabs all linked scripts + components
        consumableSpecificScript = GetComponent<IsConsumable>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tells the pickup specific script that it has been picked up
        bool needToDelete = consumableSpecificScript.OnPickup();
        // For instantaenous pickup, we need to delete instantly, for timed buffs, it will handle deletion independetnly 
        if (needToDelete)
        {
            // Destroy self
            Destroy(this.gameObject);
        }
    }
}
