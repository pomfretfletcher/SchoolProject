using UnityEngine;

public class RoomTransitionHandler : MonoBehaviour
{
    FadeScreenScript screenScript;
    public GameObject fadeScreen;
    GameObject player;

    private void Awake()
    {
        // Grabs all linked scripts + components
        player = GameObject.Find("Player");
    }

    public void BeginFade(string transitionType)
    {
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
