using UnityEngine;

public class EnemyAttackHandler : MonoBehaviour
{
    // Enemy Controller Link
    public EnemyController controller;

    // Damage Variables
    private int _meleeDamage;
    private int _rangedDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<EnemyController>();
        _meleeDamage = controller.meleeDamage;
        _rangedDamage = controller.rangedDamage;
    }

    // Update is called once per frame
    void Update()
    {
        // These variables change as the game goes on, therefore theyre also grabbed again every frame
        _meleeDamage = controller.meleeDamage;
        _rangedDamage = controller.rangedDamage;
    }

    public void CompleteMeleeAttack() { }
    public void CompleteRangedAttack() { }
}
