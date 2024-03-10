using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveHeatVisual;
    [SerializeField] private GameObject stoveSizzleParticles;

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;

        Hide();
    }

    private void StoveCounter_OnStateChanged(StoveCounter.CookingState state)
    {
        if(state==StoveCounter.CookingState.Cooking || state == StoveCounter.CookingState.Cooked)
        {
            Show();
        }
        else 
        { 
            Hide(); 
        }
    }

    private void Hide()
    {
        stoveHeatVisual.SetActive(false);
        stoveSizzleParticles.SetActive(false);
    }
    private void Show()
    {
        stoveHeatVisual.SetActive(true);
        stoveSizzleParticles.SetActive(true);
    }
}
