using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryTable : BaseCounter
{
    private bool isOccupied;
    private Customer customer;

    [SerializeField] private Transform[] sittingPositionArray;
    [SerializeField] private KitchenObjectSO uncleanPlateSO;

    [SerializeField] private int tableNumber;
    public event Action<int> SetNumberUI;

    public static event Action<Transform> OnAnySuccessfulDelivery;
    public static event Action<Transform> OnAnyFailedDelivery;

    public event Action OnSuccessfulDelivery;
    public event Action OnFailedDelivery;

    private bool correctRecipeDelivered = true;

    public static event Action<DeliveryTable> OnAnyTableUnoccupied;

    private void Start()
    {

        SetNumberUI?.Invoke(tableNumber);
    }

    public override void Interact(IKitchenObjectParent player)
    {
        if (player.HasKitchenObject() && !HasKitchenObject() && IsOccupied())
        {
            //Only accepts clean plate
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject) && player.GetKitchenObject().GetKitchenObjectSO() != uncleanPlateSO)
            {
                //Delivering recipe to Delivery manager
                if(DeliveryManager.Instance.TryDeliveringRecipe(plateKitchenObject, this))
                {
                    //Player delivered right recipe
                    Debug.Log("Correct recipe Delivered");
                    correctRecipeDelivered = true;

                    customer.DeliveredCorrectRecipe();

                    OnAnySuccessfulDelivery?.Invoke(transform);
                    OnSuccessfulDelivery?.Invoke();
                }
                else
                {
                    //Player delivered Incorrect Recipe
                    Debug.Log("Incorrect Recipe Delivered");
                    correctRecipeDelivered = false;

                    customer.DeliveredIncorrectRecipe();

                    OnAnyFailedDelivery?.Invoke(transform);
                    OnFailedDelivery?.Invoke();
                }

                //Put plate on table
                player.GetKitchenObject().SetKitchenObjectParent(this);

                //Only accepts plate
                //player.GetKitchenObject().DestroySelf();
            }
        }
        else
        {
            //Only give player the plate
            if (HasKitchenObject())
            {
                if(GetKitchenObject().GetKitchenObjectSO() == uncleanPlateSO || !correctRecipeDelivered)
                {
                    //We have a kitchen object plate
                    //We give it to player
                    GetKitchenObject().SetKitchenObjectParent(player);

                    //Reset it or employee will try to get a plate with wrong recipe
                    correctRecipeDelivered = true;
                }
            }
        }
    }

    public void FoodEaten()
    {
        //Destry the plate on table and spawn a unclean plate
        GetKitchenObject().DestroySelf();
        KitchenObject.SpawnKitchenObject(uncleanPlateSO, this);
        //ToggleOccupied();
    }

    public Transform GetSittingPosition()
    {
        return sittingPositionArray[UnityEngine.Random.Range(0, 2)];
    }


    public bool IsOccupied()
    {
        return isOccupied;
    }
    public void ToggleOccupied()
    {
        isOccupied = !isOccupied;

        if (!isOccupied)
        {
            OnAnyTableUnoccupied?.Invoke(this);
        }
    }


    public void SetCustomer(Customer customer)
    {
        this.customer = customer;
    }
    public void ClearCustomer()
    {
        customer = null;
    }
    public int GetTableNumber()
    {
        return tableNumber;
    }


    public override bool HasPlateToClean()
    {
        if (HasKitchenObject())
        {
            //Check if table has an unclean plate (Correct recipedelivered)
            //Or plate with wrong recipe (Icorrect recipe delivered)
            return GetKitchenObject().GetKitchenObjectSO() == uncleanPlateSO || correctRecipeDelivered==false;
        }
        else
        {
            return false;
        }
    }

    new public static void ResetStaticData()
    {
        OnAnySuccessfulDelivery = null;
        OnAnyFailedDelivery = null;
    }
}
