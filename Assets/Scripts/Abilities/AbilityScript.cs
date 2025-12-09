using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    IsAbility specificAbilityScript;

    public float cooldown;
    public float timerProgression;

    void Awake()
    {
        specificAbilityScript = GetComponent<IsAbility>();
    }

    public void OnUse()
    {
        // Runs the specific abilities function
        specificAbilityScript.OnActivation();
    }
}
