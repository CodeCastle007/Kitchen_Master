using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour,IKitchenObjectParent
{
    [SerializeField] private Transform tableTopPoint;
    [SerializeField] private Transform employeeStandingPoint; 
    [SerializeField] private CounterSO counterSO; 

    private KitchenObject kitchenObject;

    public static event Action<Transform> OnAnyObjectPlacedHere;

    public virtual void Interact(IKitchenObjectParent player)
    {
        Debug.Log("Interacting with Base Counter");
    }

    public virtual void InteractAlternate(IKitchenObjectParent kitchenObjectParent)
    {
        Debug.Log("Alternate Interacting with Base Counter");
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject ? true : false;
    }

    public String GetDisplayName()
    {
        return counterSO.displayUIName;
    }

    public String GetTutorialMessage()
    {
        return counterSO.tutorialMessage;
    }

    public Transform GetEmployeeStandingPoint()
    {
        return employeeStandingPoint;
    }
    public virtual bool HasPlateToClean()
    {
        if (HasKitchenObject())
        {
            if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //Return true if we have unclean plate
                return !plateKitchenObject.IsClean();
            }
        }
        return false;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (this.kitchenObject != null)
        {
            OnAnyObjectPlacedHere?.Invoke(transform);
        }
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return tableTopPoint;
    }

    public static void ResetStaticData()
    {
        OnAnyObjectPlacedHere = null;
    }

    public CounterSO GetCounterSO()
    {
        return counterSO;
    }
}
