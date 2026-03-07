using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    // Script + Component Links
    GameSetup gameSetup;
    GameObject player;
    PlayerInputHandler playerInputHandler;
    GameData gameData;
    VisualAndSoundEffectHandling vsfxHandler;
    GameEndHandler gameEndHandler;
    AbilityHandler playerAbilityHandler;
    PlayerController playerController;

    [Header("Screens/Menus")]
    public GameObject gameoverScreen;
    public GameObject titleScreen;
    public GameObject settingsScreen;
    public GameObject pauseScreen;
    public GameObject audioSubSettings;
    public GameObject visualSubSettings;
    public GameObject runStatsScreen;
    public GameObject controlsScreen;
    public GameObject inventoryScreen;
    public GameObject tutorialScreen;

    [Header("Sliders")]
    public Slider visualEffectSlider;
    public Slider audioVolSlider;
    public Slider musicVolSlider;

    [Header("Run Stats Text")]
    public TMP_Text finalKillCount;
    public TMP_Text endDifficultyScale;
    public TMP_Text collectablesUsed;
    public TMP_Text abilitiesUsed;
    public TMP_Text totalRunTime;

    [Header("Controls Text")]
    public TMP_Text movement;
    public TMP_Text jump;
    public TMP_Text dash;
    public TMP_Text meleeAttack;
    public TMP_Text rangedAttack;
    public TMP_Text useAbility;
    public TMP_Text pickupAbility;
    public TMP_Text swapAbility;

    [Header("Inventory References")]
    public GameObject abilityOneRef;
    public GameObject abilityTwoRef;
    public GameObject abilityThreeRef;
    public TMP_Text abilityOneDesc;
    public TMP_Text abilityTwoDesc;
    public TMP_Text abilityThreeDesc;
    public Image abilityOneImage;
    public Image abilityTwoImage;
    public Image abilityThreeImage;
    public TMP_Text currentHealth;
    public TMP_Text fullHealth;
    public TMP_Text speed;
    public TMP_Text dashCooldown;
    public TMP_Text meleeDamage;
    public TMP_Text rangedDamage;

    private string previousScreen;

    private void Awake()
    {
        // Grabs all linked scripts + components
        gameSetup = GetComponent<GameSetup>();
        gameData = GetComponent<GameData>();
        vsfxHandler = GetComponent<VisualAndSoundEffectHandling>();
        gameEndHandler = GetComponent<GameEndHandler>();

        // Deactivate all menus but title screen
        titleScreen.SetActive(true);
        gameoverScreen.SetActive(false);
        settingsScreen.SetActive(false);
        pauseScreen.SetActive(false);
        audioSubSettings.SetActive(false);
        visualSubSettings.SetActive(false);
        runStatsScreen.SetActive(false);
        controlsScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        tutorialScreen.SetActive(false);

        Time.timeScale = 0f;
    }

    private void ChangeScreens(GameObject prevScreen, GameObject newScreen)
    {
        prevScreen.SetActive(false);
        newScreen.SetActive(true);
    }

    // Called by TitleScreen [Start Run Button]
    public void DeactivateTitleScreen()
    {
        // Deactivates the title screen menu that triggered this method
        titleScreen.SetActive(false);

        if (gameData.displayTutorialScreen)
        {
            OpenTutorialMenu();

            // Do not show tutorial screen on subsequent runs
            gameData.displayTutorialScreen = false;
        }
    }

    // Called by GameoverScreen [Restart Run Button]
    public void RestartGame()
    {
        // Deactivates the game over screen menu that triggered this method
        gameoverScreen.SetActive(false);

        // Flush all stats
        gameData.FlushRunStats();

        // Restart run state
        gameSetup.SetupGame();
    }

    // Called by TitleScreen [Quit Game Button]
    public void QuitGame()
    {
        Application.Quit();
    }

    // Called by GameoverScreen [Return To Title Button]
    public void ReturnToTitleScreen()
    {
        // Deactivates the game over screen menu that triggered this method
        gameoverScreen.SetActive(false);

        // Activate title screen
        titleScreen.SetActive(true);

        // Flush run stats
        gameData.FlushRunStats();
    }

    // Called by FadeScreenScript on game end
    public void ActivateGameOverScreen()
    {
        Time.timeScale = 0f;
        gameoverScreen.SetActive(true);
    }

    // Called by TitleScreen [Settings Button]
    public void OpenSettingsScreenFromTitle()
    {
        settingsScreen.SetActive(true);
        titleScreen.SetActive(false);
        previousScreen = "titleScreen";
    }

    // Called by PauseScreen [Settings Button]
    public void OpenSettingsScreenFromPause()
    {
        settingsScreen.SetActive(true);
        pauseScreen.SetActive(false);
        previousScreen = "pauseScreen";
    }

    // Called by SettingsScreen [Exit Settings Button]
    public void CloseSettingsScreen()
    { 
        if (previousScreen == "titleScreen")
        {
            settingsScreen.SetActive(false);
            titleScreen.SetActive(true);
        }
        else if (previousScreen == "pauseScreen")
        {
            settingsScreen.SetActive(false);
            pauseScreen.SetActive(true);
        }
    }

    // Called by PlayerInputHandler in response to player input of Esc
    public void OpenPauseScreen()
    {
        playerInputHandler = GameObject.Find("Player").GetComponent<PlayerInputHandler>();
        playerInputHandler.inPauseMenu = true;

        pauseScreen.SetActive(true);
        Time.timeScale = 0f;

        // Prevent player from inputting while in pause screen
        player = GameObject.Find("Player");
        playerInputHandler = player.GetComponent<PlayerInputHandler>();
        playerInputHandler.CanMove = false;
        playerInputHandler.CanAttack = false;
        playerInputHandler.CanUseAbilities = false;
    }

    // Called by PauseScreen [Return To Game Button]
    public void ClosePauseScreen()
    {
        playerInputHandler = GameObject.Find("Player").GetComponent<PlayerInputHandler>();
        playerInputHandler.inPauseMenu = false;

        pauseScreen.SetActive(false);
        Time.timeScale = 1f;

        // Allow player to input again
        player = GameObject.Find("Player");
        playerInputHandler = player.GetComponent<PlayerInputHandler>();
        playerInputHandler.CanMove = true;
        playerInputHandler.CanAttack = true;
        playerInputHandler.CanUseAbilities = true;
    }

    // Called by VisualSettingsScreen [Visual Opacity Slider]
    public void ChangeVisualOpacity()
    {
        gameData.universalVisualEffectOpacity = visualEffectSlider.value;
    }

    // Called by AudioSettingsScreen [Sound Effects Volume Slider]
    public void ChangeAudioVolume()
    {
        gameData.universalSoundVolume = audioVolSlider.value;
    }

    // Called by VisualSettingsScreen [Music Volume Slider]
    public void ChangeMusicVolume()
    {
        gameData.universalMusicVolume = musicVolSlider.value;

        // Change volume of music, important if music is currently playing
        vsfxHandler.ChangeMusicVolume(1f);
    }

    // Called by VisualSettingsScreen [Visual Opacity Deactivate Button]
    public void DeactivateVisualEffects()
    {
        gameData.universalVisualEffectOpacity = 0f;
        visualEffectSlider.value = 0f;
    }

    // Called by AudioSettingsScreen [Sound Effects Deactivate Button]
    public void DeactivateSoundEffects()
    {
        gameData.universalSoundVolume = 0f;
        audioVolSlider.value = 0f;
    }

    // Called by AudioSettingsScreen [Music Deactivate Button]
    public void DeactivateMusic()
    {
        gameData.universalMusicVolume = 0f;
        musicVolSlider.value = 0f;

        // Change volume of music, important if music is currently playing
        vsfxHandler.ChangeMusicVolume(1f);
    }

    // Called by SettingsScreen [Visual Settings Button]
    public void OpenVisualSubSettings()
    {
        visualSubSettings.SetActive(true);
        settingsScreen.SetActive(false);

        visualEffectSlider.value = gameData.universalVisualEffectOpacity;
    }

    // Called by SettingsScreen [Audio Settings Button]
    public void OpenAudioSubSettings()
    {
        audioSubSettings.SetActive(true);
        settingsScreen.SetActive(false);

        audioVolSlider.value = gameData.universalSoundVolume;
        musicVolSlider.value = gameData.universalMusicVolume;
    }

    // Called by either VisualSubSettings, AudioSubSettings or ControlSubSettings [Return To Settings Button]
    public void ReturnToRootSettingsMenu()
    {
        // +close controls
        audioSubSettings.SetActive(false);
        visualSubSettings.SetActive(false);
        settingsScreen.SetActive(true);
    }

    // Called by Pause Screen [Return To Title Button]
    public void ReturnToTitleFromPause()
    {
        // Get rid of run components
        gameEndHandler.DeleteRunParts();
        gameData.FlushRunStats();

        // Deactivates the pause screen menu that triggered this method
        pauseScreen.SetActive(false);

        // Activate title screen
        titleScreen.SetActive(true);
    }

    // Called by Gameover Screen [View Run Stats Button]
    public void OpenRunStats()
    {
        gameoverScreen.SetActive(false);
        runStatsScreen.SetActive(true);

        finalKillCount.text = "Final Kill Count - " + gameData.playerKillCount;
        endDifficultyScale.text = "End Difficulty Scale - " + gameData.difficultyScale;
        collectablesUsed.text = "Collectables Used - " + gameData.consumablesCollected;
        abilitiesUsed.text = "Abilities Used - " + gameData.abilitiesUsed;

        int runHours = (int)(gameData.runTime / 3600);
        int runMinutes = (int)((gameData.runTime / 60) % 60);
        int runSeconds = (int)(gameData.runTime % 3600);

        totalRunTime.text = "Total Run Time - " + runHours + "/" + runMinutes + "/" + runSeconds;
    }

    // Called by Run Stats Screen [Return To Gameover Button]
    public void ReturnToGameoverScreen()
    {
        runStatsScreen.SetActive(false);
        gameoverScreen.SetActive(true);
    }

    // Called by Controls Screen [Return To Settings Button]
    public void ReturnToSettingsFromControls()
    {
        settingsScreen.SetActive(true);
        controlsScreen.SetActive(false);
    }

    // Called by Settings Screen [Controls Button]
    public void OpenControlsScreen()
    {
        controlsScreen.SetActive(true);
        settingsScreen.SetActive(false);

        movement.text = "Movement - A|D, Left+Right Arrows";
        jump.text = "Jump - Space";
        dash.text = "Dash - Left Shift";
        meleeAttack.text = "Melee Attack - Left Mouse, X";
        rangedAttack.text = "Ranged Attack - Right Mouse, F";
        useAbility.text = "Use Ability - 1/2/3";
        pickupAbility.text = "Pickup Ability - P";
        swapAbility.text = "Swap Ability - P and 1/2/3";
    }

    // Called by Pause Screen [Inventory Button]
    public void OpenInventoryScreenFromPauseScreen()
    {
        playerInputHandler = GameObject.Find("Player").GetComponent<PlayerInputHandler>();

        playerInputHandler.inPauseMenu = false;
        playerInputHandler.inInventoryMenu = true;

        inventoryScreen.SetActive(true);
        pauseScreen.SetActive(false);

        SetupInventoryScreen();
    }

    // Called by Inventory Screen [Return To Pause Button]
    public void OpenPauseScreenFromInventoryScreen()
    {
        playerInputHandler = GameObject.Find("Player").GetComponent<PlayerInputHandler>();

        inventoryScreen.SetActive(false);
        pauseScreen.SetActive(true);

        playerInputHandler.inPauseMenu = true;
        playerInputHandler.inInventoryMenu = false;
    }

    // Called internally for setting up inventory data when inventory screen opened
    public void SetupInventoryScreen()
    {
        player = GameObject.Find("Player");
        playerAbilityHandler = player.GetComponent<AbilityHandler>();
        playerController = player.GetComponent<PlayerController>();

        if (playerAbilityHandler.abilityOne == null)
        {
            abilityOneRef.SetActive(false);
        }
        else
        {
            abilityOneRef.SetActive(true);
            AbilityScript ability = playerAbilityHandler.abilityOne;
            abilityOneDesc.text = ability.abilityName + " - " + ability.abilityDesc;
            abilityOneImage.sprite = ability.gameObject.GetComponent<SpriteRenderer>().sprite;
        }

        if (playerAbilityHandler.abilityTwo == null)
        {
            abilityTwoRef.SetActive(false);
        }
        else
        {
            abilityTwoRef.SetActive(true);
            AbilityScript ability = playerAbilityHandler.abilityTwo;
            abilityTwoDesc.text = ability.abilityName + " - " + ability.abilityDesc;
            abilityTwoImage.sprite = ability.gameObject.GetComponent<SpriteRenderer>().sprite;
        }

        if (playerAbilityHandler.abilityThree == null)
        {
            abilityThreeRef.SetActive(false);
        }
        else
        {
            abilityThreeRef.SetActive(true);
            AbilityScript ability = playerAbilityHandler.abilityThree;
            abilityThreeDesc.text = ability.abilityName + " - " + ability.abilityDesc;
            abilityThreeImage.sprite = ability.gameObject.GetComponent<SpriteRenderer>().sprite;
        }

        currentHealth.text = "Current Health - " + (int)playerController.currentHealth;
        fullHealth.text = "Full Health - " + (int)playerController.fullHealth;
        speed.text = "Speed - " + (int)playerController.currentSpeed + "m/s";
        dashCooldown.text = "Dash Cooldown - " + (int)playerController.dodgeCooldown + "s";
        meleeDamage.text = "Melee Damage - " + (int)playerController.currentMeleeDamage;
        rangedDamage.text = "Ranged Damage - " + (int)playerController.currentRangedDamage;
    }

    // Called by PlayerInputHandler in response to player input of I in run
    public void OpenInventoryFromGame()
    {
        playerInputHandler = GameObject.Find("Player").GetComponent<PlayerInputHandler>();

        inventoryScreen.SetActive(true);

        Time.timeScale = 0f;

        playerInputHandler.inInventoryMenu = true;

        SetupInventoryScreen();
    }

    // Called by PlayerInputHandler in response to player input of I in Inventory Screen
    public void ContinueRunFromInventory()
    {
        playerInputHandler = GameObject.Find("Player").GetComponent<PlayerInputHandler>();

        inventoryScreen.SetActive(false);

        Time.timeScale = 1f;

        playerInputHandler.inInventoryMenu = false;
    }

    public void OpenTutorialMenu()
    {
        tutorialScreen.SetActive(true);

        // Prevent player from inputting while in tutorial screen
        player = GameObject.Find("Player");
        playerInputHandler = player.GetComponent<PlayerInputHandler>();
        playerInputHandler.CanMove = false;
        playerInputHandler.CanAttack = false;
        playerInputHandler.CanUseAbilities = false;
    }

    public void CloseTutorialScreen()
    {
        tutorialScreen.SetActive(false);

        // Reallow player control
        player = GameObject.Find("Player");
        playerInputHandler = player.GetComponent<PlayerInputHandler>();
        playerInputHandler.CanMove = true;
        playerInputHandler.CanAttack = true;
        playerInputHandler.CanUseAbilities = true;
    }
}
