using UnityEngine;

public class DifficultyPurgePowerup : MonoBehaviour, IsConsumable
{
    // Script + Component Links
    GameData gameData;

    // Interface Cast Variables
    public bool PickedUp { get => pickedUp; set => pickedUp = value; }

    // Customizable Values
    public float purgeAmount;

    // Internal Logic Variables
    private bool pickedUp = false;

    private void Awake()
    {
        // Grabs all linked scripts + components
        gameData = GameObject.Find("GameHandler").GetComponent<GameData>();
    }

    public bool OnPickup()
    {
        // Reduce global difficulty scale
        gameData.ReduceDifficultyScale(purgeAmount);

        // Return true to be deleted
        pickedUp = true;
        return true;
    }
}
