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
}

// A custom class designed to allow viewing of 2D arrays in the inspector
[System.Serializable]
public class GameObjectList
{
    public List<GameObject> objects = new List<GameObject>();
}