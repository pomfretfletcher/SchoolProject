using UnityEngine;
using System.Collections.Generic;

public class GameData : MonoBehaviour
{
    // Where rooms are stored and organized
    public List<GameObjectList> roomStructure = new List<GameObjectList>();

    public GameObject currentRoom;

    // Customizable Values
    public float universalSoundVolume;
    public float universalMusicVolume;
    public float universalVisualEffectOpacity;

    // Run stats
    public float playerKillCount;
    public float runTime;

    private void FixedUpdate()
    {
        runTime += Time.deltaTime;
    }

    public void FlushRunStats()
    {
        playerKillCount = 0f;
        runTime = 0f;
    }
}