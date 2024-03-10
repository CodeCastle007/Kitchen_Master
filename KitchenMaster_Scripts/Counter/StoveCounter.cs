using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter,IHasProgress
{
    public enum CookingState
    {
        Idle,
        Cooking,
        Cooked,
        Burned
    }

    public event Action<float> OnProgressChanged;
    public event Action<CookingState> OnStateChanged;

    [SerializeField] private CookingRecipeSO[] cookingRecipeSOArray;
    private float cookingTimer;
    private CookingRecipeSO cookingRecipeSO;

    [SerializeField] private BurnedRecipeSO[] burnedRecipeSOArray;
    private float burnedTimer;
    private BurnedRecipeSO burnedRecipeSO;

    private CookingState state;
    
    private void Start()
    {
        state = CookingState.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case CookingState.Idle:
                    break;

                case CookingState.Cooking:

                    cookingTimer += Time.deltaTime;

                    //Firing event for Progress UI
                    OnProgressChanged?.Invoke(cookingTimer / cookingRecipeSO.cookingTimerMax);

                    if (cookingTimer >= cookingRecipeSO.cookingTimerMax)
                    {
                        //Cooked
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(cookingRecipeSO.output, this);

                        //We initialized the burned parameters
                        burnedTimer = 0;
                        burnedRecipeSO = GetBurnedRecipeFromInput(cookingRecipeSO.output);
                        state = CookingState.Cooked;

                        OnStateChanged?.Invoke(state);
                    }

                    break;

                case CookingState.Cooked:

                    burnedTimer += Time.deltaTime;

                    //Firing event for Progress UI
                    OnProgressChanged?.Invoke(burnedTimer / burnedRecipeSO.burnedTimerMAx);

                    if (burnedTimer >= burnedRecipeSO.burnedTimerMAx)
                    {
                        //Burned
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burnedRecipeSO.output, this);

                        state = CookingState.Burned;

                        OnStateChanged?.Invoke(state);
                    }
                    break;

                case CookingState.Burned:
                    break;
            }
        }
    }

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

                    cookingTimer = 0;
                    cookingRecipeSO = GetCookingRecipeFromInput(GetKitchenObject().GetKitchenObjectSO());

                    state = CookingState.Cooking;

                    //Firing event for Progress UI
                    OnProgressChanged?.Invoke(0);

                    OnStateChanged?.Invoke(state);
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

                        cookingTimer = 0;
                        state = CookingState.Idle;

                        //Firing event for Progress UI
                        OnProgressChanged?.Invoke(0);

                        OnStateChanged?.Invoke(state);
                    }

                }
            }
            else
            {
                //Player is empty handed
                GetKitchenObject().SetKitchenObjectParent(player);

                cookingTimer = 0;
                state = CookingState.Idle;

                //Firing event for Progress UI
                OnProgressChanged?.Invoke(0);

                OnStateChanged?.Invoke(state);
            }
        }
    }

    private KitchenObjectSO GetInputFromOutput(KitchenObjectSO inputKitchenObjectSO)
    {
        CookingRecipeSO cookingRecipeSO = GetCookingRecipeFromInput(inputKitchenObjectSO);
        if (cookingRecipeSO != null)
        {
            return cookingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private bool HasOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CookingRecipeSO cookingRecipeSO = GetCookingRecipeFromInput(inputKitchenObjectSO);
        return cookingRecipeSO != null;
    }

    private CookingRecipeSO GetCookingRecipeFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CookingRecipeSO cookingRecipeSO in cookingRecipeSOArray)
        {
            if (cookingRecipeSO.input == inputKitchenObjectSO)
            {
                return cookingRecipeSO;
            }
        }
        return null;
    }

    private BurnedRecipeSO GetBurnedRecipeFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurnedRecipeSO burnedRecipeSO in burnedRecipeSOArray)
        {
            if (burnedRecipeSO.input == inputKitchenObjectSO)
            {
                return burnedRecipeSO;
            }
        }
        return null;
    }

    public bool PattyCooked()
    {
        return state == CookingState.Cooked;
    }
}
