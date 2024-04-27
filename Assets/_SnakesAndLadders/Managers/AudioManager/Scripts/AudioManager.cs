using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource soundAudioSource;

    [SerializeField] private List<Audio_Clip> clipList = new List<Audio_Clip>();

    [Space(10)]
    public bool isFocused = true;

    private float musicPauseTime = 0;


    private string musicVol = "Music_Vol";
    internal bool IsMusicOn
    {
        get
        {
            return PlayerPrefsExtra.GetBool(musicVol, true);
        }
        set
        {
            PlayerPrefsExtra.SetBool(musicVol, value);
            PlayerPrefs.Save();
            musicAudioSource.volume = IsMusicOn ? 1 : 0;
            Debug.Log($"Music Pref : {value}");
        }
    }

    private string soundFxVol = "soundFx_Vol";
    internal bool IsSoundOn
    {
        get
        {
            return PlayerPrefsExtra.GetBool(soundFxVol, true);
        }
        set
        {
            PlayerPrefsExtra.SetBool(soundFxVol, value);
            PlayerPrefs.Save();
            soundAudioSource.volume = IsSoundOn ? 1 : 0;
            Debug.Log($"Sound Pref : {value}");
        }
    }



    private void Awake() => Instance = this;

    public void PlayAudioClip(ClipName _ClipName)
    {
        var clip = clipList.First(clip => clip.clipName == _ClipName).clip;
        if (clip != null)
            soundAudioSource.PlayOneShot(clip);
        else
            Debug.LogError("Clip is not Found");
    }

    public void PauseMusic()
    {
        musicPauseTime = musicAudioSource.time;
        musicAudioSource.Pause();
        //Debug.Log($"<color=yellow>Music Pause Time : {musicPauseTime} - {musicAudioSource.time}</color>");
    }

    public void PlayMusic()
    {
        musicAudioSource.time = musicPauseTime;
        musicAudioSource.Play();
        //Debug.Log($"<color=blue>Music Pause Time : {musicPauseTime} - {musicAudioSource.time}</color>");
    }



    

    void OnApplicationFocus(bool hasFocus)
    {
        isFocused = hasFocus;

        HandleAudioFocusChange();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        isFocused = !pauseStatus;

        HandleAudioFocusChange();
    }

    private void HandleAudioFocusChange()
    {
        if (isFocused && IsMusicOn)
        {
            if (musicAudioSource != null && !musicAudioSource.isPlaying)
            {
                // Only unpause if there's something in the queue to play
                // Set the audio to the time it was paused at and play
                PlayMusic();
                // Reset pauseTime
                musicPauseTime = 0f;
            }
        }
        else if (!isFocused)
        {
            if (musicAudioSource != null && musicAudioSource.isPlaying)
            {
                PauseMusic();
            }
        }
    }

}// CLASS

[Serializable]
public class Audio_Clip
{
    public string name;
    public ClipName clipName;
    public AudioClip clip;
}

public enum ClipName
{
    None = 0,
    Victory,
    TokenMove,
    DiceRoll,
    Snake,
    Ladder

}// ENUM