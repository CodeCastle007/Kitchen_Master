using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeTemplateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI tableNumberText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private void Start()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipe(RecipeSO recipeSO, int tableNumber)
    {
        nameText.text = recipeSO.recipeName;
        tableNumberText.text = "Table: " + tableNumber;

        foreach (Transform child in iconContainer)
        {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach(KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            Transform iconTemplateTransform = Instantiate(iconTemplate, iconContainer);
            iconTemplateTransform.gameObject.SetActive(true);

            iconTemplateTransform.GetComponentInChildren<Image>().sprite = kitchenObjectSO.icon;
        }
    }
}
