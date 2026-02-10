using UnityEngine;

public class ToxicDrain : MonoBehaviour, IsAbility
{
    // Script + Component Links
    GameData gameData;

    public GameObject toxicEffect;

    // Customizable Values
    public float drainDamage;
    public float drainDuration;

    private void Awake()
    {
        // Grabs all linked scripts + components
        gameData = GameObject.Find("GameHandler").GetComponent<GameData>();
    }

    public void OnActivation()
    {
        SpreadDrain();
    }

    public void SpreadDrain()
    {
        GameObject room = gameData.currentRoom;
        foreach (Transform child in room.transform)
        {
            if (child.gameObject.tag == "Enemy")
            {
                HPHandler enemyHPHandler = child.GetComponent<HPHandler>();
                enemyHPHandler.StartDrain(drainDamage, drainDuration, toxicEffect, "toxicEffect");
            }
        }
    }
}
