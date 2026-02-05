using UnityEngine;
using System.Collections.Generic;

public class RoomLogic : MonoBehaviour
{
    // Script + Component Links
    RoomData nextRoomData;
    GameObject nextRoom;
    public GameObject nextRoomForTesting;
    RoomData thisRoomData;
    GameObject player;
    GameData gameData;
    RoomHandling roomHandling;
    TeleportHandler teleHandler;
    RoomTransitionHandler transitionHandler;

    // Internal Logic Variables
    private int nextRoomPositionX = 0;
    private int nextRoomPositionY = 0;

    private void Awake()
    {
        // Grabs all linked scripts + components
        gameData = GameObject.Find("GameHandler").GetComponent<GameData>();
        roomHandling = GameObject.Find("GameHandler").GetComponent<RoomHandling>();
        thisRoomData = GetComponent<RoomData>();
        player = GameObject.Find("Player");
        teleHandler = GameObject.Find("GameHandler").GetComponent<TeleportHandler>();
        transitionHandler = GameObject.Find("GameHandler").GetComponent<RoomTransitionHandler>();
    }

    public void EnterStartingRoom()
    {
        // Grab the starting position of the player
        Transform playerSpawn = thisRoomData.playerSpawnPoint;

        // Update player is in room value
        thisRoomData.playerIsInRoom = true;

        transitionHandler.BeginFade("startroom");

        gameData.currentRoom = this.gameObject;
    }

    public void EnterNewRoom(string roomEnterDirection, GameObject nextRoom)
    {
        // Create player spawn variable, this will be immediately overwritten but it needs a value to be created with
        Transform playerSpawn = player.transform;

        PlayerInputHandler inputHandler = player.GetComponent<PlayerInputHandler>();
        inputHandler.movingThroughRooms = true;

        // Grab needed data
        nextRoomData = nextRoom.GetComponent<RoomData>();

        // Update player is in room values for this and the connecting room
        thisRoomData.playerIsInRoom = false;
        nextRoomData.playerIsInRoom = true;

        // Depending on the direction we are entering the room from, decide the position to spawn to
        if (roomEnterDirection == "left") { playerSpawn = nextRoomData.rightEntrancePlayerSpawnPoint; }
        else if (roomEnterDirection == "right") { playerSpawn = nextRoomData.leftEntrancePlayerSpawnPoint; }
        else if (roomEnterDirection == "top") { playerSpawn = nextRoomData.bottomEntrancePlayerSpawnPoint; }
        else if (roomEnterDirection == "bottom") { playerSpawn = nextRoomData.topEntrancePlayerSpawnPoint; }

        gameData.currentRoom = nextRoom.gameObject;

        // Teleport the player to the decided upon position
        teleHandler.OnPlayerTeleport(playerSpawn.position);
    }

