using UnityEngine;
using System.Collections.Generic;

public class GameEndHandler : MonoBehaviour
{
    // Script + Component Links
    GameObject player;
    GameData gameData;

    // Inspector assigned screen that is created when the game begins to end
    public GameObject fadeAwayScreen;

    public void EndGame()
    {
        // Grabs all linked scripts + components
        player = GameObject.Find("Player");
        gameData = GetComponent<GameData>();

        // Begin screen fade to black
        // Spawn the screen that will fade to black before the end of game menu spawns
        GameObject gameoverFade = Instantiate(fadeAwayScreen, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Quaternion.Euler(0, 0, 0));
        gameoverFade.GetComponent<FadeScreenScript>().SetMode("Gameover");
    }

    public void DeleteRunParts()
    {
        player = GameObject.Find("Player");
        Destroy(player);

        GameObject healthBar = GameObject.Find("HealthBar");
        Destroy(healthBar);

        GameObject gameHandler = GameObject.Find("GameHandler");
        GameData gameData = gameHandler.GetComponent<GameData>();
        gameData.roomStructure = new List<GameObjectList>();

        GameObject roomsFolder = GameObject.Find("RunParts/Rooms");
        foreach (Transform room in roomsFolder.transform)
        {
            Destroy(room.gameObject);
        }
    }
}
