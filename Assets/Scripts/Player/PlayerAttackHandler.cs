using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    // Player Controller Link
    public PlayerController controller;

    // Melee Damage Variables
    private int _minMeleeDamage;
    private int _defaultMeleeDamage;
    private int _currentMeleeDamage;

    // Ranged Damage Variables
    private int _minRangedDamage;
    private int _defaultRangedDamage;
    private int _currentRangedDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grabs variables from player controller
        _minMeleeDamage = controller.minMeleeDamage;
        _defaultMeleeDamage = controller.defaultMeleeDamage;
        _currentMeleeDamage = controller.currentMeleeDamage;
        _minRangedDamage = controller.minRangedDamage;
        _defaultRangedDamage = controller.defaultRangedDamage;
        _currentRangedDamage = controller.currentRangedDamage;
    }

    // Update is called once per frame
    void Update()
    {
        // These variables change as the game goes on, therefore theyre also grabbed again every frame
        _currentMeleeDamage = controller.currentMeleeDamage;
        _currentRangedDamage = controller.currentRangedDamage;
    }

    public void CompleteMeleeAttack() { }
    public void CompleteRangedAttack() { }
}
