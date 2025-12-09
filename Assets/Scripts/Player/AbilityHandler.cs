using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class AbilityHandler : MonoBehaviour
{
    CooldownTimer cooldownHandler;

    // Ability Storage Variables
    public AbilityScript abilityOne;
    public AbilityScript abilityTwo;
    public AbilityScript abilityThree;

    public void AddAbilities(object newAbility) { }
    public void ChangeAbilities(object newAbility, int slotToChange) { }
    public void RemoveAbilities(int slotToRemove) { }

    void Awake()
    {
        cooldownHandler = GetComponent<CooldownTimer>();
    }

    void FixedUpdate()
    {
        // Updates each cooldown to keep in sync with the ability object
        if (abilityOne != null) { cooldownHandler.cooldownDict["abilityOneCooldown"] = abilityOne.cooldown; }
        if (abilityTwo != null) { cooldownHandler.cooldownDict["abilityTwoCooldown"] = abilityTwo.cooldown; }
        if (abilityThree != null) { cooldownHandler.cooldownDict["abilityThreeCooldown"] = abilityThree.cooldown; }

        if (cooldownHandler.timerStatusDict["abilityOneCooldown"] == 1) 
        {
            abilityOne.timerProgression = cooldownHandler.timerDict["abilityOneCooldown"];
        }
        if (cooldownHandler.timerStatusDict["abilityTwoCooldown"] == 1)
        {
            abilityTwo.timerProgression = cooldownHandler.timerDict["abilityTwoCooldown"];
        }
        if (cooldownHandler.timerStatusDict["abilityThreeCooldown"] == 1)
        {
            abilityThree.timerProgression = cooldownHandler.timerDict["abilityThreeCooldown"];
        }
    }

    public void UseAbilityOne(InputAction.CallbackContext context)
    {
        if (abilityOne != null && cooldownHandler.timerStatusDict["abilityOneCooldown"] == 0)
        {
            cooldownHandler.timerStatusDict["abilityOneCooldown"] = 1;
            abilityOne.OnUse();
        }
    }
    public void UseAbilityTwo(InputAction.CallbackContext context)
    {
        if (abilityTwo != null && cooldownHandler.timerStatusDict["abilityTwoCooldown"] == 0)
        {
            cooldownHandler.timerStatusDict["abilityTwoCooldown"] = 1;
            abilityTwo.OnUse();
        }
    }
    public void UseAbilityThree(InputAction.CallbackContext context)
    {
        if (abilityThree != null && cooldownHandler.timerStatusDict["abilityThreeCooldown"] == 0)
        {
            cooldownHandler.timerStatusDict["abilityThreeCooldown"] = 1;
            abilityThree.OnUse();
        }
    }

    public void SetTimerProgression(int abilityNo)
    {
        if (abilityNo == 1) { abilityOne.timerProgression = abilityOne.cooldown; }
        if (abilityNo == 2) { abilityTwo.timerProgression = abilityTwo.cooldown; }
        if (abilityNo == 3) { abilityThree.timerProgression = abilityThree.cooldown; }
    }
}
