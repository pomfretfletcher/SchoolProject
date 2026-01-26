using UnityEngine;

public interface UniversalController
{
    // Interface Cast Variables
    float FullHealth { get; set; }
    float CurrentHealth { get; set; }
    float MeleeDamageAmount { get; set; }
    bool IsInvulnerable { get; set; }
}