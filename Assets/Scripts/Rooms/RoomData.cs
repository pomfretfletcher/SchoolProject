using UnityEngine;
using System.Collections.Generic;

public class RoomData : MonoBehaviour
{
    [Header("Size")]
    public string roomSize;

    [Header("Entrance Bools")]
    public bool hasLeftEntrance;
    public bool hasRightEntrance;
    public bool hasTopEntrance;
    public bool hasBottomEntrance;

    [Header("Entrance Positions")]
    public Transform leftEntrancePlayerSpawnPoint;
    public Transform rightEntrancePlayerSpawnPoint;
    public Transform topEntrancePlayerSpawnPoint;
    public Transform bottomEntrancePlayerSpawnPoint;

    [Header("Entrance Colliders")]
    public Collider2D leftEntranceExit;
    public Collider2D rightEntranceExit;
    public Collider2D topEntranceExit;
    public Collider2D bottomEntranceExit;

    [Header("Starting Room Variables")]
    public bool isStartingRoom;
    public Transform playerSpawnPoint;

    [Header("Pickup, Ability, Powerup and Enemy Positions")]
    public List<Transform> enemySpawnPositions;
    public List<Transform> pickupSpawnPositions;
    public List<Transform> abilitySpawnPositions;
    public List<Transform> powerupSpawnPositions;

    [Header("Tracked Variables")]
    public bool playerIsInRoom = false;
}