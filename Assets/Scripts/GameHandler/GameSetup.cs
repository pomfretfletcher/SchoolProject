using UnityEngine;
using Unity.Cinemachine;

public class GameSetup : MonoBehaviour
{
    // Script + Component Links
    GameData gameData;
    RoomHandling roomHandling;
    VisualAndSoundEffectHandling vsfxHandler;

    public CinemachineCamera uiCamera;

    // Room player will start game in - assigned in inspector
    public GameObject startingRoom;

    // Music that will play in the background of the game - assigned in inspector
    public AudioClip gameMusic;

    public GameObject player;

    public GameObject healthBar;
    public float healthBarXOffset;
    public float healthBarYOffset;

    public void SetupGame()
    {
        // Grabs all linked scripts + components
        gameData = GetComponent<GameData>();
        roomHandling = GetComponent<RoomHandling>();
        vsfxHandler = GetComponent<VisualAndSoundEffectHandling>();

        // Creates an instance of the player and ensures it is named correctly
        GameObject playerInstance = Instantiate(player, Vector3.zero, Quaternion.identity);
        playerInstance.name = "Player";

        // Sets the cinemachine camera to follow the player
        uiCamera.Follow = playerInstance.transform;

        // Creates an instance of the health bar, sets it as a child to the ui and moves it to the correct position on the screen and ensures it is named correctly
        GameObject healthBarInstance = Instantiate(healthBar, Vector3.zero, Quaternion.identity);
        healthBarInstance.transform.parent = uiCamera.transform;
        healthBarInstance.transform.position = new Vector3(uiCamera.transform.position.x + healthBarXOffset, uiCamera.transform.position.y + healthBarYOffset, 10f);
        healthBarInstance.name = "HealthBar";

        GameObject runPartsFolder = GameObject.Find("RunParts");
        playerInstance.transform.parent = runPartsFolder.transform;

        // Sets up run
        SetupRoomStructure();
        SetupStartingRoom();

        // Start Music
        vsfxHandler.PlayMusic(gameMusic, 1f);
    }

    public void SetupRoomStructure()
    {
        // Create 5 empty rows
        for (int i = 0; i < 5; i++)
        {
            GameObjectList row = new GameObjectList();
            // Create 5 empty cells for the row
            for (int j = 0; j < 5; j++)
            {
                row.objects.Add(null);
            }
            gameData.roomStructure.Add(row);
        }
    }

    public void SetupStartingRoom()
    {
        GameObject setupRoom = Instantiate(startingRoom, Vector3.zero, Quaternion.identity);
        RoomCreation setupRoomCreator = setupRoom.GetComponent<RoomCreation>();
        setupRoomCreator.SetupRoom("filler", 0, null, 0, null, 0, null, 0, null, 0);

        // Places the setup room in the default starting position of the starting room
        roomHandling.PlaceRoomInStructure(2, 2, setupRoom);
    }
}