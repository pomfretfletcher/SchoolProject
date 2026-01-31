using UnityEngine;

public class GameEndHandler : MonoBehaviour
{
    // Script + Component Links
    FadeScreenScript fadeAwayScreenScript;
    GameObject player;

    // Inspector assigned screen that is created when the game begins to end
    public GameObject fadeAwayScreen;

    private void Awake()
    {
        // Grabs all linked scripts + components
        player = GameObject.Find("Player");
    }

    public void EndGame()
    {
        // Begin screen fade to black
        // Spawn the screen that will fade to black before the end of game menu spawns
        Instantiate(fadeAwayScreen, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Quaternion.Euler(0, 0, 0));
    }
}
