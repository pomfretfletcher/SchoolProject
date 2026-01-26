using UnityEngine;

public class AbilityGreyOut : MonoBehaviour
{
    // Script + Component Links
    SpriteRenderer renderer;
    AbilityScript abilitySpecificScript;

    private void Awake()
    {
        // Grabs all linked scripts + components
        renderer = GetComponent<SpriteRenderer>();
        abilitySpecificScript = GetComponent<AbilityScript>();
    }

    private void Start()
    {
        // Sets initial value of timer progression to cooldown. This starts out the ability to be fully bright
        abilitySpecificScript.timerProgression = abilitySpecificScript.cooldown;
    }

    private void FixedUpdate()
    {
        // Calculate the percentage of how greyed out the icon will be and then apply that level of greyscale
        float percent = abilitySpecificScript.timerProgression / abilitySpecificScript.cooldown;
        renderer.color = new Color(percent, percent, percent);
    }
}