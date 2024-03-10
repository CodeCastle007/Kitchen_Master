using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinksCounterSound : MonoBehaviour
{
    [SerializeField] private DrinksCounter drinksCounter;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        drinksCounter.OnStateChanged += DrinksCounter_OnStateChanged;
    }

    private void DrinksCounter_OnStateChanged(DrinksCounter.FillingState state)
    {
        bool playSound = state == DrinksCounter.FillingState.Filling;
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
