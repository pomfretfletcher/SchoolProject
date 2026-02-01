using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class AbilityHandler : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    CooldownTimer cooldownHandler;
    PlayerInputHandler playerInputHandler;
    Collider2D playerAbilityPickupRange;

    // Stores the player's current abilities - can also be assigned in inspector for testing
    public AbilityScript abilityOne;
    public AbilityScript abilityTwo;
    public AbilityScript abilityThree;

    private GameObject abilityToPickup;
    private ContactFilter2D abilityFilter;
    private Collider2D[] tempResults = new Collider2D[16];

    public void AddAbilities(object newAbility) { }
    public void ChangeAbilities(object newAbility, int slotToChange) { }
    public void RemoveAbilities(int slotToRemove) { }

    private void Awake()
    {
        // Grabs all linked scripts + components
        cooldownHandler = GetComponent<CooldownTimer>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
        playerAbilityPickupRange = transform.Find("AbilityPickupRange").GetComponent<Collider2D>();

        // Sets up filter for detecting player
        abilityFilter = new ContactFilter2D();
        abilityFilter.useLayerMask = true;
        abilityFilter.useTriggers = true;
        int abilityLayerIndex = LayerMask.NameToLayer("Abilities");
        abilityFilter.layerMask = 1 << abilityLayerIndex;
    }

    private void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "swapInterval" };
        List<float> lengthList = new List<float> { 1f // Interval player must wait between swapping abilities - prevents swapping back and forth if holding inputs
                                                   };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    private void FixedUpdate()
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
        if (abilityOne != null && cooldownHandler.timerStatusDict["abilityOneCooldown"] == 0 && playerInputHandler.CanUseAbilities)
        {
            cooldownHandler.timerStatusDict["abilityOneCooldown"] = 1;
            abilityOne.OnUse();
        }
    }
    public void UseAbilityTwo(InputAction.CallbackContext context)
    {
        if (abilityTwo != null && cooldownHandler.timerStatusDict["abilityTwoCooldown"] == 0 && playerInputHandler.CanUseAbilities)
        {
            cooldownHandler.timerStatusDict["abilityTwoCooldown"] = 1;
            abilityTwo.OnUse();
        }
    }
    public void UseAbilityThree(InputAction.CallbackContext context)
    {
        if (abilityThree != null && cooldownHandler.timerStatusDict["abilityThreeCooldown"] == 0 && playerInputHandler.CanUseAbilities)
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

    public void PickupAbility(InputAction.CallbackContext context)
    {
        // If an ability is in pickup range, and at least one slot is open, we can pickup the ability, if not, end this function
        if (abilityOne != null && abilityTwo != null && abilityThree != null)
        {
            return;
        }

        // Grabs the number of abilities we could pickup, these abilities are stored within temp results
        int abilitiesInRange = playerAbilityPickupRange.Overlap(abilityFilter, tempResults);

        // If there is an ability, grab its gameobject
        if (abilitiesInRange > 0)
        {
            List<Collider2D> results = new List<Collider2D>(abilitiesInRange);
            for (int i = 0; i < abilitiesInRange; i++)
            {
                results.Add(tempResults[i]);
            }
            abilityToPickup = results[0].transform.gameObject;
        }

        // If no ability, return
        if (abilitiesInRange == 0)
        {
            return;
        }

        // Checks each slot in order
        if (abilityOne == null)
        {
            abilityOne = abilityToPickup.GetComponent<AbilityScript>();
            abilityOne.SetToIconMode(1);
        }
        else if (abilityTwo == null)
        {
            abilityTwo = abilityToPickup.GetComponent<AbilityScript>();
            abilityTwo.SetToIconMode(2);
        }
        else if (abilityThree == null)
        {
            abilityThree = abilityToPickup.GetComponent<AbilityScript>();
            abilityThree.SetToIconMode(3);
        }
    }

    public void SwapAbilityOne(InputAction.CallbackContext context)
    {
        // Grabs the number of abilities we could pickup, these abilities are stored within temp results
        int abilitiesInRange = playerAbilityPickupRange.Overlap(abilityFilter, tempResults);

        // If there is an ability, grab its gameobject
        if (abilitiesInRange > 0)
        {
            List<Collider2D> results = new List<Collider2D>(abilitiesInRange);
            for (int i = 0; i < abilitiesInRange; i++)
            {
                results.Add(tempResults[i]);
            }
            abilityToPickup = results[0].transform.gameObject;
        }

        // If no ability, return
        if (abilitiesInRange == 0)
        {
            return;
        }

        //
        if (abilityOne == null || abilityTwo == null || abilityThree == null)
        {
            return;
        }

        else if (cooldownHandler.timerStatusDict["swapInterval"] == 0)
        {
            AbilityScript swappedAbility = abilityOne;
            swappedAbility.SetToConsumableMode(abilityToPickup.transform);
            abilityOne = abilityToPickup.GetComponent<AbilityScript>();
            abilityOne.SetToIconMode(1);
            cooldownHandler.timerStatusDict["swapInterval"] = 1;
        }
    }

    public void SwapAbilityTwo(InputAction.CallbackContext context)
    {
        // Grabs the number of abilities we could pickup, these abilities are stored within temp results
        int abilitiesInRange = playerAbilityPickupRange.Overlap(abilityFilter, tempResults);

        // If there is an ability, grab its gameobject
        if (abilitiesInRange > 0)
        {
            List<Collider2D> results = new List<Collider2D>(abilitiesInRange);
            for (int i = 0; i < abilitiesInRange; i++)
            {
                results.Add(tempResults[i]);
            }
            abilityToPickup = results[0].transform.gameObject;
        }

        // If no ability, return
        if (abilitiesInRange == 0)
        {
            return;
        }

        //
        if (abilityOne == null || abilityTwo == null || abilityThree == null)
        {
            return;
        }

        else if (cooldownHandler.timerStatusDict["swapInterval"] == 0)
        {
            AbilityScript swappedAbility = abilityTwo;
            swappedAbility.SetToConsumableMode(abilityToPickup.transform);
            abilityTwo = abilityToPickup.GetComponent<AbilityScript>();
            abilityTwo.SetToIconMode(2);
            cooldownHandler.timerStatusDict["swapInterval"] = 1;
        }
    }

    public void SwapAbilityThree(InputAction.CallbackContext context)
    {
        // Grabs the number of abilities we could pickup, these abilities are stored within temp results
        int abilitiesInRange = playerAbilityPickupRange.Overlap(abilityFilter, tempResults);

        // If there is an ability, grab its gameobject
        if (abilitiesInRange > 0)
        {
            List<Collider2D> results = new List<Collider2D>(abilitiesInRange);
            for (int i = 0; i < abilitiesInRange; i++)
            {
                results.Add(tempResults[i]);
            }
            abilityToPickup = results[0].transform.gameObject;
        }

        // If no ability, return
        if (abilitiesInRange == 0)
        {
            return;
        }

        //
        if (abilityOne == null || abilityTwo == null || abilityThree == null)
        {
            return;
        }

        else if (cooldownHandler.timerStatusDict["swapInterval"] == 0)
        {
            AbilityScript swappedAbility = abilityThree;
            swappedAbility.SetToConsumableMode(abilityToPickup.transform);
            abilityThree = abilityToPickup.GetComponent<AbilityScript>();
            abilityThree.SetToIconMode(3);
            cooldownHandler.timerStatusDict["swapInterval"] = 1;
        }
    }

    public void CooldownEndProcess(string key)
    {
        // Filler as no end processes in this script but needed for interface
    }
 }