using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTemplateUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI resourceAmount;

    public void SetResourceAmountandIcon(int amount,Sprite icon)
    {
        if (amount > 0)
        {
            resourceAmount.color = Color.white;
        }
        else
        {
            resourceAmount.color = Color.red;
        }

        resourceAmount.text = amount.ToString();
        this.icon.sprite = icon;
    }
}
