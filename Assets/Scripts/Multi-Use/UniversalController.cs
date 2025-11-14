using UnityEngine;

public interface UniversalController
{
    int FullHealth { get; set; }
    int CurrentHealth { get; set; }
    int MeleeDamageAmount { get; set; }
    bool IsInvulnerable { get; set; }
}
