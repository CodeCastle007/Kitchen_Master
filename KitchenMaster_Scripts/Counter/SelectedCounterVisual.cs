using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private List<GameObject> selectedVisualList;

    private void Start()
    {
        Player.Instance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
    }

    private void Player_OnSelectedCounterChange(BaseCounter baseCounter)
    {
        if (this.baseCounter == baseCounter)
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
        foreach(GameObject selectedVisual in selectedVisualList)
            selectedVisual.SetActive(true);
    }
    private void Hide()
    {
        foreach (GameObject selectedVisual in selectedVisualList)
            selectedVisual.SetActive(false);
    }
}
