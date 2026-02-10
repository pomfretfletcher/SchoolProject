using UnityEngine;
using System.Collections.Generic;

public class VisualAndSoundEffectHandling : MonoBehaviour
{
    // Script + Component Links
    GameData gameData;
    public AudioSource audioSource;
    public AudioSource musicSource;

    private void Awake()
    {
        // Grabs all linked scripts + components
        gameData = GetComponent<GameData>();
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
            if (gameData.universalVisualEffectOpacity > 0)
            {
                Color col = new Color(255, 255, 255);
                col.a = effectOpacity * (gameData.universalVisualEffectOpacity / 100);
                renderer.color = col;
            }
        }
        if (effect == "toxicEffect")
        {
            if (gameData.universalVisualEffectOpacity > 0)
            {
                Color col = renderer.color;
                col.a = effectOpacity * (gameData.universalVisualEffectOpacity / 100);
                renderer.color = col;
            }
        }
    }
}
