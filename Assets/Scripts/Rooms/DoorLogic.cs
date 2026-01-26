using UnityEngine;

public class DoorLogic : MonoBehaviour
{
    // Which door the applied object is for deciding which direction to create/enter rooms with - assigned in inspector
    public bool isLeftDoor;
    public bool isRightDoor;
    public bool isTopDoor;
    public bool isBottomDoor;

    // Internal Logic Variables
    private string direction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Grabs needed components
        RoomLogic roomLogic = GetComponentInParent<RoomLogic>();

        // Decides which direction to feed the change room function
        if (isLeftDoor) { direction = "left"; }
        else if (isRightDoor) { direction = "right"; }
        else if (isTopDoor) { direction = "top"; }  
        else if (isBottomDoor) {  direction = "bottom"; }

        // Tells the current room to change room based on direction given
        roomLogic.ChangeRoom(direction);
    }
}