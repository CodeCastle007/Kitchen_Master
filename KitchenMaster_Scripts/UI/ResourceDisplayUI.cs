using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceDisplayUI : MonoBehaviour
{
    [SerializeField] private Transform resourceUiTemplate;
    [SerializeField] private Transform resourceUITemplateParent;

    private void Start(){
        SetResourceCountText();
       
        ResourceManager.Instace.OnResourceCountChanged += ResourceManager_OnresourceCountChanged; 
    }

    private void ResourceManager_OnresourceCountChanged(){
        SetResourceCountText();
    }

    private void SetResourceCountText(){

        //Destry all other gameObjects
        foreach (Transform child in resourceUITemplateParent)
        {
            if (child == resourceUiTemplate)
                continue;

            Destroy(child.gameObject);
        }

        ResourceManager.Resource[] resourceArray = ResourceManager.Instace.GetResourceArray();
        for (int i = 0; i < resourceArray.Length; i++)
        {
            Transform instantiatedTransform = Instantiate(resourceUiTemplate, resourceUITemplateParent);

            instantiatedTransform.GetComponent<ResourceTemplateUI>().SetResourceAmountandIcon(resourceArray[i].resourceAmount, resourceArray[i].kitchenObjectSO.icon);
            instantiatedTransform.gameObject.SetActive(true);
        }
    }

    }
