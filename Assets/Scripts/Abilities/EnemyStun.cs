using System.Collections.Generic;
using UnityEngine;

public class EnemyStun : MonoBehaviour, IsAbility, UsesCooldown
{
    // Script + Component Links
    CooldownTimer cooldownHandler;
    GameData gameData;

    // Customizable Values
    public float stunDuration;

    // Internal logic variables
    private GameObject affectedRoom;

    private void Awake()
    {
        // Grabs all linked scripts + components
        cooldownHandler = GetComponent<CooldownTimer>();
        gameData = GameObject.Find("GameHandler").GetComponent<GameData>();
    }

    private void Start()
    {
        // Setup cooldown for how long boost lasts
        List<string> keyList = new List<string> { "stunDuration" };
        List<float> lengthList = new List<float> { stunDuration };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    public void OnActivation()
    {
        StartStun();
    }

    public void StartStun()
    {
        // For each enemy in current room, prevent movement, attacking for a short time
        affectedRoom = gameData.currentRoom;
        foreach (Transform child in affectedRoom.transform)
        {
            if (child.gameObject.tag == "Enemy")
            {
                LogicScript enemyLogicScript = child.GetComponent<LogicScript>();
                enemyLogicScript.CanMove = false;
                enemyLogicScript.CanAttack = false;
            }
        }
        cooldownHandler.timerStatusDict["stunDuration"] = 1;
    }

    public void CooldownEndProcess(string key)
    {
        // Deactivate stun
        foreach (Transform child in affectedRoom.transform)
        {
            if (child.gameObject.tag == "Enemy")
            {
                LogicScript enemyLogicScript = child.GetComponent<LogicScript>();
                enemyLogicScript.CanMove = true;
                enemyLogicScript.CanAttack = true;
            }
        }
    }
}
