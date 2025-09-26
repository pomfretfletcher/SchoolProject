using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    // Ability Storage Variables
    public object abilityOne;
    public object abilityTwo;
    public object abilityThree;

    // Cooldown storage of stored abilities
    [SerializeField]
    private int abilityOneCooldown;
    [SerializeField]
    private int abilityTwoCooldown;
    [SerializeField]
    private int abilityThreeCooldown;

    // Ongoing Timers
    private float timeSinceAbilityOne;
    private float timeSinceAbilityTwo;
    private float timeSinceAbilityThree;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAbilities(object newAbility) { }
    public void ChangeAbilities(object newAbility, int slotToChange) { }
    public void RemoveAbilities(int slotToRemove) { }
}
