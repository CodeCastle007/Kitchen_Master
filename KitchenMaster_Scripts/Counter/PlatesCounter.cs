using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private int plateSpawnCount;
    private int plateSpawnCountMax = 5;

    public event Action OnPlateSpawned;
    public event Action OnPlateRemoved;

  


    private void Start()
    {
        for (int i = 0; i < plateSpawnCountMax; i++)
        {
            plateSpawnCount++;

            //Firing the event for visual
            OnPlateSpawned?.Invoke();
        }
    }

    public override void Interact(IKitchenObjectParent player)
    {
        if (!player.HasKitchenObject())
        {
            //Player is empty Handed
            if (plateSpawnCount > 0)
            {
                //We have atleast one plate
                plateSpawnCount--;

                //Assigning plate to plate
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                //Firing event for visual
                OnPlateRemoved?.Invoke();
            }
        }
        else
        {
            //Player has something
            if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //Check if we have a clean plate 
                if (plateKitchenObject.IsClean() && plateKitchenObject.GetKitchenObjectSOList().Count==0)
                {
                    //Player is carrying a plate
                    player.GetKitchenObject().DestroySelf();

                    //We will add the plate to counter
                    plateSpawnCount++;

                    OnPlateSpawned?.Invoke();
                }
            }
        }
    }
}
