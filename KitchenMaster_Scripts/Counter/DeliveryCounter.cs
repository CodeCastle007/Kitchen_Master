using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(IKitchenObjectParent player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //Delivering recipe to Delivery manager
               // DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                //Only accepts plate
                player.GetKitchenObject().DestroySelf();
            }
        }
    }

}
