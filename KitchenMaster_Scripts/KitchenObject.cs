using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private IKitchenObjectParent kitchenObjectParent;

    //For Plate
    [SerializeField] private bool isClean;

    public static event Action<KitchenObjectSO> OnAnyResourceKitchenObjectSpawn;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        //If we already have a kitchen object parent then clear kitchen object from that parent
        if (this.kitchenObjectParent != null)
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }
        //Set new parent
        this.kitchenObjectParent = kitchenObjectParent;

        //If parent already has a Kitchen Object
        if (this.kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("Parent already has a Kitchen Object!!");
        }

        //Tell the parent this is the new kitchen object
        this.kitchenObjectParent.SetKitchenObject(this);

        //Reset the transform
        transform.parent = this.kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if(this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

    public bool IsClean()
    {
        return isClean;
    }


    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        //Check if the kitchen object SO in from the resource
        if(ResourceManager.Instace.IsKitchenObjectResource(kitchenObjectSO)){
             //Check if we have the kitchen object resource abailable
            if(ResourceManager.Instace.HasKitchenObjectResourceCount(kitchenObjectSO)){
                //Spawn kitchen object

                Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, kitchenObjectParent.GetKitchenObjectFollowTransform());

                KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

                kitchenObject.SetKitchenObjectParent(kitchenObjectParent);

                OnAnyResourceKitchenObjectSpawn?.Invoke(kitchenObjectSO);

                return kitchenObject;
            }else{
                //We donot have the resource count
                return null;
            }
        }else{
            //It is not a resource
             Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, kitchenObjectParent.GetKitchenObjectFollowTransform());

            KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

            kitchenObject.SetKitchenObjectParent(kitchenObjectParent);


            return kitchenObject;
        }
    }

    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }
}
