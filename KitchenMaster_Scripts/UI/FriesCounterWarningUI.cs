using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriesCounterWarningUI : MonoBehaviour
{
    [SerializeField] private Transform icon;
    private FryerCounter fryerCounter;

    private void Start()
    {
        fryerCounter = GetComponentInParent<FryerCounter>();

        fryerCounter.OnStateChange += FryerCounter_OnStateChange;
        Hide();
    }

    private void FryerCounter_OnStateChange(FryerCounter.FryingState obj)
    {
        if (fryerCounter.FriesFried())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        icon.gameObject.SetActive(true);
    }
    private void Hide()
    {
        icon.gameObject.SetActive(false);
    }
}
