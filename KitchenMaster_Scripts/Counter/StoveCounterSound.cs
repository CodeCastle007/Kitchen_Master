using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        SoundManager.Instance.OnSoundVolumeChanged += SoundManager_OnSoundVolumeChanged;
    }

    private void SoundManager_OnSoundVolumeChanged(float obj)
    {
        audioSource.volume = obj;
    }

    private void StoveCounter_OnStateChanged(StoveCounter.CookingState state)
    {
        bool playSound = state == StoveCounter.CookingState.Cooking || state == StoveCounter.CookingState.Cooked;
        if (playSound)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }
}
