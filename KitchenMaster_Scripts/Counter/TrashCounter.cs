using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO uncleanPlateKitchenObjectSO;
    public static event Action<Transform> OnAnyObjectTrashed;

    public override void Interact(IKitchenObjectParent player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //Check if player is holding a plate
                if (plateKitchenObject.GetKitchenObjectSOList().Count > 0)
                {
                    //There are kitchen objects over plate
                    //Give player the dirty plate
                    player.GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(uncleanPlateKitchenObjectSO, player);

                    OnAnyObjectTrashed?.Invoke(transform);
                }
            }
            else
            {
                //Player is carrying something else
                player.GetKitchenObject().DestroySelf();

                OnAnyObjectTrashed?.Invoke(transform);
            }
        }
    }

    new public static void ResetStaticData()
    {
        OnAnyObjectTrashed = null;
    }
}
