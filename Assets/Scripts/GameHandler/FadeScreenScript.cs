using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FadeScreenScript : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    CooldownTimer cooldownHandler;
    MenuNavigation menuNav;
    GameEndHandler gameEndHandler;
    GameObject player;
    PlayerInputHandler playerInputHandler;
    
    // Customizable Values
    public float timeOnScreen;
    public float timeFading;
    public string fadeToColor;

    // Inspector assigned screen that will be changed color
    public GameObject colorScreen;
    public Image colorScreenImage;

    // Internal logic variables
    private float staticScreenTime = 0.2f;
    private bool gameoverMode = false;

    private void Awake()
    {
        // Grabs all linked scripts + components
        cooldownHandler = GetComponent<CooldownTimer>();
        menuNav = GameObject.Find("GameHandler").GetComponent<MenuNavigation>();
        gameEndHandler = GameObject.Find("GameHandler").GetComponent<GameEndHandler>();
        player = GameObject.Find("Player");
        playerInputHandler = player.GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "timeOnScreen", "staticScreenTime" };
        List<float> lengthList = new List<float> { timeOnScreen, staticScreenTime };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
        cooldownHandler.timerStatusDict["staticScreenTime"] = 1;

        // If fading from nothing to black, start color at 0 alpha
        if (fadeToColor == "black")
        {
            Color color = colorScreenImage.color;
            color.a = Mathf.Clamp01(0);
            colorScreenImage.color = color;
        }
    }

    public void FixedUpdate()
    {
        if (cooldownHandler.timerStatusDict["timeOnScreen"] == 1)
        {
            if (fadeToColor == "white")
            {
                // Decides percentage of how much the screen will be faded and then applies that to the screen
                float alphaPercent = 1 - ((cooldownHandler.timerDict["timeOnScreen"] / timeFading) / 2);
                Color color = colorScreenImage.color;
                color.a = Mathf.Clamp01(alphaPercent);
                colorScreenImage.color = color;
            }
            else if (fadeToColor == "black")
            {
                // Decides percentage of how much the screen will be faded and then applies that to the screen
                float alphaPercent = ((cooldownHandler.timerDict["timeOnScreen"] / timeFading));
                Color color = colorScreenImage.color;
                color.a = Mathf.Clamp01(alphaPercent);
                colorScreenImage.color = color;
            }
        }
    }

    public void SetMode(string mode)
    { 
        // Set functionality mode of the screen based on input
        if (mode == "Gameover")
        {
            gameoverMode = true;
        }
    }

    public void CooldownEndProcess(string key)
    {
        // Fade to white is for room transitions, if we're fading to black, we don't want to remove the screen
        if (key == "timeOnScreen" && fadeToColor == "white")
        {
            playerInputHandler.movingThroughRooms = false;
            Destroy(this.gameObject);
        }
        // Time on screen without fading, once completed, we now start fading
        else if (key == "staticScreenTime")
        {
            cooldownHandler.timerStatusDict["timeOnScreen"] = 1;
        }
        // If game over screen, delete run components
        else if (key == "timeOnScreen" && gameoverMode == true)
        {
            // Activate the game over screen
            menuNav.ActivateGameOverScreen();
            gameEndHandler.DeleteRunParts();
            Destroy(this.gameObject);
        }
    }
}
