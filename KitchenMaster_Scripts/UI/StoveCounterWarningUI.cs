using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoveCounterWarningUI : MonoBehaviour
{
    [SerializeField] private Transform icon;
    private StoveCounter stoveCounter;

    private void Start()
    {
        stoveCounter = GetComponentInParent<StoveCounter>();

        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        Hide();
    }

    private void StoveCounter_OnStateChanged(StoveCounter.CookingState obj)
    {
        if (stoveCounter.PattyCooked())
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
