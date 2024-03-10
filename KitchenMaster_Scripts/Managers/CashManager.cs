using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashManager : MonoBehaviour
{
    #region Singleton
    public static CashManager Instance{get;private set;}
    private void Awake(){
        Instance=this;
    }
    #endregion

    [SerializeField] private float totalCash;
   // [SerializeField] private int totalCents;

    public event Action OnCashAmountChanged;

    private void Start(){
        Customer.OnAnyCustomerLeave += Customer_OnAnyCustomerLeave;
        ManagerUI.Instance.OnPurchaseConfirm += ManagerUI_OnPurchase;
    }

    private void ManagerUI_OnPurchase(float obj)
    {
        ReduceCash(obj);
    }

    private void Customer_OnAnyCustomerLeave(float cost){
        AddCash(cost);
    }

    private void AddCash(float dollars){
        totalCash+= dollars;
        
        OnCashAmountChanged?.Invoke();
    }

    private void ReduceCash(float dollars){
        totalCash-= dollars;
       
        if(totalCash<0){
            Debug.LogError("Cash has gone under O...!!");
            totalCash=0;
        }

        OnCashAmountChanged?.Invoke();
    } 


    public float GetTotalCash(){
        return totalCash;
    }
}
