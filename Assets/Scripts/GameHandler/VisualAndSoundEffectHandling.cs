using UnityEngine;

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
        float volume = soundVolume * gameData.universalSoundVolume;
        audioSource.PlayOneShot(soundEffect, volume);
    }

    public void PlayMusic(AudioClip musicTrack, float musicVolume)
    {
        // Decide volume of music based of the specific music and the global music scale, set to loop, then play the music
        musicSource.clip = musicTrack;
        musicSource.volume = musicVolume * gameData.universalMusicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayVisualEffect(float effectOpacity)
    {

    }
}
