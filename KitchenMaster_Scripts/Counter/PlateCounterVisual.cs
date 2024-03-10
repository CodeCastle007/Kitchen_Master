using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCounterVisual : MonoBehaviour
{
    [SerializeField] private Transform tableTopPoint;
    [SerializeField] private Transform platePrefab;
    [SerializeField] private PlatesCounter plateCounter;

    private List<Transform> spawnedPlatesTransformList;

    private void Awake()
    {
        spawnedPlatesTransformList = new List<Transform>();

        plateCounter.OnPlateSpawned += PlateCounter_OnPlateSpawned;
        plateCounter.OnPlateRemoved += PlateCounter_OnPlateRemoved;
    }

    private void Start()
    {
        
    }

    private void PlateCounter_OnPlateRemoved()
    {
        Transform plateToRemove = spawnedPlatesTransformList[spawnedPlatesTransformList.Count - 1];
        spawnedPlatesTransformList.Remove(plateToRemove);

        Destroy(plateToRemove.gameObject);
    }

    private void PlateCounter_OnPlateSpawned()
    {
        Transform spawnedPlate = Instantiate(platePrefab, tableTopPoint);
        spawnedPlate.localPosition = new Vector3(0, .1f * spawnedPlatesTransformList.Count, 0);

        spawnedPlatesTransformList.Add(spawnedPlate);
    }
}
