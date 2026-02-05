using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    // Script + Component Links
    IsAbility specificAbilityScript;
    Collider2D collider;
    AbilityGreyOut greyOutLogic;
    ConsumableBob bobLogic;
    Transform iconFrame;
    Transform iconBackground;
    SpriteRenderer iconRenderer;
    SpriteRenderer iconFrameRenderer;
    SpriteRenderer iconBackgroundRenderer;
    GameData gameData;
    GameObject ui;
    Transform slot1;
    Transform slot2;
    Transform slot3;

    public float cooldown;
    public float timerProgression;
    private bool inConsumableMode = false;
    private bool inIconMode = true;

    public Transform currentConsumablePosition;

    private void Awake()
    {
        // Grabs all linked scripts + components
        specificAbilityScript = GetComponent<IsAbility>();
        collider = GetComponent<Collider2D>();
        greyOutLogic = GetComponent<AbilityGreyOut>();
        bobLogic = GetComponent<ConsumableBob>();
        gameData = GameObject.Find("GameHandler").GetComponent<GameData>();
        iconFrame = transform.Find("AbilityFrame");
        iconBackground = transform.Find("AbilityFrameBackground");
        iconRenderer = GetComponent<SpriteRenderer>();
        iconFrameRenderer = iconFrame.GetComponent<SpriteRenderer>();
        iconBackgroundRenderer = iconBackground.GetComponent<SpriteRenderer>();
        ui = GameObject.Find("UICamera");
        slot1 = ui.transform.Find("HealthBar").Find("Slot1");
        slot2 = ui.transform.Find("HealthBar").Find("Slot2");
        slot3 = ui.transform.Find("HealthBar").Find("Slot3");
    }

    public void OnUse()
    {
        if (inIconMode)
        {
            // Runs the specific abilities function
            specificAbilityScript.OnActivation();
        }
    }

    public void SetToIconMode(int slot)
    {
        inIconMode = true;
        inConsumableMode = false;

        // Un-interactable in scene
        collider.enabled = false;
        greyOutLogic.enabled = true;

        // Turn off consumable features
        bobLogic.enabled = false;

        // Set render layer to icon layer (above scene and player)
        iconRenderer.sortingLayerName = "AbilityIcon";
        iconFrameRenderer.sortingLayerName = "AbilityIcon";
        iconBackgroundRenderer.sortingLayerName = "AbilityIcon";

        // Assign to proper slot and put in ui location
        if (slot == 1)
        {
            transform.parent = slot1.transform;
            transform.position = slot1.position;
        }
        else if (slot == 2)
        {
            transform.parent = slot2.transform;
            transform.position = slot2.position;
        }
        else if (slot == 3)
        {
            transform.parent = slot3.transform;
            transform.position = slot3.position;
        }
    }

    public void SetToConsumableMode(Transform consumableLocation)
    {   
        inConsumableMode = true;
        inIconMode = false;

        // Interactable in scene
        collider.enabled = true;
        greyOutLogic.enabled = false;

        // Consumable features turned on
        bobLogic.enabled = true;

        // Set render layer to consumables (in scene and below player)
        iconRenderer.sortingLayerName = "Consumables";
        iconFrameRenderer.sortingLayerName = "Consumables";
        iconBackgroundRenderer.sortingLayerName = "Consumables";

        // Place in designated location (node for new abilities, previous abilities location if swapped)
        transform.position = consumableLocation.position;

        // Sets to correct room parent
        this.gameObject.transform.SetParent(gameData.currentRoom.transform, true);
    }
}