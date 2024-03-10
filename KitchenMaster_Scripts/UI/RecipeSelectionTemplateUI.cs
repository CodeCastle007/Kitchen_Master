using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeSelectionTemplateUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI recipeName;

    [SerializeField] private Transform recipeIconParent;
    [SerializeField] private Transform recipeIconTemplate;

    [SerializeField] private Toggle selectionToggle;

    private RecipeSO recipeSO;

    private bool selected;
    public static event Action<RecipeSO> OnAnyRecipeSelected;
    public static event Action<RecipeSO> OnAnyRecipeDeselected;

    private void Awake()
    {
        selectionToggle.onValueChanged.AddListener((bool value) =>
        {
            ToggleSelection(value);
        });
    }

    private void Start()
    {
        recipeIconTemplate.gameObject.SetActive(false);
    }

    private void SetTemplates()
    {
        foreach (Transform child in recipeIconParent)
        {
            if (child == recipeIconTemplate) continue;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < recipeSO.kitchenObjectSOList.Count; i++)
        {
            Transform iconTemplateTransform = Instantiate(recipeIconTemplate, recipeIconParent);

            //Setting the icon to the template
            iconTemplateTransform.GetComponent<RecipeIconTemplateUI>().SetIcon(recipeSO.kitchenObjectSOList[i].icon);

            iconTemplateTransform.gameObject.SetActive(true);
        }
    }

    public void SetRecipeSO(RecipeSO recipeSO)
    {
        this.recipeSO = recipeSO;

        recipeName.text = recipeSO.recipeName;

        if(this.recipeSO != null)
        {
            SetTemplates();
        }
    }

    private void ToggleSelection(bool value)
    {
        selected = value;
        if (selected)
        {
            OnAnyRecipeSelected?.Invoke(recipeSO);
        }
        else
        {
            OnAnyRecipeDeselected?.Invoke(recipeSO);
        }
    }
}
