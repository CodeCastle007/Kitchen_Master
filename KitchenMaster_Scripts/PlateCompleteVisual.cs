using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [System.Serializable]
    public struct KitchenObjectSoGameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject kitchenObjectVisual;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSoGameObject> kitchenObjectSoGameObjectList;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        plateKitchenObject.OnClearVisual += PlateKitchenObject_OnClearVisual;

        foreach (KitchenObjectSoGameObject kitchenObjectSOGameObject in kitchenObjectSoGameObjectList)
        {
            kitchenObjectSOGameObject.kitchenObjectVisual.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnClearVisual()
    {
        foreach (KitchenObjectSoGameObject kitchenObjectSOGameObject in kitchenObjectSoGameObjectList)
        {
            kitchenObjectSOGameObject.kitchenObjectVisual.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(KitchenObjectSO addedIngredient)
    {
        foreach (KitchenObjectSoGameObject kitchenObjectSOGameObject in kitchenObjectSoGameObjectList)
        {
            if (kitchenObjectSOGameObject.kitchenObjectSO == addedIngredient)
            {
                kitchenObjectSOGameObject.kitchenObjectVisual.SetActive(true);
            }
        }
    }
}
