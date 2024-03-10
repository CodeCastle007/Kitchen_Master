using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour,IHasProgress
{
    public enum CustomerState
    {
        Idle,
        Walking,
        Sitting,
        Order,
        Waiting,
        Eating,
        Standing,
        Returning
    }
    [SerializeField] private CustomerState state;

    private DeliveryTable deliveryTable;
    private Transform sittingTransform;
    private NavMeshAgent naveMeshAgent;
    private Transform spawnPoint;

    private float eatingTimer;
    private float eatingTimerMax = 10f;

    private float waitingTimer;
    private float waitingtimerMax = 90f;

    private RecipeSO orderedRecipe;

    public event Action<CustomerState> OnStateChange;
    public event Action<float> OnProgressChanged;
    public static event Action<float> OnAnyCustomerServed;
    public static event Action<DeliveryTable> OnCustomerWaitingTimerEnd; 


    public static event Action<float> OnAnyCustomerLeave;
    private bool deliveredCorrectRecipe;

    private void Awake()
    {
        naveMeshAgent = GetComponent<NavMeshAgent>();

        waitingTimer = waitingtimerMax;
        state = CustomerState.Idle;
    }

    private void Update()
    {
        switch (state)
        {
            case CustomerState.Idle:
                CustomerIdle();
                break;

            case CustomerState.Walking:
                CustomerWalking();
                break;

            case CustomerState.Sitting:
                CustomerSitting();
                break;

            case CustomerState.Order:
                CustomerOrder();
                break;

            case CustomerState.Waiting:
                CustomerWaiting();
                break;

            case CustomerState.Eating:
                CustomerEating();
                break;

            case CustomerState.Standing:
                CustomerStanding();
                break;

            case CustomerState.Returning:
                CustomerReturning();
                break;
        }
    }

    private void CustomerIdle()
    {
        if (deliveryTable)
        {
            state = CustomerState.Walking;

            OnStateChange?.Invoke(state);
        }
    }
    private void CustomerWalking()
    {
        //Walk towards table
        naveMeshAgent.SetDestination(sittingTransform.position);

        float stoppingDistnce = 1f;
        if (Vector3.Distance(transform.position, sittingTransform.position) <= stoppingDistnce)
        {
            naveMeshAgent.isStopped = true;
            state = CustomerState.Sitting;

            //Place the customer
            transform.position = sittingTransform.position;
            transform.forward = sittingTransform.forward;

            OnStateChange?.Invoke(state);
        }
    }
    private void CustomerSitting()
    {
        //First we will check can we place order or not
        if (DeliveryManager.Instance.CanPlaceOrder())
        {
            state = CustomerState.Order;
            OnStateChange?.Invoke(state);
        }
    }
    private void CustomerOrder()
    {
        orderedRecipe = DeliveryManager.Instance.PlaceOrder(deliveryTable);

        state = CustomerState.Waiting;
        OnStateChange?.Invoke(state);
    }
    private void CustomerWaiting()
    {
        waitingTimer -= Time.deltaTime;

        OnProgressChanged?.Invoke(waitingTimer / waitingtimerMax);

        if (waitingTimer <= 0)
        {
            OnCustomerWaitingTimerEnd(deliveryTable);

            //Leave
            Leave();

            //Player failed to deliver recipe on time
            state = CustomerState.Standing;

            OnStateChange?.Invoke(state);
        }
    }
    private void CustomerEating()
    {
        eatingTimer += Time.deltaTime;
        if (eatingTimer >= eatingTimerMax)
        {
            //Tell the table that customer has eaten his food
            deliveryTable.FoodEaten();

            Leave();

            //Customer has eaten the food
            state = CustomerState.Standing;
            OnStateChange?.Invoke(state);
        }
    }
    private void CustomerStanding()
    {
        //Give cash
        if(deliveredCorrectRecipe){
            OnAnyCustomerLeave?.Invoke(orderedRecipe.cost);
        }

        Vector3 offset = new Vector3(1f, 0, 0);
        transform.localPosition += offset;

        state = CustomerState.Returning;
        OnStateChange?.Invoke(state);
    }
    private void CustomerReturning()
    {
        //Walk to spawn point
        naveMeshAgent.isStopped = false;
        naveMeshAgent.SetDestination(spawnPoint.position);

        if (Vector3.Distance(transform.position, spawnPoint.position) <= 0.2f)
        {
            CustomerManager.Instance.CustomerExitted();
            Destroy(gameObject);
        }
    }


    public void DeliveredCorrectRecipe()
    {
        deliveredCorrectRecipe=true;

        //Player delivered correct recipe
        state = CustomerState.Eating;

        //Firing the event for switching off the progress bar
        OnProgressChanged?.Invoke(0);
        OnStateChange?.Invoke(state);
        OnAnyCustomerServed?.Invoke((waitingtimerMax - waitingTimer)/100f);
    }
    public void DeliveredIncorrectRecipe()
    {
        deliveredCorrectRecipe=false;
        //Player delivered incorrect recipe
        state = CustomerState.Standing;

        OnStateChange?.Invoke(state);

        Leave();
    }



    public void SetDeliveryTable(DeliveryTable deliveryTable)
    {
        this.deliveryTable = deliveryTable;
    }

    public void Leave()
    {
        deliveryTable.ToggleOccupied();

        deliveryTable.ClearCustomer();
        deliveryTable = null;
        sittingTransform = null;

        //Firing the event for switching off the progress bar
        OnProgressChanged?.Invoke(0);
    }
    public void SetSittingTransform(Transform sittingPos)
    {
        sittingTransform = sittingPos;
    }
    public void SetSpawnPoint(Transform spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }


    public static void SpawnCustomer(Transform customerPrefab, Transform spawnPoint, DeliveryTable deliveryTable)
    {
        Transform customerTransform = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);

        Customer customer = customerTransform.GetComponent<Customer>();

        //Let table know which customer is on and also customer know which table he is on
        deliveryTable.SetCustomer(customer);
        customer.SetDeliveryTable(deliveryTable);
        customer.SetSittingTransform(deliveryTable.GetSittingPosition());
        customer.SetSpawnPoint(spawnPoint);
    }
}
