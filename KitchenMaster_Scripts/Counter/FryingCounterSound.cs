using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FryingCounterSound : MonoBehaviour
{
    [SerializeField] private FryerCounter fryerCounter;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        fryerCounter.OnStateChange += FryerCounter_OnStateChange;
        SoundManager.Instance.OnSoundVolumeChanged += SoundManager_OnSoundVolumeChanged;
    }

    private void SoundManager_OnSoundVolumeChanged(float obj)
    {
        audioSource.volume = obj;
    }

    private void FryerCounter_OnStateChange(FryerCounter.FryingState state)
    {
        bool playSound = state == FryerCounter.FryingState.Cooking || state == FryerCounter.FryingState.Cooked;
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
