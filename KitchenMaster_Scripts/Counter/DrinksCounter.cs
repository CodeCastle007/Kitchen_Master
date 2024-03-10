using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinksCounter : BaseCounter,IHasProgress
{
    public enum FillingState
    {
        Idle,
        Filling,
        Filled,
    }

    public event Action<float> OnProgressChanged;
    public event Action<FillingState> OnStateChanged;

    [SerializeField] private FillingRecipeSO[] fillingRecipeSOArray;
    private float fillingTimer;
    private FillingRecipeSO fillingRecipeSO;

    private FillingState state;

    private void Start()
    {
        state = FillingState.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case FillingState.Idle:
                    break;

                case FillingState.Filling:

                    fillingTimer += Time.deltaTime;

                    //Firing event for Progress UI
                    OnProgressChanged?.Invoke(fillingTimer / fillingRecipeSO.fillingTimerMax);

                    if (fillingTimer >= fillingRecipeSO.fillingTimerMax)
                    {
                        //Cooked
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fillingRecipeSO.output, this);

                        state = FillingState.Filled;

                        OnStateChanged?.Invoke(state);
                    }

                    break;

                case FillingState.Filled:
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

                    fillingTimer = 0;
                    fillingRecipeSO = GetFillingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());

                    state = FillingState.Filling;

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

                        fillingTimer = 0;
                        state = FillingState.Idle;

                        //Firing event for Progress UI
                        OnProgressChanged?.Invoke(0);

                        OnStateChanged?.Invoke(state);
                    }

                }
                else
                {
                    //Player is carrying something else and there is something over here
                    if (HasOutputForInput(player.GetKitchenObject().GetKitchenObjectSO()))
                    {
                        //Player is carrying and empty glass and there is a glass already over here then destry the glass placed here
                        GetKitchenObject().DestroySelf();

                        //Player has object with valid recipe
                        player.GetKitchenObject().SetKitchenObjectParent(this);

                        fillingTimer = 0;
                        fillingRecipeSO = GetFillingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());

                        state = FillingState.Filling;

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

                fillingTimer = 0;
                state = FillingState.Idle;

                //Firing event for Progress UI
                OnProgressChanged?.Invoke(0);

                OnStateChanged?.Invoke(state);
            }
        }
    }

    private bool HasOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FillingRecipeSO fillingRecipeSO = GetFillingRecipeSOFromInput(inputKitchenObjectSO);
        return fillingRecipeSO != null;
    }

    private FillingRecipeSO GetFillingRecipeSOFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FillingRecipeSO fillingRecipeSO in fillingRecipeSOArray)
        {
            if (fillingRecipeSO.input == inputKitchenObjectSO)
            {
                return fillingRecipeSO;
            }
        }
        return null;
    }

    public bool GlassFilled()
    {
        return state == FillingState.Filled;
    }

}
