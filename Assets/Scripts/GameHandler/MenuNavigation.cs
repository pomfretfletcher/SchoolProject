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

    [Header("Screens/Menus")]
    public GameObject gameoverScreen;
    public GameObject titleScreen;
    public GameObject settingsScreen;
    public GameObject pauseScreen;
    public GameObject audioSubSettings;
    public GameObject visualSubSettings;

    [Header("Sliders")]
    public Slider visualEffectSlider;
    public Slider audioVolSlider;
    public Slider musicVolSlider;

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

        Time.timeScale = 0f;
    }

    // Called by TitleScreen [Start Run Button]
    public void DeactivateTitleScreen()
    {
        // Deactivates the title screen menu that triggered this method
        titleScreen.SetActive(false);
    }

    // Called by GameoverScreen [Restart Run Button]
    public void RestartGame()
    {
        // Deactivates the game over screen menu that triggered this method
        gameoverScreen.SetActive(false);

        // Flush all stats
        gameData.playerKillCount = 0;
        gameData.runTime = 0;

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
}
