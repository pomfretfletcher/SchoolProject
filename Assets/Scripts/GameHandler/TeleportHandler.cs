using UnityEngine;
using System.Collections.Generic;

public class TeleportHandler : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    GameObject player;
    PlayerInputHandler playerInputHandler;
    CooldownTimer cooldownHandler;
    RoomTransitionHandler transitionHandler;

    // Customizable Values
    public float playerLockTime = 1f;

    private void Awake()
    {
        // Grabs all linked scripts + components
        cooldownHandler = GetComponent<CooldownTimer>();
        transitionHandler = GetComponent<RoomTransitionHandler>();
    }

    public void OnPlayerTeleport(Vector3 playerEndPosition)
    {
        player = GameObject.Find("Player");
        playerInputHandler = player.GetComponent<PlayerInputHandler>();
        // Prevents most forms of player input - all important ones
        playerInputHandler.CanMove = false;
        playerInputHandler.CanAttack = false;
        playerInputHandler.CanUseAbilities = false;

        // Sets cooldown for how long movement prevention will be active
        cooldownHandler.timerStatusDict["playerLockTime"] = 1;

        // Teleport player to position
        player.transform.position = playerEndPosition;

        transitionHandler.BeginFade("room");
    }

    private void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "playerLockTime" };
        List<float> lengthList = new List<float> { playerLockTime };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    public void CooldownEndProcess(string key)
    {
        if (key == "playerLockTime")
        {
            // Re provides player with control over their character
            playerInputHandler.CanMove = true;
            playerInputHandler.CanAttack = true;
            playerInputHandler.CanUseAbilities = true;
        }
    }
}
