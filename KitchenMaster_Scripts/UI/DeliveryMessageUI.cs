using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryMessageUI : MonoBehaviour
{
    private DeliveryTable deliveryTable;
    [SerializeField] private Transform container;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;

    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;


    [SerializeField] private Sprite checkMark;
    [SerializeField] private Sprite crossMark;

    [SerializeField] private float activateTimerMax = 1f;
    private float currentActivationTimer;

    private void Start()
    {
        deliveryTable = GetComponentInParent<DeliveryTable>();
        deliveryTable.OnSuccessfulDelivery += DeliveryTable_OnSuccessfulDelivery;
        deliveryTable.OnFailedDelivery += DeliveryTable_OnFailedDelivery;

        container.gameObject.SetActive(false);
       
    }

    private void Update()
    {
        if (currentActivationTimer > 0)
        {
            currentActivationTimer -= Time.deltaTime;

            if (currentActivationTimer <= 0)
            {
                container.gameObject.SetActive(false);
            }
        }
    }

    private void DeliveryTable_OnFailedDelivery()
    {
        backgroundImage.color = failedColor;
        text.text = "Incorrect Delivery";
        icon.sprite = crossMark;

        container.gameObject.SetActive(true);
        currentActivationTimer = activateTimerMax;
    }

    private void DeliveryTable_OnSuccessfulDelivery()
    {
        backgroundImage.color = successColor;
        text.text = "Correct Delivery";
        icon.sprite = checkMark;

        container.gameObject.SetActive(true);
        currentActivationTimer = activateTimerMax;
    }
}
