using UnityEngine;
using System.Collections.Generic;

public class RoomHandling : MonoBehaviour
{
    // Script + Component Links
    GameData gameData;

    // Internal/External Logic Variables - used for deciding where room is placed in world
    public int currentXPosInScene = 0;
    public int currentYPosInScene = 0;

    private void Awake()
    {
        // Grabs all linked scripts + components
        gameData = GetComponent<GameData>();
    }

    // Add row to either side of the room grid
    public void AddRowToStructure(string side) // side is either top or bottom for where to add the row
    {
        if (side == "top")
        {
            // Create an empty row
            GameObjectList row = new GameObjectList();
            // Create 5 empty cells for the row
            for (int i = 0; i < gameData.roomStructure[0].objects.Count; i++)
            {
                // These will be empty until a room is added into them
                row.objects.Add(null);
            }
            gameData.roomStructure.Insert(0, row);
        }
        else if (side == "bottom")
        {
            // Create an empty row
            GameObjectList row = new GameObjectList();
            // Create 5 empty cells for the row
            for (int i = 0; i < gameData.roomStructure[0].objects.Count; i++)
            {
                // These will be empty until a room is added into them
                row.objects.Add(null);
            }
            gameData.roomStructure.Add(row);
        }
    }

    // Add column to either side of the room grid
    public void AddColumnToStructure(string side) // side is either left or right for where to add the column
    {
        if (side == "left")
        {
            for (int i = 0; i < gameData.roomStructure.Count; i++)
            {
                // Add a cell to each row in the room structure
                gameData.roomStructure[i].objects.Insert(0, null);
            }
        }
        else if (side == "right")
        {
            for (int i = 0; i < gameData.roomStructure.Count; i++)
            {
                // Add a cell to each row in the room structure
                gameData.roomStructure[i].objects.Add(null);
            }
        }
    }

    // Places a room within the room grid structure, this keeps them in place for room to room travel
    public void PlaceRoomInStructure(int rowIndex, int colIndex, GameObject room)
    {
        // If the indexed position is currently in the map, place it in there, this is merely a check, as the indexes should be kept within
        // boundaries by another function
        if ((0 <= rowIndex && rowIndex <= gameData.roomStructure.Count) && (0 <= colIndex && colIndex <= gameData.roomStructure[0].objects.Count)) 
        { 
            gameData.roomStructure[rowIndex].objects[colIndex] = room;
        }
    }

    // Finds the position of a given room within the roomn grid structure
    public Vector2Int? GetRoomPosition(GameObject room)
    {
        // Checks each row
        for (int row = 0; row < gameData.roomStructure.Count; row++)
        {
            // Grabs that row for easier access
            List<GameObject> rowList = gameData.roomStructure[row].objects;

            // Checks each cell within that row (this is the column of the greater structure)
            for (int col = 0; col < rowList.Count; col++)
            {
                // Returns row and column if the room is found
                if (rowList[col] == room)
                {
                    return new Vector2Int(row, col);
                }
            }
        }

        // If not found, return null
        return null;
    }

    // If a room needs to be made but the grid isn't big enough in one direction, a new row/column will be made in said direction
    public void KeepPositionInBoundaries(int rowIndex, int colIndex)
    {
        // Ensures position remains in boundaries by adding rows and columns to the structure if needed
        if (rowIndex >= 0 && rowIndex < gameData.roomStructure.Count) { } // In boundaries, do not need to add anything 
        else if (rowIndex < 0) { AddRowToStructure("top"); }
        else { AddRowToStructure("bottom"); }

        if (colIndex >= 0 && colIndex < gameData.roomStructure[0].objects.Count) { } // In boundaries, do not need to add anything 
        else if (colIndex < 0) { AddColumnToStructure("left"); }
        else { AddColumnToStructure("right"); }
    }

    // Create a room based off a given new room prefab within an index
    public void CreateRoom(GameObject newRoom, int newRoomRowIndex, int newRoomColIndex, string enterDirection)
    {
        // Creates the new room based on the prefab and it gives it the variables required to setup its enemies and pickups etc
        GameObject setupRoom = Instantiate(newRoom, new Vector3(currentXPosInScene, currentYPosInScene, 0), Quaternion.identity);
        RoomCreation setupRoomCreator = setupRoom.GetComponent<RoomCreation>();

        // fillerrrrrrrrrrrrrrrrrr
        setupRoomCreator.SetupRoom(enterDirection, 1, null, 2, null, 2, null, 1, null, 1);

        // Places the setup room in the alloted position
        PlaceRoomInStructure(newRoomRowIndex, newRoomColIndex, setupRoom);
    }

    public GameObject GetRoom(int roomRowIndex, int roomColIndex)
    {
        GameObject fetchedRoom = gameData.roomStructure[roomRowIndex].objects[roomColIndex];
        return fetchedRoom;
    }

    public RoomData GetRoomData(int roomRowIndex, int roomColIndex)
    {
        GameObject fetchedRoom = gameData.roomStructure[roomRowIndex].objects[roomColIndex];
        if (fetchedRoom == null) { return null; }
        RoomData fetchedRoomData = fetchedRoom.GetComponent<RoomData>();
        return fetchedRoomData;
    }
}
