using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        DeliveryManager.Instance.OnDeliveryFail += Instance_OnDeliveryFail;

        recipeTemplate.gameObject.SetActive(false);

        UpdateVisual();
    }

    private void Instance_OnDeliveryFail()
    {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeCompleted()
    {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawned(RecipeSO obj)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (KeyValuePair<DeliveryTable,RecipeSO> recipeSOTable in DeliveryManager.Instance.GetWaitingRecipeSOTableList())
        {
            Transform recipeVisual = Instantiate(recipeTemplate, container);
            recipeVisual.gameObject.SetActive(true);

            //Setting template visual
            recipeVisual.GetComponent<RecipeTemplateUI>().SetRecipe(recipeSOTable.Value, recipeSOTable.Key.GetTableNumber());
        }
    }
}
