using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private const string PLAYERPREFS_MUSIC_MANAGER = "MusicVolume";
    public static MusicManager Instance { get; private set; }
    private float volume = 0.3f;
    private AudioSource audioSource;
    private void Awake()
    {
        Instance = this;

        volume = PlayerPrefs.GetFloat(PLAYERPREFS_MUSIC_MANAGER, .3f);
    }

    private void OptionsUI_OnMusicVolumeChanged(float obj)
    {
        SetVolume(obj);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        OptionsUI.Instance.OnMusicVolumeChanged += OptionsUI_OnMusicVolumeChanged;

        SetVolume(volume);
    }
    public void SetVolume(float volume)
    {
        this.volume = volume;

        audioSource.volume = this.volume;
    }

    //public float GetVolume()
    //{
    //    return volume;
    //}
}
