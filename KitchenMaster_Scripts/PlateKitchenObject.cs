using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;
    private List<KitchenObjectSO> kitchenObjectSOList;

    public event Action<KitchenObjectSO> OnIngredientAdded;
    public event Action OnClearVisual;

    private void Start()
    {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }
    public bool TryAddIngredient(KitchenObjectSO ingredient)
    {
        if (!validKitchenObjectSOList.Contains(ingredient))
        {
            //Not a valid ingredient 
            return false;
        }

        if (kitchenObjectSOList.Contains(ingredient))
        {
            //List aleady has that ingredient
            return false;
        }
        else
        {
            kitchenObjectSOList.Add(ingredient);

            //Firing event for Plate visuals
            OnIngredientAdded.Invoke(ingredient);

            return true;
        }
        
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }

    public void ClearVisuals()
    {
        //Clear the added ingredients list
        kitchenObjectSOList.Clear();

        OnClearVisual?.Invoke();
    }
}
