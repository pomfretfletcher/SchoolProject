using UnityEngine;

public class ConsumableScript : MonoBehaviour
{
    // Script + Component Links
    IsConsumable consumableSpecificScript;
    VisualAndSoundEffectHandling vsfxHandler;

    // Audio clip that will be played when the consumable is collected - assigned in inspector
    public AudioClip pickupSound;

    private void Awake()
    {
        // Grabs all linked scripts + components
        consumableSpecificScript = GetComponent<IsConsumable>();
        vsfxHandler = GameObject.Find("GameHandler").GetComponent<VisualAndSoundEffectHandling>();
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

        // Play sound if avaiable and if picked up
        if (pickupSound != null && consumableSpecificScript.PickedUp == true) { vsfxHandler.PlaySound(pickupSound, 1f); }
    }
}
