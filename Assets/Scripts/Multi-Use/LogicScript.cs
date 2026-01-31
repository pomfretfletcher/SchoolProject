using UnityEngine;

public interface LogicScript
{
    // Interface Cast Methods
    void Deactivate();

    bool IsSufferingKnockback { get; set; }
}