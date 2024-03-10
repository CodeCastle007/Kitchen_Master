using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeSelectionUI : MonoBehaviour
{
    [SerializeField] private RecipeListSO basicRecipeListSO;

    [SerializeField] private Transform recipeSelectionUIParent;
    [SerializeField] private Transform recipeSelectionUITemplate;
    [SerializeField] private string messageText;

    private void Start()
    {
        recipeSelectionUITemplate.gameObject.SetActive(false);


        SetTemplates();
    }

    private void OnEnable()
    {
        if (MessageTextManager.Instance != null)
        {
            MessageTextManager.Instance.DisplayTutorialMessage(messageText);
        }
    }

    private void SetTemplates()
    {
        foreach (Transform child in recipeSelectionUIParent)
        {
            if (child == recipeSelectionUITemplate) continue;
            Destroy(child.gameObject);
        }


        for (int i = 0; i < basicRecipeListSO.recipeSOList.Count; i++)
        {
            Transform templateTransform = Instantiate(recipeSelectionUITemplate, recipeSelectionUIParent);

            //Set recipe SO to the template
            templateTransform.GetComponent<RecipeSelectionTemplateUI>().SetRecipeSO(basicRecipeListSO.recipeSOList[i]);

            templateTransform.gameObject.SetActive(true);
        }
    }
}
