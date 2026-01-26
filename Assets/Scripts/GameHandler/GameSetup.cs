using UnityEngine;

public class GameSetup : MonoBehaviour
{
    // Script + Component Links
    GameData gameData;
    RoomHandling roomHandling;
    VisualAndSoundEffectHandling vsfxHandler;

    // Room player will start game in - assigned in inspector
    public GameObject startingRoom;

    // Music that will play in the background of the game - assigned in inspector
    public AudioClip gameMusic;

    private void Start()
    {
        gameData = GetComponent<GameData>();
        roomHandling = GetComponent<RoomHandling>();
        vsfxHandler = GetComponent<VisualAndSoundEffectHandling>();

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