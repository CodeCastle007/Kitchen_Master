using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private Transform[] customerPrefabArray;
    [SerializeField] private Transform customerSpawnPoint;

    [SerializeField] private float customerSpawnTimer;
    [SerializeField] private float customerBaseSpawnTimer;
    [SerializeField] private float customerMinSpawnTimer;
    [SerializeField] private float difficultyChangeRate;
    private float customerCurrentSpawnTimer;


    //ONLY FOR DEBUG PURPOSES
    //private int maxCustomers = 5;
    private int currentCustomersCount;

    private void Start()
    {
        customerSpawnTimer = customerBaseSpawnTimer;

        //Only wait 10 seconds before spawning first customer
        customerCurrentSpawnTimer = customerSpawnTimer - 10;

        Customer.OnAnyCustomerServed += Customer_OnAnyCustomerServed;
    }

    private void Customer_OnAnyCustomerServed(float obj)
    {
        //Check in how many time customer is being served
        if (obj < 0.4f)
        {
            //Customer is served in less than 30 seconds
            //Increase Difficulty
            customerSpawnTimer -=  difficultyChangeRate;
            if (customerSpawnTimer <= customerMinSpawnTimer)
            {
                customerSpawnTimer = customerMinSpawnTimer;
            }

            Debug.Log("Difficulty Increased");
        }
        else if(obj > 0.6)
        {
            //Customer is served in more than 70 seconds
            //Decrease difficulty
            customerSpawnTimer += (difficultyChangeRate/2);
            if (customerSpawnTimer >= customerBaseSpawnTimer)
            {
                customerSpawnTimer = customerBaseSpawnTimer;
            }

            Debug.Log("Difficulty Decreased");
        }
        else
        {
            //customer is served in 40 to 60 seconds
            //Nochange
            Debug.Log("Difficulty Remain Same");
        }
    }

    private void Update()
    {
        if (GameHandler.Instance.IsGamePlaying())
        {
            customerCurrentSpawnTimer += Time.deltaTime;
            if (customerCurrentSpawnTimer >= customerSpawnTimer)
            {
                currentCustomersCount++;
                customerCurrentSpawnTimer = 0;

                SpawnCustomer();
            }
        }
    }

    private void SpawnCustomer()
    {
        //Checks if we have an empty table or not // And we have recipes avaialable to search
        if (DeliveryTableManager.Instance.HasEmptytable() && GameHandler.Instance.IsGamePlaying() && RecipeSelectionManager.Instance.HasRecipesAvaialable())
        {
            //We have an empty table
            //Spawn a random customer
            DeliveryTable deliveryTable = DeliveryTableManager.Instance.GetEmptyTable();
            Customer.SpawnCustomer(customerPrefabArray[Random.Range(0, customerPrefabArray.Length)], customerSpawnPoint, deliveryTable);
        }
        else
        {
            //We donot have an empty table
        }
    }

    public Transform GetCustomerSpawnPoint()
    {
        return customerSpawnPoint;
    }

    public void CustomerExitted()
    {
        currentCustomersCount--;
        customerCurrentSpawnTimer = 0;

        if (currentCustomersCount <= 0)
        {
            SpawnCustomer();
        }
    }
}
