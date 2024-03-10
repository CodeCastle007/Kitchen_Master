using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CounterTransformHandler : MonoBehaviour
{
    public static CounterTransformHandler Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private Transform[] clearCounterArray;

    [SerializeField] private Transform[] deliveryTableArray;

    [SerializeField] private Transform[] plateCounterTransformArray;

    [SerializeField] private Transform[] sinkCounterTransformArray;

    [SerializeField] private Transform[] trashCounterTransformArray;

    [SerializeField] private Transform[] glassCounterTransformArray;
    [SerializeField] private Transform[] drinksCounterTransformArray;

    [SerializeField] private Transform[] friesCounterTransformArray;
    [SerializeField] private Transform[] fryingCounterTransformArray;

    [SerializeField] private Transform[] meatPattyCounterTransformArray;
    [SerializeField] private Transform[] stoveCounterTransformArray;

    public Transform GetNearestSinkCounterTransform(Vector3 position)
    {
        float nearestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < sinkCounterTransformArray.Length; i++)
        {
            if (Vector3.Distance(position, sinkCounterTransformArray[i].position) < nearestDistance)
            {
                nearestDistance = Vector3.Distance(position, sinkCounterTransformArray[i].position);
                index = i;
            }
        }

        return sinkCounterTransformArray[index];
    }
    public Transform GetNearestPlatesCounterTransform(Vector3 position)
    {
        float nearestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < plateCounterTransformArray.Length; i++)
        {
            if (Vector3.Distance(position, plateCounterTransformArray[i].position) < nearestDistance)
            {
                nearestDistance = Vector3.Distance(position, plateCounterTransformArray[i].position);
                index = i;
            }
        }

        return plateCounterTransformArray[index];
    }
    public Transform GetNearestTrashCounterTransform(Vector3 position)
    {
        float nearestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < trashCounterTransformArray.Length; i++)
        {
            if (Vector3.Distance(position, trashCounterTransformArray[i].position) < nearestDistance)
            {
                nearestDistance = Vector3.Distance(position, trashCounterTransformArray[i].position);
                index = i;
            }
        }

        return trashCounterTransformArray[index];
    }

    public Transform GetNearestGlassCounterTransform(Vector3 position)
    {
        float nearestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < glassCounterTransformArray.Length; i++)
        {
            if (Vector3.Distance(position, glassCounterTransformArray[i].position) < nearestDistance)
            {
                nearestDistance = Vector3.Distance(position, glassCounterTransformArray[i].position);
                index = i;
            }
        }

        return glassCounterTransformArray[index];
    }
    public Transform GetNearestDrinksCounterTransform(Vector3 position)
    {
        float nearestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < drinksCounterTransformArray.Length; i++)
        {
            if (Vector3.Distance(position, drinksCounterTransformArray[i].position) < nearestDistance)
            {
                nearestDistance = Vector3.Distance(position, drinksCounterTransformArray[i].position);
                index = i;
            }
        }

        return drinksCounterTransformArray[index];
    }

    public Transform GetNearestFriesCounterTransform(Vector3 position)
    {
        float nearestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < friesCounterTransformArray.Length; i++)
        {
            if (Vector3.Distance(position, friesCounterTransformArray[i].position) < nearestDistance)
            {
                nearestDistance = Vector3.Distance(position, friesCounterTransformArray[i].position);
                index = i;
            }
        }

        return friesCounterTransformArray[index];
    }
    public Transform GetNearestFryingCounterTransform(Vector3 position)
    {
        float nearestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < fryingCounterTransformArray.Length; i++)
        {
            if (Vector3.Distance(position, fryingCounterTransformArray[i].position) < nearestDistance)
            {
                nearestDistance = Vector3.Distance(position, fryingCounterTransformArray[i].position);
                index = i;
            }
        }

        return fryingCounterTransformArray[index];
    }

    public Transform GetNearestMeatPattyCounterTransform(Vector3 position)
    {
        float nearestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < meatPattyCounterTransformArray.Length; i++)
        {
            if (Vector3.Distance(position, meatPattyCounterTransformArray[i].position) < nearestDistance)
            {
                nearestDistance = Vector3.Distance(position, meatPattyCounterTransformArray[i].position);
                index = i;
            }
        }

        return meatPattyCounterTransformArray[index];
    }
    public Transform GetNearestStoveCounterTransform(Vector3 position)
    {
        float nearestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < stoveCounterTransformArray.Length; i++)
        {
            if (Vector3.Distance(position, stoveCounterTransformArray[i].position) < nearestDistance)
            {
                nearestDistance = Vector3.Distance(position, stoveCounterTransformArray[i].position);
                index = i;
            }
        }

        return stoveCounterTransformArray[index];
    }

    public Transform GetNearestClearCounter(Vector3 position)
    {
        float nearestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < clearCounterArray.Length; i++)
        {
            if (Vector3.Distance(position, clearCounterArray[i].position) < nearestDistance && !clearCounterArray[i].GetComponent<BaseCounter>().HasKitchenObject())
            {
                nearestDistance = Vector3.Distance(position, clearCounterArray[i].position);
                index = i;
            }
        }

        return clearCounterArray[index];
    }
    public BaseCounter GetCounterWithUnCleanPlate()
    {
        //Search all clear counter
        for (int i = 0; i < clearCounterArray.Length; i++)
        {
            if (clearCounterArray[i].TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter.HasPlateToClean())
                {
                    return baseCounter;
                }
            }
        }

        //Search all delivery tables
        for (int i = 0; i < deliveryTableArray.Length; i++)
        {
            if (deliveryTableArray[i].TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter.HasPlateToClean())
                {
                    return baseCounter;
                }
            }
        }

        return null;
    }
}
