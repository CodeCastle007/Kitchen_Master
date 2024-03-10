using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryTableManager : MonoBehaviour
{
    public static DeliveryTableManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private List<DeliveryTable> deliveryTableList;

    private List<DeliveryTable> emptyTableList;
    private List<DeliveryTable> occupiedTableList;

    private void Start()
    {
        DeliveryTable.OnAnyTableUnoccupied += DeliveryTable_OnAnyTableUnoccupied;

        emptyTableList = new List<DeliveryTable>(deliveryTableList);
        occupiedTableList = new List<DeliveryTable>();

    }

    private void DeliveryTable_OnAnyTableUnoccupied(DeliveryTable obj)
    {
        if (occupiedTableList.Contains(obj))
        {
            occupiedTableList.Remove(obj);
        }

        if (!emptyTableList.Contains(obj))
        {
            emptyTableList.Add(obj);
        }
    }

    public bool HasEmptytable()
    {
        return emptyTableList.Count > 0;
    }

    public DeliveryTable GetEmptyTable()
    {
        int index = Random.Range(0, emptyTableList.Count);
        DeliveryTable deliveryTable = emptyTableList[index];

        occupiedTableList.Add(deliveryTable);
        emptyTableList.Remove(deliveryTable);
        deliveryTable.ToggleOccupied();

        return deliveryTable;
    }

    public List<DeliveryTable> GetDeliveryTableList()
    {
        return deliveryTableList;
    }
}
