using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePurchaseTemplateUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private TextMeshProUGUI purchaseTextUI;
    [SerializeField] private TextMeshProUGUI costTextUI;

    [SerializeField] private Button increaseCountButton;
    [SerializeField] private Button decreaseCountButton;

    private int purchaseCount = 0;

    private KitchenObjectSO kitchenObjectSO;

    //Subscribed by Manager Ui to set the total cost 
    public static event Action<KitchenObjectSO> OnAnyProductCountIncrease;
    public static event Action<KitchenObjectSO> OnAnyProductCountDecrease;

    private void Awake()
    {
        increaseCountButton.onClick.AddListener(() =>
        {
            purchaseCount++;
            purchaseTextUI.text = purchaseCount.ToString();

            OnAnyProductCountIncrease?.Invoke(kitchenObjectSO);
        });

        decreaseCountButton.onClick.AddListener(() =>
        {
            purchaseCount--;
            if (purchaseCount >= 0)
            {
                //We will only invoke event if value is greater than 0
                OnAnyProductCountDecrease?.Invoke(kitchenObjectSO);
            }
            else
            {
                purchaseCount = 0;
            }

            purchaseTextUI.text = purchaseCount.ToString();
        });
    }


    public void SetKitchenObjectSOValues(KitchenObjectSO kitchenObjectSO)
    {
        this.kitchenObjectSO = kitchenObjectSO;

        icon.sprite = kitchenObjectSO.icon;
        nameText.text = kitchenObjectSO.objectName;
        costTextUI.text = kitchenObjectSO.cost_dollars + "$";
        purchaseTextUI.text = purchaseCount.ToString();
    }

    public int GetPurchaseCount()
    {
        return purchaseCount;
    }

    //For resetting the purchase count when purchase happen
    public void ResetPurchaseCount()
    {
        purchaseCount = 0;
        purchaseTextUI.text = purchaseCount.ToString();
    }

}
