using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeSelectionManager : MonoBehaviour
{
    #region Singleton
    public static RecipeSelectionManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [SerializeField] private RecipeListSO recipeListSO;

    //TEMPORARILY SERIALIZED // LATER VALUES WILL BE ASSIGNED FROM UI
    //ONLY SIMPLE RECIPES NOT COMPLETE
    [SerializeField] private List<RecipeSO> selectedRecipeSOList;

    //Hold all kitchen object SO which are in selectedRecipeSOList
    private List<KitchenObjectSO> validKitchenObjectSOList;

    //All recipes which are valid to order
    //Customer will order from this list
    [SerializeField] private List<RecipeSO> availableRecipeSOList;

    public event Action OnAvailableRecipeChange;

    private void Start()
    {
        validKitchenObjectSOList = new List<KitchenObjectSO>();
        availableRecipeSOList = new List<RecipeSO>();

        //ONLY FOR DEBUG PURPOSES
        //THIS WILL BE UNCOMMENTED IN FINAL BUILD
        //selectedRecipeSOList = new List<RecipeSO>();

        RecipeSelectionTemplateUI.OnAnyRecipeSelected += RecipeSelectionTemplateUI_OnAnyRecipeSelected;
        RecipeSelectionTemplateUI.OnAnyRecipeDeselected += RecipeSelectionTemplateUI_OnAnyRecipeDeselected;

        SetValidKitchenObjectSOList();
        SetAvailableRecipeSOList();
    }

    private void RecipeSelectionTemplateUI_OnAnyRecipeSelected(RecipeSO obj)
    {
        AddRecipeToSelected(obj);
    }
    private void RecipeSelectionTemplateUI_OnAnyRecipeDeselected(RecipeSO obj)
    {
        RemoveRecipeFromSelected(obj);
    }

    private void SetValidKitchenObjectSOList()
    {
        validKitchenObjectSOList.Clear();

        //Itterate through the recipe list
        for (int i = 0; i < selectedRecipeSOList.Count; i++)
        {
            //Itterate through kitchen objects recipe holds
            for (int j = 0; j < selectedRecipeSOList[i].kitchenObjectSOList.Count; j++)
            {
                //If same object does not already exists
                if (!validKitchenObjectSOList.Contains(selectedRecipeSOList[i].kitchenObjectSOList[j]))
                {
                    validKitchenObjectSOList.Add(selectedRecipeSOList[i].kitchenObjectSOList[j]);
                }
            }
        }
    }

    private void SetAvailableRecipeSOList()
    {
        availableRecipeSOList.Clear();

        //Itterate through each recipe
        foreach (RecipeSO recipeSO in recipeListSO.recipeSOList)
        {
            bool objectsAvailable = true;

            //Itterate through each kitchen object in list
            foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
            {
                if (validKitchenObjectSOList.Contains(kitchenObjectSO))
                {
                    //We have the kitchen object SO in valid list
                }
                else
                {
                    //Object is not available
                    //We are not selling this recipe
                    objectsAvailable = false;
                }
            }

            if (objectsAvailable)
            {
                //All kitchen objects of recipe are available
                //This is a valid recipe
                if (!availableRecipeSOList.Contains(recipeSO))
                {
                    availableRecipeSOList.Add(recipeSO);
                }
            }
        }

        OnAvailableRecipeChange?.Invoke();
    }

    public RecipeSO GetRandomRecipeSO()
    {
        if (availableRecipeSOList.Count == 0)
        {
            return null;
        }

        return availableRecipeSOList[UnityEngine.Random.Range(0, availableRecipeSOList.Count)];
    }

    private void AddRecipeToSelected(RecipeSO recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("The recipe trying to add in null...!!");
        }
        else if (!selectedRecipeSOList.Contains(recipe))
        {
            //Add the recipe to list
            selectedRecipeSOList.Add(recipe);

            SetValidKitchenObjectSOList();
            SetAvailableRecipeSOList();
        }
    }

    private void RemoveRecipeFromSelected(RecipeSO recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("The recipe trying to remove in null...!!");
        }
        else if (selectedRecipeSOList.Contains(recipe))
        {
            //Remove recipe from list
            selectedRecipeSOList.Remove(recipe);

            SetValidKitchenObjectSOList();
            SetAvailableRecipeSOList();
        }
    }

    public bool HasRecipesAvaialable()
    {
        if (availableRecipeSOList.Count > 0)
        {
            //We have recipes to serve
            return true;
        }
        else
        {
            return false;
        }
    }

}
