using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmButtonSound : MonoBehaviour
{
    private void Start()
    {
        ManagerUI.Instance.OnPurchaseConfirm += ManagerUI_OnPurchaseConfirm;
    }

    private void ManagerUI_OnPurchaseConfirm(float obj)
    {
        if (obj > 0)
        {
            SoundManager.Instance.PlayPurchaseSound();
        }
        else
        {
            SoundManager.Instance.PlayButtonClickSound();
        }
    }
}
