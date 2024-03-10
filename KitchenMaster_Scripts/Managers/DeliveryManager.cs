using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event Action<RecipeSO> OnRecipeSpawned;
    public event Action OnRecipeCompleted;
    public event Action OnDeliveryFail;

    //[SerializeField] private RecipeListSO recipeListSO;

    //private List<RecipeSO> waitingRecipeSOList;
    private Dictionary<DeliveryTable, RecipeSO> waitingRecipeSO_Table;

    private int recipeCountMax = 6;
    private int deliveredRecipesCount;

    public static DeliveryManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        waitingRecipeSO_Table = new Dictionary<DeliveryTable, RecipeSO>();
    }

    private void Start()
    {
        Customer.OnCustomerWaitingTimerEnd += Customer_OnCustomerWaitingTimerEnd;
    }

    private void Customer_OnCustomerWaitingTimerEnd(DeliveryTable deliveryTable)
    {
        //Remove the customer order from list
        waitingRecipeSO_Table.Remove(deliveryTable);

        //Firing the event for UI
        OnDeliveryFail.Invoke();
    }

    public RecipeSO PlaceOrder(DeliveryTable deliveryTable)
    {
        if (waitingRecipeSO_Table.Count < recipeCountMax)
        {
            //Getting a random recipe
            RecipeSO spawnedRecipe = RecipeSelectionManager.Instance.GetRandomRecipeSO();

            waitingRecipeSO_Table.Add(deliveryTable, spawnedRecipe);

            //Firing the event for UI
            OnRecipeSpawned?.Invoke(spawnedRecipe);

            return spawnedRecipe;
        }
        return null;
    }

    //private RecipeSO GenerateRandomRecipe()
    //{
    //    //Get a random number to determine the number of items in recipe to order
    //    int items = UnityEngine.Random.Range(1, 4);

    //    RecipeSO generatedRecipeSO = ScriptableObject.CreateInstance<RecipeSO>();

    //    //Initializing the list for preventing null reference exception
    //    generatedRecipeSO.kitchenObjectSOList = new List<KitchenObjectSO>();


    //    for (int i = 0; i < items; i++)
    //    {
    //        //Get a random recipe from list
    //        RecipeSO recipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

    //        //Itterate through the recipe kitvhen object list and add them in generated recipe
    //        for (int j = 0; j < recipeSO.kitchenObjectSOList.Count; j++)
    //        {
    //            //Check if we already have the item then donot add it
    //            if (!generatedRecipeSO.kitchenObjectSOList.Contains(recipeSO.kitchenObjectSOList[j]))
    //            {
    //                generatedRecipeSO.kitchenObjectSOList.Add(recipeSO.kitchenObjectSOList[j]);
    //            }
    //        }
    //        //Add the cost
    //        generatedRecipeSO.cost += recipeSO.cost;

    //        if (i == 0)
    //        {
    //            //If itteration is first then set the name
    //            generatedRecipeSO.recipeName = recipeSO.recipeName;
    //        }
    //        else
    //        {
    //            //else add the + symbol and concatenate the string
    //            generatedRecipeSO.recipeName += " + " + recipeSO.recipeName;
    //        }
    //    }

    //    return generatedRecipeSO;
    //}


    public bool TryDeliveringRecipe(PlateKitchenObject plateKitchenObject, DeliveryTable deliveryTable)
    {
        foreach (KeyValuePair<DeliveryTable,RecipeSO> waitingRecipe_Table in waitingRecipeSO_Table)
        {
            if (deliveryTable == waitingRecipe_Table.Key)
            {
                //We have delivered to right table
                RecipeSO waitingRecipeSO = waitingRecipe_Table.Value;

                if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
                {
                    //we have same number of ingredients
                    bool plateContentMathesRecipe = true;
                    foreach (KitchenObjectSO waitingKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                    {
                        //Cycling through all the ingredients in recipe
                        bool ingredientfound = false;
                        foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                        {
                            //Cycling through all the ingredients in the recipe
                            if (waitingKitchenObjectSO == plateKitchenObjectSO)
                            {
                                //Ingredient found
                                ingredientfound = true;
                                break;
                            }
                        }
                        if (!ingredientfound)
                        {
                            //This recipe ingredient was not found on the plate
                            plateContentMathesRecipe = false;
                        }
                    }
                    if (plateContentMathesRecipe)
                    {
                        //Increasing the count
                        deliveredRecipesCount++;

                        //Player has Delivered the correct recipe
                        waitingRecipeSO_Table.Remove(deliveryTable);

                        //Firing the event for UI
                        OnRecipeCompleted.Invoke();

                        return true;
                    }

                }
            }
        }
        //Still remove the recipe if player delivered wrong one bcz customer leaves
        waitingRecipeSO_Table.Remove(deliveryTable);
        OnDeliveryFail?.Invoke();
        return false;
        //No mathes found
        //Player did not delivered a correct recipe
    }

    public Dictionary<DeliveryTable,RecipeSO> GetWaitingRecipeSOTableList()
    {
        return waitingRecipeSO_Table;
    }
    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();
        foreach (KeyValuePair<DeliveryTable, RecipeSO> recipeSOTable in DeliveryManager.Instance.GetWaitingRecipeSOTableList())
        {
            waitingRecipeSOList.Add(recipeSOTable.Value);
        }

        return waitingRecipeSOList;
    }

    public int GetDeliveredRecipeCount()
    {
        return deliveredRecipesCount;
    }
    public bool CanPlaceOrder()
    {
        return waitingRecipeSO_Table.Count < recipeCountMax;
    }
}
