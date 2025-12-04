using UnityEngine;

public class AbilityGreyOut : MonoBehaviour
{
    SpriteRenderer renderer;
    AbilityScript abilitySpecificScript;

    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        abilitySpecificScript = GetComponent<AbilityScript>();
    }

    void Start()
    {
        // Sets initial value of timer progression to cooldown. This starts out the ability to be fully bright
        abilitySpecificScript.timerProgression = abilitySpecificScript.cooldown;
    }

    void FixedUpdate()
    {
        float percent = abilitySpecificScript.timerProgression / abilitySpecificScript.cooldown;
        renderer.color = new Color(percent, percent, percent);
    }
}
