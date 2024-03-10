using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CashUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cashText;
    
    private void Start(){
        CashManager.Instance.OnCashAmountChanged += CashManager_OnCashAmountChanged;

        UpdateCashText();
    }

    private void CashManager_OnCashAmountChanged(){
        UpdateCashText();
    }

    private void UpdateCashText(){
        string dollars = CashManager.Instance.GetTotalCash().ToString();

        cashText.text = dollars;
    }

       
}