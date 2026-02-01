using UnityEngine;
using System.Collections.Generic;

public class VisualAndSoundEffectHandling : MonoBehaviour, UsesCooldown
{
    // Script + Component Links
    GameData gameData;
    CooldownTimer cooldownHandler;
    public AudioSource audioSource;
    public AudioSource musicSource;

    private List<SpriteRenderer> whiteFlashRenderers = new List<SpriteRenderer>();

    private void Awake()
    {
        // Grabs all linked scripts + components
        gameData = GetComponent<GameData>();
        cooldownHandler = GetComponent<CooldownTimer>();
    }

    private void Start()
    {
        // Gives cooldown handler necessary values to setup timers
        List<string> keyList = new List<string> { "whiteFlashLength" };
        List<float> lengthList = new List<float> { 0.1f };
        cooldownHandler.SetupTimers(keyList, lengthList, this);
    }

    public void PlaySound(AudioClip soundEffect, float soundVolume = 1f)
    {
        // Decide volume of sound based of the specific sound and the global volume scale, then play the sound
        float volume = soundVolume * (gameData.universalSoundVolume / 100);
        audioSource.PlayOneShot(soundEffect, volume);
    }

    public void PlayMusic(AudioClip musicTrack, float musicVolume = 1f)
    {
        // Decide volume of music based of the specific music and the global music scale, set to loop, then play the music
        musicSource.clip = musicTrack;
        musicSource.volume = musicVolume * (gameData.universalMusicVolume / 100);
        musicSource.loop = true;
        musicSource.Play();
    }

    public void ChangeMusicVolume(float musicVolume = 1f)
    {
        musicSource.volume = musicVolume * (gameData.universalMusicVolume / 100);
    }

    public void PlayVisualEffect(string effect, float effectOpacity, SpriteRenderer renderer)
    {
        if (effect == "whiteflash")
        {
            cooldownHandler.timerStatusDict["whiteFlashLength"] = 1;
            Color col = new Color(0, 0, 0);
            col.a = effectOpacity * (gameData.universalVisualEffectOpacity / 100);
            renderer.color = col;
        }
    }

    // Allows specific processes to be coded in to happen once a cooldown ends
    public void CooldownEndProcess(string key)
    {
        if (key == "whiteFlashLength")
        {
            // Reset player color
            SpriteRenderer renderer = whiteFlashRenderers[0];
            Color col = renderer.color;
            col.a = 0;
            renderer.color = col;
            /////////////////////////////////////////////////////////////////////////////////////////////// this + playvisualeffect doesnt work as alot of flashes may happen at once, add another script to each flashable object? or put it in hp handler, have the timer be set in there, as well as the reset, but activation here
        }
    }
}
