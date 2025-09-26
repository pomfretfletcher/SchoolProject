using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    // Player Controller Link
    public PlayerController controller;

    // Cooldowns
    private int _jumpCooldown;
    private int _dodgeCooldown;
    private int _meleeAttackCooldown;
    private int _rangedAttackCooldown;

    // Ongoing Timers
    private float timeSinceJump;
    private float timeSinceDodge;
    private float timeSinceMeleeAttack;
    private float timeSinceRangedAttack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grabs variables from controller
        _jumpCooldown = controller.jumpCooldown;
        _dodgeCooldown = controller.dodgeCooldown;
        _meleeAttackCooldown = controller.meleeAttackCooldown;
        _rangedAttackCooldown = controller.rangedAttackCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MovePlayer() { }
    public void ExecutePlayerJump() { }
    public void ExecutePlayerDodge() { }
    public void ExecutePlayerAttack() { }
    public void ExecutePlayerAbility() { }
}
