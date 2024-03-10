using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkCounterVisual : MonoBehaviour
{
    [SerializeField] private SinkCounter sinkCounter;
    [SerializeField] private GameObject plateVisual;

    private void Start()
    {
        sinkCounter.OnUnCleanPlateDropped += SinkCounter_OnUnCleanPlateDropped;
        sinkCounter.OnCleanPlatePicked += SinkCounter_OnCleanPlatePicked;

        Hide();
    }

    private void SinkCounter_OnCleanPlatePicked()
    {
        Hide();
    }

    private void SinkCounter_OnUnCleanPlateDropped()
    {
        Show();
    }

    private void Hide()
    {
        plateVisual.SetActive(false);
    }
    private void Show()
    {
        plateVisual.SetActive(true);
    }
}
