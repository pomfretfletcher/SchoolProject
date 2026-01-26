using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    // Script + Component Links
    IsAbility specificAbilityScript;
    Collider2D collider;
    AbilityGreyOut greyOutLogic;

    // Private internal logic variables
    public float cooldown;
    public float timerProgression;

    private bool inConsumableMode = false;
    private bool inIconMode = true;

    private void Awake()
    {
        // Grabs all linked scripts + components
        specificAbilityScript = GetComponent<IsAbility>();
        collider = GetComponent<Collider2D>();
        greyOutLogic = GetComponent<AbilityGreyOut>();
    }

    public void OnUse()
    {
        if (inIconMode)
        {
            // Runs the specific abilities function
            specificAbilityScript.OnActivation();
        }
    }

    public void SetToIconMode()
    {
        collider.enabled = false;
        greyOutLogic.enabled = true;
        inConsumableMode = false;
        inIconMode = true;
        // collider deactivate
        // bob deactivate
        // greyout activate
        // specific script activate
        // place in players slots
        // move to position in ui
    }

    public void SetToConsumableMode()
    {
        collider.enabled = true;
        greyOutLogic.enabled = false;
        inConsumableMode = true;
        inIconMode = false;
        // collider activate
        // bob activate
        // greyout dectivate
        // speciifc script deactivate
        // remove from player slots if in slots
        // move to either the place swapped ability was at or ability spawn point
    }
}