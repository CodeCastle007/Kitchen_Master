using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FryingCounterVisual : MonoBehaviour
{
    [SerializeField] private FryerCounter fryerCounter;
    [SerializeField] private GameObject sizzlingParticles;

    private void Start()
    {
        fryerCounter.OnStateChange += FryerCounter_OnStateChange;
    }

    private void FryerCounter_OnStateChange(FryerCounter.FryingState state)
    {
        if(state==FryerCounter.FryingState.Cooking || state == FryerCounter.FryingState.Cooked)
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
        sizzlingParticles.SetActive(false);
    }
    private void Show()
    {
        sizzlingParticles.SetActive(true);
    }
}
