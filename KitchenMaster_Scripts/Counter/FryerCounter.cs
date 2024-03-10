using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FryerCounter : BaseCounter,IHasProgress
{
    public enum FryingState
    {
        Idle,
        Cooking,
        Cooked,
        Burned
    }

    public event Action<float> OnProgressChanged;
    public event Action<FryingState> OnStateChange;

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;

    [SerializeField] private BurnedRecipeSO[] burnedRecipeSOArray;
    private float burnedTimer;
    private BurnedRecipeSO burnedRecipeSO;

    private FryingState state;

    private void Start()
    {
        state = FryingState.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case FryingState.Idle:
                    break;

                case FryingState.Cooking:

                    fryingTimer += Time.deltaTime;

                    //Firing event for Progress UI
                    OnProgressChanged?.Invoke(fryingTimer / fryingRecipeSO.fryingTimerMax);

                    if (fryingTimer >= fryingRecipeSO.fryingTimerMax)
                    {
                        //Cooked
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        //We initialized the burned parameters
                        burnedTimer = 0;
                        burnedRecipeSO = GetBurnedRecipeFromInput(fryingRecipeSO.output);
                        state = FryingState.Cooked;

                        OnStateChange?.Invoke(state);
                    }

                    break;

                case FryingState.Cooked:

                    burnedTimer += Time.deltaTime;

                    //Firing event for Progress UI
                    OnProgressChanged?.Invoke(burnedTimer / burnedRecipeSO.burnedTimerMAx);

                    if (burnedTimer >= burnedRecipeSO.burnedTimerMAx)
                    {
                        //Burned
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burnedRecipeSO.output, this);

                        state = FryingState.Burned;

                        OnStateChange?.Invoke(state);
                    }
                    break;

                case FryingState.Burned:
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
                    fryingTimer = 0;
                    fryingRecipeSO = GetFryingRecipeFromInput(player.GetKitchenObject().GetKitchenObjectSO());

                    //Player has object with valid recipe
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    state = FryingState.Cooking;

                    //Firing event for Progress UI
                    OnProgressChanged?.Invoke(0);

                    OnStateChange?.Invoke(state);
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

                        fryingTimer = 0;
                        state = FryingState.Idle;

                        //Firing event for Progress UI
                        OnProgressChanged?.Invoke(0);

                        OnStateChange?.Invoke(state);
                    }

                }
                else
                {
                    //There is something here and player is also carrying something
                    //Player is carrying something
                    if (HasOutputForInput(player.GetKitchenObject().GetKitchenObjectSO()))
                    {
                        //Player is carrying fries and there are fries over here then destry them
                        GetKitchenObject().DestroySelf();

                        fryingTimer = 0;
                        fryingRecipeSO = GetFryingRecipeFromInput(player.GetKitchenObject().GetKitchenObjectSO());

                        //Player has object with valid recipe
                        player.GetKitchenObject().SetKitchenObjectParent(this);

                        state = FryingState.Cooking;

                        //Firing event for Progress UI
                        OnProgressChanged?.Invoke(0);

                        OnStateChange?.Invoke(state);
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);

                fryingTimer = 0;
                state = FryingState.Idle;

                //Firing event for Progress UI
                OnProgressChanged?.Invoke(0);

                OnStateChange?.Invoke(state);
            }
        }
    }

    private bool HasOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeFromInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private FryingRecipeSO GetFryingRecipeFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
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

    public bool FriesFried()
    {
        return state == FryingState.Cooked;
    }
}
