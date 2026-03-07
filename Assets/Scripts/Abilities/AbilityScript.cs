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

    // Customizable Values
    public float cooldown;
    public float timerProgression;
    public string abilityName;
    public string abilityDesc;

    // Internal logic variables
    private bool inIconMode = true;

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
            gameData.abilitiesUsed++;
        }
    }

    public void SetToIconMode(int slot)
    {
        inIconMode = true;

        // Un-interactable in scene
        collider.enabled = false;
        greyOutLogic.enabled = true;

        // Turn off consumable features
        bobLogic.enabled = false;
        collider.enabled = false;

        // Set render layer to icon layer (above scene and player)
        iconRenderer.sortingLayerName = "AbilityIcon";
        iconFrameRenderer.sortingLayerName = "AbilityIcon";
        iconBackgroundRenderer.sortingLayerName = "AbilityIcon";

        // Remove from current parent (room)
        transform.SetParent(null, true); // keep current world position

        // Reset scale and rotation to UI values
        transform.localScale = Vector3.one * 0.2f; // icon scale
        transform.rotation = Quaternion.identity;

        // Assign to proper slot and put in ui location
        if (slot == 1)
        {
            // Parent to the UI slot
            transform.SetParent(slot1.transform, false);

            // Snap to slot position
            transform.localPosition = Vector3.zero;
        }
        else if (slot == 2)
        {
            // Parent to the UI slot
            transform.SetParent(slot2.transform, false);

            // Snap to slot position
            transform.localPosition = Vector3.zero;
        }
        else if (slot == 3)
        {
            // Parent to the UI slot
            transform.SetParent(slot3.transform, false);

            // Snap to slot position
            transform.localPosition = Vector3.zero;
        }
    }

    public void SetToConsumableMode(Vector3 consumableLocation, string context = "new")
    {
        inIconMode = false;

        // Interactable in scene
        collider.enabled = true;
        greyOutLogic.enabled = false;

        // Consumable features turned on
        bobLogic.enabled = true;
        collider.enabled = true;

        // Set render layer to consumables (in scene and below player)
        iconRenderer.sortingLayerName = "Consumables";
        iconFrameRenderer.sortingLayerName = "Consumables";
        iconBackgroundRenderer.sortingLayerName = "Consumables";

        transform.SetParent(null, true);

        transform.localScale = new Vector3(1, 1, 1);

        // Sets to correct room parent
        this.gameObject.transform.SetParent(gameData.currentRoom.transform, true);

        if (context == "swap")
        {
            transform.position = consumableLocation;
            bobLogic.startPos = consumableLocation;
        }
        else
        {
            transform.position = consumableLocation;
        }
    }
}