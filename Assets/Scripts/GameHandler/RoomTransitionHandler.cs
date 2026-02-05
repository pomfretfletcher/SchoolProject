using UnityEngine;

public class RoomTransitionHandler : MonoBehaviour
{
    // Script + Component Links
    GameObject player;

    // Inspector assigned screen that is created when entering a new room
    public GameObject fadeScreen;

    public void BeginFade(string transitionType)
    {
        player = GameObject.Find("Player");

        if (transitionType == "room")
        {
            // Spawn the screen that will fade the player in when game begins
            Instantiate(fadeScreen, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Quaternion.Euler(0, 0, 0));
        }
        else if (transitionType == "startroom")
        {
            // Spawn the screen that will fade the player in when game begins
            Instantiate(fadeScreen, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        }
    }
}
