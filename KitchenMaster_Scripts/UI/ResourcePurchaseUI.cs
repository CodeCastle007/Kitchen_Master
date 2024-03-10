using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class ResourcePurchaseUI : MonoBehaviour
{
    [SerializeField] private Transform resourcePurchaseUiTemplate;
    [SerializeField] private Transform resourcePurchaseUiParent;
    [SerializeField] private string messageText;

    private ResourceManager.Resource[] resourceArray;
    private List<ResourcePurchaseTemplateUI> resourcePurchaseTemplateUIList;

    private void Start()
    {
        resourcePurchaseTemplateUIList = new List<ResourcePurchaseTemplateUI>();
        resourceArray = ResourceManager.Instace.GetResourceArray();

        resourcePurchaseUiTemplate.gameObject.SetActive(false);

        ManagerUI.Instance.OnPurchaseConfirm += ManagerUi_OnPurchase;
        ManagerUI.Instance.OnManagerUIHide += ManagerUI_OnManagerUIHide;

        SetUiTemplates();
    }

    private void OnEnable()
    {
        if (MessageTextManager.Instance != null)
        {
            MessageTextManager.Instance.DisplayTutorialMessage(messageText);
        }
    }

    private void ManagerUI_OnManagerUIHide()
    {
        ResetPurchaseCountOfAllTemplates();
    }
    private void ManagerUi_OnPurchase(float obj)
    {
        //We will set the new counts
        SetResourceArrayPurchaseCounts();

        ResetPurchaseCountOfAllTemplates();

        //Give array to Resource Manager to set the display counts
        ResourceManager.Instace.SetResourceArray();
    }



    private void SetUiTemplates()
    {
        //Destroy extra UI templates
        foreach (Transform child in resourcePurchaseUiParent)
        {
            if (child == resourcePurchaseUiTemplate) continue;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < resourceArray.Length; i++)
        {
            Transform transformUI = Instantiate(resourcePurchaseUiTemplate, resourcePurchaseUiParent);

            ResourcePurchaseTemplateUI resourceTemplateUI = transformUI.GetComponent<ResourcePurchaseTemplateUI>();
            resourceTemplateUI.SetKitchenObjectSOValues(resourceArray[i].kitchenObjectSO);

            //we will add it in the list to keep for later use
            resourcePurchaseTemplateUIList.Add(resourceTemplateUI);

            transformUI.gameObject.SetActive(true);
        }
    }

    private void SetResourceArrayPurchaseCounts()
    {
        for (int i = 0; i < resourceArray.Length; i++)
        {
            resourceArray[i].resourceAmount += resourcePurchaseTemplateUIList[i].GetPurchaseCount();
        }
    }

    private void ResetPurchaseCountOfAllTemplates()
    {
        foreach (Transform child in resourcePurchaseUiParent)
        {
            child.GetComponent<ResourcePurchaseTemplateUI>().ResetPurchaseCount();
        }
    }
}
