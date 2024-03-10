using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instace {get;private set;}
    private void Awake(){
        Instace=this;
    }


    [Serializable]
    public struct Resource{
    public KitchenObjectSO kitchenObjectSO;
    public int resourceAmount;
   }
    [SerializeField] private Resource[] resourcesArray;

    public event Action OnResourceCountChanged;

    private void Start(){
        KitchenObject.OnAnyResourceKitchenObjectSpawn += KitchenObject_OnAnyKitchenObjectSpawn;
    }


    private void KitchenObject_OnAnyKitchenObjectSpawn(KitchenObjectSO kitchenObjectSO){
        ReduceResource(kitchenObjectSO);
    }

    public void ReduceResource(KitchenObjectSO objectSO){

        int reduceCount=1;
        //Itterate through the whole array
        for(int i=0; i<resourcesArray.Length; i++){
            //If the object matches then reduce the total count
            if(resourcesArray[i].kitchenObjectSO == objectSO){
                resourcesArray[i].resourceAmount -= reduceCount;

                OnResourceCountChanged?.Invoke();
            }
        }
    }

    //Checks the count of Kitchen object SO
    public bool HasKitchenObjectResourceCount(KitchenObjectSO kitchenObjectSO){
        for(int i=0; i < resourcesArray.Length; i++){
            if(resourcesArray[i].kitchenObjectSO == kitchenObjectSO){
                //We have found the correct kitchen object
                if(resourcesArray[i].resourceAmount > 0){
                    //If we have the resource
                    return true;
                }
            }
        }
        return false;
    }

    //Checks if the kitchen object SO is in the list or not
     public bool IsKitchenObjectResource(KitchenObjectSO kitchenObjectSO){
        for(int i=0; i < resourcesArray.Length; i++){
            if(resourcesArray[i].kitchenObjectSO == kitchenObjectSO){
                //We have found the correct kitchen object
               return true;
            }
        }
        return false;
    }
    
    //Called by ResourceManagerUI to set the resource array when the purchase occur to chage the count of products
    public void SetResourceArray()
    {
        OnResourceCountChanged?.Invoke();
    }

    public Resource[] GetResourceArray(){
        return resourcesArray;
    }
}