    public void ChangeRoom(string direction)
    {
        bool connectionValid = false;
        nextRoom = GetNextRoom(direction);
        if (nextRoom != null)
        {
            nextRoomData = nextRoom.GetComponent<RoomData>();
            // Checks if the room is valid to move into from the current room
            if (direction == "left") { connectionValid = nextRoomData.hasRightEntrance; }
            else if (direction == "right") { connectionValid = nextRoomData.hasLeftEntrance; }
            else if (direction == "top") { connectionValid = nextRoomData.hasBottomEntrance; }
            else if (direction == "bottom") { connectionValid = nextRoomData.hasTopEntrance; }
        }
        // If can move into pre-existing room from current room
        if (connectionValid)
        {
            // Moves the position in the scene that the room the player is in is
            if (direction == "left") { roomHandling.currentXPosInScene -= 50; }
            else if (direction == "right") { roomHandling.currentXPosInScene += 50; }
            else if (direction == "top") { roomHandling.currentYPosInScene += 50; }
            else if (direction == "bottom") { roomHandling.currentYPosInScene -= 50; }

            EnterNewRoom(direction, nextRoom);
        }
        // If can't move into pre-existing room from current room, flag as error in console, this shouldn't happen
        else if (nextRoom != null && !connectionValid)
        {
            Debug.Log("Connecting room does not have a correct entrance.");
        }

        // If the place the player is trying to move into is a room to be created
        if (nextRoom == null)
        {
            // Choose new room - need to add
            bool roomChosen = false;
            // Load all prefabs in the folder room prefabs
            List<GameObject> prefabs = new List<GameObject>(Resources.LoadAll<GameObject>("Room Prefabs"));

            while (roomChosen == false)
            {
                // Pick one of the rooms in the folder at random
                GameObject chosenRoom = prefabs[Random.Range(0, prefabs.Count)];
                RoomData chosenRoomData = chosenRoom.GetComponent<RoomData>();

                // Checks if the room is valid to move into from the current room
                if (direction == "left") { connectionValid = chosenRoomData.hasRightEntrance; }
                else if (direction == "right") { connectionValid = chosenRoomData.hasLeftEntrance; }
                else if (direction == "top") { connectionValid = chosenRoomData.hasBottomEntrance; }
                else if (direction == "bottom") { connectionValid = chosenRoomData.hasTopEntrance; }

                if (connectionValid)
                {
                    connectionValid = CheckNewRoomFits(nextRoomPositionX, nextRoomPositionY, chosenRoomData);
                }

                // If we can enter through the right, we will use this room, if not, choose a new room
                if (connectionValid)
                {
                    // Moves the position in the scene that the room the player is in is
                    if (direction == "left") { roomHandling.currentXPosInScene -= 50; }
                    else if (direction == "right") { roomHandling.currentXPosInScene += 50; }
                    else if (direction == "top") { roomHandling.currentYPosInScene += 50; }
                    else if (direction == "bottom") { roomHandling.currentYPosInScene -= 50; }

                    roomChosen = true;
                    // Creates the chosen room
                    roomHandling.CreateRoom(chosenRoom, nextRoomPositionX, nextRoomPositionY, direction);
                    nextRoom = gameData.roomStructure[nextRoomPositionX].objects[nextRoomPositionY];
                    EnterNewRoom(direction, nextRoom);
                }
                else
                {
                    // If not valid to move into, it can't be selected to try to move into again this time
                    prefabs.Remove(chosenRoom);
                    // If empty, do a one exit room
                    if (prefabs.Count == 0)
                    {
                        // Load all prefabs in the folder room prefabs
                        List<GameObject> deadendPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>("DeadEnd Rooms"));
                        bool correctDeadEnd = false;

                        while (correctDeadEnd == false)
                        {
                            // Pick one of the rooms in the folder at random
                            GameObject chosenDeadEnd = deadendPrefabs[Random.Range(0, deadendPrefabs.Count)];
                            RoomData chosenDeadEndData = chosenDeadEnd.GetComponent<RoomData>();

                            // Checks if the room is valid to move into from the current room
                            if (direction == "left") { connectionValid = chosenDeadEndData.hasRightEntrance; }
                            else if (direction == "right") { connectionValid = chosenDeadEndData.hasLeftEntrance; }
                            else if (direction == "top") { connectionValid = chosenDeadEndData.hasBottomEntrance; }
                            else if (direction == "bottom") { connectionValid = chosenDeadEndData.hasTopEntrance; }

                            if (connectionValid)
                            {
                                correctDeadEnd = true;

                                // Moves the position in the scene that the room the player is in is
                                if (direction == "left") { roomHandling.currentXPosInScene -= 50; }
                                else if (direction == "right") { roomHandling.currentXPosInScene += 50; }
                                else if (direction == "top") { roomHandling.currentYPosInScene += 50; }
                                else if (direction == "bottom") { roomHandling.currentYPosInScene -= 50; }

                                // Creates the chosen room
                                roomHandling.CreateRoom(chosenDeadEnd, nextRoomPositionX, nextRoomPositionY, direction);
                                nextRoom = gameData.roomStructure[nextRoomPositionX].objects[nextRoomPositionY];
                                EnterNewRoom(direction, nextRoom);
                            }
                            else
                            {
                                prefabs.Remove(chosenDeadEnd);
                            }
                        }
                    }
                }
            }
        }
    }

    private GameObject GetNextRoom(string direction)
    {
        // Grab position of current room
        Vector2Int? thisRoomPosition = roomHandling.GetRoomPosition(this.gameObject);

        if (direction == "left") { nextRoomPositionX = thisRoomPosition.Value.x; nextRoomPositionY = thisRoomPosition.Value.y - 1; }
        else if (direction == "right") { nextRoomPositionX = thisRoomPosition.Value.x; nextRoomPositionY = thisRoomPosition.Value.y + 1; }
        else if (direction == "top") { nextRoomPositionX = thisRoomPosition.Value.x - 1; nextRoomPositionY = thisRoomPosition.Value.y; }
        else if (direction == "bottom") { nextRoomPositionX = thisRoomPosition.Value.x + 1; nextRoomPositionY = thisRoomPosition.Value.y; }
        
        // Makes space in the room structure if not there
        roomHandling.KeepPositionInBoundaries(nextRoomPositionX, nextRoomPositionY);

        // The room handling function will make sure there is space in the structure for placing the room, but this makes sure the array reference is constant based on that change
        if (nextRoomPositionX < 0) { nextRoomPositionX = 0; }
        if (nextRoomPositionY < 0) { nextRoomPositionY = 0; }

        // Grabs the data stored in the position of the next room in the grid structure
        GameObject nextRoom = gameData.roomStructure[nextRoomPositionX].objects[nextRoomPositionY];
        return nextRoom;
    }

    private bool CheckNewRoomFits(int roomPositionX, int roomPositionY, RoomData chosenRoomData)
    {
        bool connectionValid = true;

        // Decide north, east, south, west rooms in the structure - These will be checked against the chosen room to ensure that the entire dungeon connects correctly through each door
        RoomData leftRoomData = (roomPositionY - 1 >= 0) ? roomHandling.GetRoomData(roomPositionX, roomPositionY - 1) : null;
        RoomData rightRoomData = (roomPositionY + 1 < gameData.roomStructure[0].objects.Count) ? roomHandling.GetRoomData(roomPositionX, roomPositionY + 1) : null;
        RoomData topRoomData = (roomPositionX - 1 >= 0) ? roomHandling.GetRoomData(roomPositionX - 1, roomPositionY) : null;
        RoomData bottomRoomData = (roomPositionX + 1 < gameData.roomStructure.Count) ? roomHandling.GetRoomData(roomPositionX + 1, roomPositionY) : null;
        
        // Check each nesw direction to make whole dungeon valid
        if (leftRoomData != null && !leftRoomData.hasRightEntrance && chosenRoomData.hasLeftEntrance) { connectionValid = false; }
        if (rightRoomData != null && !rightRoomData.hasLeftEntrance && chosenRoomData.hasRightEntrance) { connectionValid = false; }
        if (topRoomData != null && !topRoomData.hasBottomEntrance && chosenRoomData.hasTopEntrance) { connectionValid = false; }
        if (bottomRoomData != null && !bottomRoomData.hasTopEntrance && chosenRoomData.hasBottomEntrance) { connectionValid = false; }

        return connectionValid;
    }
}