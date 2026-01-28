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
        collider.enabled = false;
        greyOutLogic.enabled = true;
        inConsumableMode = false;
        inIconMode = true;
        bobLogic.enabled = false;
        iconRenderer.sortingLayerName = "AbilityIcon";
        iconFrameRenderer.sortingLayerName = "AbilityIcon";
        iconBackgroundRenderer.sortingLayerName = "AbilityIcon";
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
        // collider deactivate
        // bob deactivate
        // greyout activate
        // specific script activate
        // place in players slots
        // move to position in ui
    }

    public void SetToConsumableMode(Transform consumableLocation)
    {
        collider.enabled = true;
        greyOutLogic.enabled = false;
        inConsumableMode = true;
        inIconMode = false;
        bobLogic.enabled = true;
        iconRenderer.sortingLayerName = "Consumables";
        iconFrameRenderer.sortingLayerName = "Consumables";
        iconBackgroundRenderer.sortingLayerName = "Consumables";
        transform.position = consumableLocation.position;
        // collider activate
        // bob activate
        // greyout dectivate
        // speciifc script deactivate
        // remove from player slots if in slots
        // move to either the place swapped ability was at or ability spawn point
    }
}