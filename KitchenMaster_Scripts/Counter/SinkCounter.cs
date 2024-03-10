using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkCounter : BaseCounter,IHasProgress
{
    [SerializeField] private List<CleaningRecipeSO> cleaningRecipeSOList;
    private CleaningRecipeSO cleaningRecipeSO;

    private float cleaningProgress;

    public event Action<float> OnProgressChanged;
    public event Action OnUnCleanPlateDropped;
    public event Action OnCleanPlatePicked;

    private bool hasUncleanPlate;

    public override void Interact(IKitchenObjectParent player)
    {
        if (player.GetKitchenObject() && cleaningRecipeSO == null)
        {
            cleaningRecipeSO = TryGetCleaningRecipeSO(player.GetKitchenObject().GetKitchenObjectSO());
            //Check if we have output for the input
            if (cleaningRecipeSO != null)
            {
                //We have the correct input
                player.GetKitchenObject().DestroySelf();

                OnUnCleanPlateDropped?.Invoke();

                hasUncleanPlate = true;
            }
        }
        else
        {
            //Player is empty handed
        }
    }

    public override void InteractAlternate(IKitchenObjectParent kitchenObjectParent)
    {
        if (cleaningRecipeSO != null)
        {
            cleaningProgress++;

            OnProgressChanged?.Invoke((float)cleaningProgress / cleaningRecipeSO.cleaningProgressMax);

            if (cleaningProgress >= cleaningRecipeSO.cleaningProgressMax)
            {
                //We have cleaned the object
                //Give the output of recipe to player
                KitchenObject.SpawnKitchenObject(cleaningRecipeSO.output, kitchenObjectParent);

                cleaningProgress = 0;
                cleaningRecipeSO = null;

                OnCleanPlatePicked?.Invoke();

                hasUncleanPlate = false;
            }
        }
    }

    private CleaningRecipeSO TryGetCleaningRecipeSO(KitchenObjectSO input)
    {
        foreach (CleaningRecipeSO cleaningRecipeSO in cleaningRecipeSOList)
        {
            if (input == cleaningRecipeSO.input)
            {
                return cleaningRecipeSO;
            }
        }
        return null;
    }

    public override bool HasPlateToClean()
    {
        return hasUncleanPlate;
    }
}
