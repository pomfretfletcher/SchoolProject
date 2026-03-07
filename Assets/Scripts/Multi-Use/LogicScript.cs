using UnityEngine;

public interface LogicScript
{
    // Interface Cast Variables
    bool IsSufferingKnockback { get; set; }
    int LookDirection { get; set; }
    bool CanMove { get; set; }
    bool CanAttack { get; set; }
    int MoveDirection {  get; set; }
    bool IsMoving { get; set; }
}