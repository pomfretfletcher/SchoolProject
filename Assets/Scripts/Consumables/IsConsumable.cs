using UnityEngine;

public interface IsConsumable
{
    // Interface Access Methods
    bool OnPickup();

    // Interface Access Variables
    bool PickedUp { get; set; }
}
