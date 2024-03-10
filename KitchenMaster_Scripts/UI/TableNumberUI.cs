using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TableNumberUI : MonoBehaviour
{
    [SerializeField] private DeliveryTable deliveryTable;
    [SerializeField] private TextMeshProUGUI numberText;

    private void Awake()
    {
        deliveryTable.SetNumberUI += DeliveryTable_SetNumberUI;
    }

    private void DeliveryTable_SetNumberUI(int number)
    {
        numberText.text = number.ToString();
    }
}
