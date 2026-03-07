using UnityEngine;
using System.Collections.Generic;

public class GameData : MonoBehaviour
{
    // Where rooms are stored and organized
    public List<GameObjectList> roomStructure = new List<GameObjectList>();

    public GameObject currentRoom;

    public List<GameObject> runEnemies = new List<GameObject>();

    // Customizable Values
    public float universalSoundVolume;
    public float universalMusicVolume;
    public float universalVisualEffectOpacity;
    public float baseDifficultyScale;
    public float difficultyScaleIncreasePerRoom;
    public bool displayTutorialScreen = true;

    // Run stats
    public float playerKillCount;
    public float runTime;
    public float difficultyScale;
    public int consumablesCollected;
    public int abilitiesUsed;

    private void FixedUpdate()
    {
        runTime += Time.deltaTime;
    }

    public void FlushRunStats()
    {
        playerKillCount = 0f;
        runTime = 0f;
        difficultyScale = 0f;
        abilitiesUsed = 0;
        consumablesCollected = 0;
    }

    public void IncreaseDifficultyScale()
    {
        // Increase by set amount
        difficultyScale += difficultyScaleIncreasePerRoom;
    }

    public void ReduceDifficultyScale(float amountLost)
    {
        // Reduce by given amount
        difficultyScale -= amountLost;

        // Alter all pre existing enemy's scaled attributes based on new scale
        for (var i = 0; i< runEnemies.Count; i++) 
        {
            GameObject enemy = runEnemies[i];
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemyController.AlterScale(difficultyScale);
        }
    }
}