using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter,IHasProgress
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    private int cuttingProgress;

    public event Action<float> OnProgressChanged;
    public event Action OnCut;
    public static event Action<Transform> OnAnyCut;

    public override void Interact(IKitchenObjectParent player)
    {
        if (!HasKitchenObject())
        {
            //There is no kitchen object here
            if (player.HasKitchenObject())
            {
                //Player is carrying something
                if (HasOutputForInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //Player has object with valid recipe
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    //Reset Cutting progress when player put something on the table
                    cuttingProgress = 0;


                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeFromInput(GetKitchenObject().GetKitchenObjectSO());
                    OnProgressChanged?.Invoke((float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax);
                }
            }
            else
            {
                //Player is not carying anything
            }
        }
        else
        {
            //There is a kitchen object here
            if (player.HasKitchenObject())
            {
                //Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //We add the kitchen object present here as an ingredient to plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }

                }
            }
            else
            {
                //Player is empty handed
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(IKitchenObjectParent kitchenObjectParent)
    {
        if (HasKitchenObject() && HasOutputForInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;

            OnCut?.Invoke();
            OnAnyCut?.Invoke(transform);

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeFromInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke((float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax);

            //If we have done the max cuts then cut the object
            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetInputFromOutput(GetKitchenObject().GetKitchenObjectSO());

                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }

    private KitchenObjectSO GetInputFromOutput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeFromInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private bool HasOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeFromInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    private CuttingRecipeSO GetCuttingRecipeFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }

    public new static void ResetStaticData()
    {
        OnAnyCut = null;
    }
}
