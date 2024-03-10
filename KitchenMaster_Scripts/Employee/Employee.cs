using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Employee : MonoBehaviour,IKitchenObjectParent
{
    [SerializeField] private EmployeeSO employeeSO;
    [SerializeField] private Transform objectHoldPoint;

    [Space]
    [Header("Kitchen Object SO")]
    [SerializeField] private KitchenObjectSO emptyGlassSO;
    [SerializeField] private KitchenObjectSO filledGlassSO;

    [SerializeField] private KitchenObjectSO unCookedFries;
    [SerializeField] private KitchenObjectSO cookedFries;

    [SerializeField] private KitchenObjectSO unCookedPatty;
    [SerializeField] private KitchenObjectSO cookedPatty;

    private enum EmployeeState { Idle,Walking,Working, Fired}
    private EmployeeState state;

    private enum DishWashingState { GettingTable,PickUpUncleanPlate,GettingTrash,TrashRecipe,GettingSink,GettingClearCounter,DropUncleanPlate,WashDishes,GettingPlateCounter,PlaceCleanPlate}
    private DishWashingState dishWashState;

    private enum MakeDrinkState { Waiting,CheckRecipe,GetGlassCounter,GetGlass,GetDrinksCounter,DropGlass,WaitForFill,GettingClearCounter}
    private MakeDrinkState makeDrinkState;

    private enum MakeFriesState { Waiting,CheckRecipe,GetFriesCounter,GetFries,GetFryingCounter,DropFries,WaitForFry,GettingClearCounter}
    private MakeFriesState makeFriesState;

    private enum MakeMeatPattyState { Waiting,CheckRecipe,GetMeatPattyCounter,GetPatty,GetStoveCounter,DropPatty,WaitForCook,GettingClearCounter}
    private MakeMeatPattyState makePattyState;

    private EmployeeManager.EmployeeTask currentEmployeeTask;

    private float actionTimerMax = .5f;
    private float currentActionTimer;

    private List<RecipeSO> waitingRecipeSOList;

    private NavMeshAgent navMeshAgent;
    private Vector3 waitingPosition;
    private Vector3 destination;
    private float stoppingDistance;
    private bool isWalking;
    private bool isFired;

    private KitchenObject kitchenObject;
    private BaseCounter counter = null;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        waitingPosition = EmployeeManager.Instance.GetWaitingPointPos();
        waitingRecipeSOList = new List<RecipeSO>();

        //Calculate a random position
        destination = GetARandomPositionAroundWaitingPoint();
        stoppingDistance = 0.5f;

        ChangeState(EmployeeState.Walking);

        ChangeTask(EmployeeManager.EmployeeTask.Unassigned);
        currentActionTimer = actionTimerMax;

        //For player working
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;

        //For occasions if emplpyee is hired after recipe spawned to notify employee about recipe to be delivered
        waitingRecipeSOList = DeliveryManager.Instance.GetWaitingRecipeSOList();
    }

    private void DeliveryManager_OnRecipeSpawned(RecipeSO recipeSO)
    {
        waitingRecipeSOList.Add(recipeSO);
    }

    private void Update()
    {
        switch (state)
        {
            case EmployeeState.Idle:
                EmployeeIdle();
                break;

            case EmployeeState.Walking:
                EmployeeWalking();
                break;

            case EmployeeState.Working:
                Working();
                break;

            case EmployeeState.Fired:
                EmployeeFired();
                break;
        }
    }

    private void EmployeeIdle()
    {
        isWalking = false;

        //Check if user has assigned task or not
        if (currentEmployeeTask != EmployeeManager.EmployeeTask.Unassigned)
        {
            //We have assigned some task
            ChangeState(EmployeeState.Working);
        }
    }
    private void EmployeeWalking()
    {
        isWalking = true;

        navMeshAgent.isStopped = false;

        navMeshAgent.SetDestination(destination);

        if (Vector3.Distance(transform.position, destination) <= stoppingDistance)
        {
            //We have reached the required point
            navMeshAgent.isStopped = true;

            if (isFired)
            {
                Destroy(this.gameObject);
            }
            else
            {
                ChangeState(EmployeeState.Idle);
            }
        }
    }
    private void EmployeeFired()
    {
        //Return to spawn point and destroyed
        if (!isFired)
        {
            isWalking = true;
            isFired = true;
            Transform spawnPoint = EmployeeManager.Instance.GetEmployeeSpawnPoint();
            destination = spawnPoint.position;
            state = EmployeeState.Walking;
        }
       

    }
    private void Working()
    {
        //Check if we have unassigned the task or not
        if (currentEmployeeTask == EmployeeManager.EmployeeTask.Unassigned)
        {
            //We have unassigned any task
            destination = GetARandomPositionAroundWaitingPoint();
            ChangeState(EmployeeState.Walking);
        }

        switch (currentEmployeeTask)
        {
            case EmployeeManager.EmployeeTask.Dish_Washer:
                switch (dishWashState)
                {

                    case DishWashingState.GettingTable:

                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //We will find the correct delivery table
                            counter = GetCounterWithUncleanPlate();

                            //If we have the delivery table we will move to the destination
                            if (counter != null)
                            {
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);
                                dishWashState = DishWashingState.PickUpUncleanPlate;
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case DishWashingState.PickUpUncleanPlate:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            InteractWithCounter();
                            counter = null;
                            currentActionTimer = actionTimerMax;

                            if (HasKitchenObject())
                            {
                                //We have got the kitchen object
                                if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                                {
                                    if (plateKitchenObject.IsClean())
                                    {
                                        //We have plate with wrong recipe
                                        dishWashState = DishWashingState.GettingTrash;
                                    }
                                    else
                                    {
                                        //we have unclean plate
                                        dishWashState = DishWashingState.GettingSink;
                                    }
                                }
                            }
                            else
                            {
                                destination = GetARandomPositionAroundWaitingPoint();
                                ChangeState(EmployeeState.Walking);
                                dishWashState = DishWashingState.GettingTable;
                            }  
                        }
                        break;

                    case DishWashingState.GettingSink:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Getting nearest sink counter
                            counter = GetNearestSinkCounter();

                            if (counter != null)
                            {
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);

                                //Check if sink already has a plate (someone place it there)
                                if (counter.GetComponent<SinkCounter>().HasPlateToClean())
                                {
                                    if (HasKitchenObject())
                                    {
                                        dishWashState = DishWashingState.GettingClearCounter;
                                    }
                                    else
                                    {
                                        dishWashState = DishWashingState.WashDishes;
                                    }
                                }
                                else
                                {
                                    dishWashState = DishWashingState.DropUncleanPlate;
                                }
                            }
                           
                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case DishWashingState.GettingClearCounter:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Getting nearest sink counter
                            counter = GetNearestClearCounter();

                            if (counter != null)
                            {
                                //Get an offset position for employee to stand
                                //Vector3 offset = new Vector3(-1.5f, 0, 0);
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);
                                dishWashState = DishWashingState.DropUncleanPlate;
                            }

                            currentActionTimer = actionTimerMax;
                        }

                        break;

                    case DishWashingState.GettingTrash:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Getting nearest sink counter
                            counter = GetNearestTrashCounter();

                            if (counter != null)
                            {
                                //Get an offset position for employee to stand
                                //Vector3 offset = new Vector3(-1.5f, 0, 0);
                                destination = counter.GetEmployeeStandingPoint().position;

                                ChangeState(EmployeeState.Walking);

                                dishWashState = DishWashingState.TrashRecipe;
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case DishWashingState.TrashRecipe:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Look at counter
                            transform.forward = counter.transform.position - transform.position;

                            InteractWithCounter();

                            currentActionTimer = actionTimerMax;

                            dishWashState = DishWashingState.GettingSink;
                        }
                        break;

                    case DishWashingState.DropUncleanPlate:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Look at counter
                            transform.forward = counter.transform.position - transform.position;
                            InteractWithCounter();
                            currentActionTimer = actionTimerMax;

                            //Check if we have dropped the plate on sink counter
                            if(counter.GetComponent<SinkCounter>())
                            {
                                dishWashState = DishWashingState.WashDishes;
                            }
                            else
                            {
                                //We have dropped it on clear counter
                                dishWashState = DishWashingState.GettingSink;
                            }
                            
                        }
                        break;

                    case DishWashingState.WashDishes:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //We have droped the unclean plate
                            //Check if we have any kitchen object or not
                            if (!HasKitchenObject())
                            {
                                //We donot have it means washing is not complete yet
                                AlternateInteractWithCounter();
                            }
                            else
                            {
                                //We have the clean plate from sink
                                dishWashState = DishWashingState.GettingPlateCounter;

                                counter = null;
                            }
                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case DishWashingState.GettingPlateCounter:

                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            counter = GetNearestPlateCounter();

                            destination = counter.GetEmployeeStandingPoint().position;
                            ChangeState(EmployeeState.Walking);

                            dishWashState = DishWashingState.PlaceCleanPlate;

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case DishWashingState.PlaceCleanPlate:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Check if we have plates counter to interact
                            if(counter != null)
                            {
                                InteractWithCounter();
                                counter = null;
                            }
                            else
                            {
                                //We have placed the plate

                                destination = GetARandomPositionAroundWaitingPoint();
                                ChangeState(EmployeeState.Walking);
                                dishWashState = DishWashingState.GettingTable;
                            }
                           
                            currentActionTimer = actionTimerMax;
                        }
                        break;
                }
                break;

            case EmployeeManager.EmployeeTask.Make_Drinks:
                switch (makeDrinkState)
                {
                    case MakeDrinkState.Waiting:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Check we have recipes to deliver and we also have glasses to make drinks
                            if (waitingRecipeSOList.Count > 0 && ResourceManager.Instace.HasKitchenObjectResourceCount(emptyGlassSO))
                            {
                                makeDrinkState = MakeDrinkState.CheckRecipe;
                            }
                            currentActionTimer = actionTimerMax;
                        }
                       
                        break;

                    case MakeDrinkState.CheckRecipe:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            RecipeSO recipeWithDrink = null;
                            //Check all available recipes
                            foreach (RecipeSO recipeSO in waitingRecipeSOList)
                            {
                                if (recipeSO.kitchenObjectSOList.Contains(filledGlassSO))
                                {
                                    //The recipe has a drink
                                    makeDrinkState = MakeDrinkState.GetGlassCounter;
                                    recipeWithDrink = recipeSO;
                                    break;
                                }
                            }
                            //Remove the recipe because we will deliver it
                            if (recipeWithDrink != null)
                            {
                                waitingRecipeSOList.Remove(recipeWithDrink);
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeDrinkState.GetGlassCounter:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            counter = GetNearestGlassCounter();
                            if (counter != null)
                            {
                                //Get an offset position for employee to stand
                                //Vector3 offset = new Vector3(-1.5f, 0, 0);
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);
                                makeDrinkState = MakeDrinkState.GetGlass;
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeDrinkState.GetGlass:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            if (counter != null)
                            {
                                InteractWithCounter();
                                counter = null;
                                if (HasKitchenObject())
                                {
                                    if (GetKitchenObject().GetKitchenObjectSO() == emptyGlassSO)
                                    {
                                        //We have an empty glass to fill
                                        makeDrinkState = MakeDrinkState.GetDrinksCounter;
                                    }
                                    else
                                    {
                                        //We have picked a filled glass
                                        makeDrinkState = MakeDrinkState.GettingClearCounter;
                                    }
                                }
                                else
                                {
                                    destination = GetARandomPositionAroundWaitingPoint();
                                    ChangeState(EmployeeState.Walking);
                                    makeDrinkState = MakeDrinkState.Waiting;
                                }
                                
                            }
                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeDrinkState.GetDrinksCounter:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            counter = GetNearestDrinksCounter();
                            if (counter != null)
                            {
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);
                                makeDrinkState = MakeDrinkState.DropGlass;
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeDrinkState.DropGlass:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            if (counter != null)
                            { 
                                InteractWithCounter();
                                if (counter.GetComponent<DrinksCounter>())
                                {
                                    //Counter is a drinks counter
                                    makeDrinkState = MakeDrinkState.WaitForFill;
                                }
                                else
                                {
                                    //Counter is other one (clear counter)
                                    counter = null;

                                    destination = GetARandomPositionAroundWaitingPoint();
                                    ChangeState(EmployeeState.Walking);
                                    makeDrinkState=MakeDrinkState.Waiting;
                                }
                            }
                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeDrinkState.WaitForFill:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            if (counter != null)
                            {
                                //check if glass is filled or not
                                if (counter.GetComponent<DrinksCounter>().GlassFilled())
                                {
                                    makeDrinkState = MakeDrinkState.GetGlass;
                                }
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeDrinkState.GettingClearCounter:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Getting nearest sink counter
                            counter = GetNearestClearCounter();

                            if (counter != null)
                            {
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);
                                makeDrinkState = MakeDrinkState.DropGlass;
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;
                }
                break;

            case EmployeeManager.EmployeeTask.Make_Fries:

                switch (makeFriesState)
                {
                    case MakeFriesState.Waiting:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Check we have recipes to deliver and we also have glasses to make drinks
                            if (waitingRecipeSOList.Count > 0 && ResourceManager.Instace.HasKitchenObjectResourceCount(unCookedFries))
                            {
                                makeFriesState = MakeFriesState.CheckRecipe;
                            }
                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeFriesState.CheckRecipe:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            RecipeSO recipeWithFries = null;
                            //Check all available recipes
                            foreach (RecipeSO recipeSO in waitingRecipeSOList)
                            {
                                if (recipeSO.kitchenObjectSOList.Contains(cookedFries))
                                {
                                    //The recipe has a drink
                                    makeFriesState = MakeFriesState.GetFriesCounter;
                                    recipeWithFries = recipeSO;
                                    break;
                                }
                            }
                            //Remove the recipe because we will deliver it
                            if (recipeWithFries != null)
                            {
                                waitingRecipeSOList.Remove(recipeWithFries);
                            }


                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeFriesState.GetFriesCounter:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            counter = GetNearestFriesCounter();
                            if (counter != null)
                            {
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);
                                makeFriesState = MakeFriesState.GetFries;
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeFriesState.GetFries:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            if (counter != null)
                            {
                                InteractWithCounter();
                                counter = null;

                                if (HasKitchenObject())
                                {
                                    if (GetKitchenObject().GetKitchenObjectSO() == unCookedFries)
                                    {
                                        //We have an empty glass to fill
                                        makeFriesState = MakeFriesState.GetFryingCounter;
                                    }
                                    else
                                    {
                                        //We have picked a filled glass
                                        makeFriesState = MakeFriesState.GettingClearCounter;
                                    }
                                }
                                else
                                {
                                    destination = GetARandomPositionAroundWaitingPoint();
                                    ChangeState(EmployeeState.Walking);
                                    makeFriesState = MakeFriesState.Waiting;
                                }
                               
                            }
                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeFriesState.GetFryingCounter:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            counter = GetNearestFryingCounter();
                            if (counter != null)
                            {
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);
                                makeFriesState = MakeFriesState.DropFries;
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeFriesState.DropFries:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            if (counter != null)
                            {
                                InteractWithCounter();
                                if (counter.GetComponent<FryerCounter>())
                                {
                                    //Counter is a frying counter
                                    makeFriesState = MakeFriesState.WaitForFry;
                                }
                                else
                                {
                                    //Counter is other one (clear counter)
                                    counter = null;

                                    destination = GetARandomPositionAroundWaitingPoint();
                                    ChangeState(EmployeeState.Walking);
                                    makeFriesState = MakeFriesState.Waiting;
                                }
                            }
                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeFriesState.WaitForFry:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            if (counter != null)
                            {
                                //check if glass is filled or not
                                if (counter.GetComponent<FryerCounter>().FriesFried())
                                {
                                    makeFriesState = MakeFriesState.GetFries;
                                }
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeFriesState.GettingClearCounter:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Getting nearest sink counter
                            counter = GetNearestClearCounter();

                            if (counter != null)
                            {
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);
                                makeFriesState=MakeFriesState.DropFries;
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                }

                break;

            case EmployeeManager.EmployeeTask.Make_Patties:
                switch (makePattyState)
                {
                    case MakeMeatPattyState.Waiting:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Check we have recipes to deliver and we also have glasses to make drinks
                            if (waitingRecipeSOList.Count > 0 && ResourceManager.Instace.HasKitchenObjectResourceCount(unCookedPatty))
                            {
                                makePattyState = MakeMeatPattyState.CheckRecipe;
                            }
                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeMeatPattyState.CheckRecipe:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            RecipeSO recipeWithPatty = null;
                            //Check all available recipes
                            foreach (RecipeSO recipeSO in waitingRecipeSOList)
                            {
                                if (recipeSO.kitchenObjectSOList.Contains(cookedPatty))
                                {
                                    //The recipe has a drink
                                    makePattyState = MakeMeatPattyState.GetMeatPattyCounter;
                                    recipeWithPatty = recipeSO;
                                    break;
                                }
                            }
                            //Remove the recipe because we will deliver it
                            if (recipeWithPatty != null)
                            {
                                waitingRecipeSOList.Remove(recipeWithPatty);
                            }


                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeMeatPattyState.GetMeatPattyCounter:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            counter = GetNearestMeatPattyCounter();
                            if (counter != null)
                            {
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);
                                makePattyState = MakeMeatPattyState.GetPatty;
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeMeatPattyState.GetPatty:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            if (counter != null)
                            {
                                InteractWithCounter();
                                counter = null;

                                if (HasKitchenObject())
                                {
                                    if (GetKitchenObject().GetKitchenObjectSO() == unCookedPatty)
                                    {
                                        //We have an uncooked patty to cook
                                        makePattyState = MakeMeatPattyState.GetStoveCounter;
                                    }
                                    else
                                    {
                                        //We have picked a cooked patty
                                        makePattyState = MakeMeatPattyState.GettingClearCounter;
                                    }
                                }
                                else
                                {
                                    destination = GetARandomPositionAroundWaitingPoint();
                                    ChangeState(EmployeeState.Walking);
                                    makePattyState = MakeMeatPattyState.Waiting;
                                }
                                
                            }
                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeMeatPattyState.GetStoveCounter:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            counter = GetNearestStoveCounter();
                            if (counter != null)
                            {
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);
                                makePattyState = MakeMeatPattyState.DropPatty;
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeMeatPattyState.DropPatty:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            if (counter != null)
                            {
                                InteractWithCounter();
                                if (counter.GetComponent<StoveCounter>())
                                {
                                    //Counter is a frying counter
                                    makePattyState = MakeMeatPattyState.WaitForCook;
                                }
                                else
                                {
                                    //Counter is other one (clear counter)
                                    counter = null;

                                    destination = GetARandomPositionAroundWaitingPoint();
                                    ChangeState(EmployeeState.Walking);
                                    makePattyState = MakeMeatPattyState.Waiting;
                                }
                            }
                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeMeatPattyState.WaitForCook:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            if (counter != null)
                            {
                                //check if glass is filled or not
                                if (counter.GetComponent<StoveCounter>().PattyCooked())
                                {
                                    makePattyState = MakeMeatPattyState.GetPatty;
                                }
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                    case MakeMeatPattyState.GettingClearCounter:
                        currentActionTimer -= Time.deltaTime;
                        if (currentActionTimer <= 0)
                        {
                            //Getting nearest sink counter
                            counter = GetNearestClearCounter();

                            if (counter != null)
                            {
                                destination = counter.GetEmployeeStandingPoint().position;
                                ChangeState(EmployeeState.Walking);
                                makePattyState = MakeMeatPattyState.DropPatty;
                            }

                            currentActionTimer = actionTimerMax;
                        }
                        break;

                }
                break;
        }
    }

    #region Working Functions

    public BaseCounter GetCounterWithUncleanPlate()
    {
        return CounterTransformHandler.Instance.GetCounterWithUnCleanPlate();
    }

    private BaseCounter GetNearestSinkCounter()
    {
        Transform sinkTransform = CounterTransformHandler.Instance.GetNearestSinkCounterTransform(transform.position);
        return sinkTransform.GetComponent<BaseCounter>();

    }
    private BaseCounter GetNearestPlateCounter()
    {
        Transform platesCounter = CounterTransformHandler.Instance.GetNearestPlatesCounterTransform(transform.position);
        return platesCounter.GetComponent<BaseCounter>();
    }
    private BaseCounter GetNearestTrashCounter()
    {
        Transform trashCounter = CounterTransformHandler.Instance.GetNearestTrashCounterTransform(transform.position);
        return trashCounter.GetComponent<BaseCounter>();

    }
    private BaseCounter GetNearestClearCounter()
    {
        Transform trashCounter = CounterTransformHandler.Instance.GetNearestClearCounter(transform.position);
        return trashCounter.GetComponent<BaseCounter>();

    }
    private BaseCounter GetNearestGlassCounter()
    {
        Transform trashCounter = CounterTransformHandler.Instance.GetNearestGlassCounterTransform(transform.position);
        return trashCounter.GetComponent<BaseCounter>();
    }
    private BaseCounter GetNearestDrinksCounter()
    {
        Transform trashCounter = CounterTransformHandler.Instance.GetNearestDrinksCounterTransform(transform.position);
        return trashCounter.GetComponent<BaseCounter>();
    }
    private BaseCounter GetNearestFriesCounter()
    {
        Transform trashCounter = CounterTransformHandler.Instance.GetNearestFriesCounterTransform(transform.position);
        return trashCounter.GetComponent<BaseCounter>();
    }
    private BaseCounter GetNearestFryingCounter()
    {
        Transform trashCounter = CounterTransformHandler.Instance.GetNearestFryingCounterTransform(transform.position);
        return trashCounter.GetComponent<BaseCounter>();
    }
    private BaseCounter GetNearestMeatPattyCounter()
    {
        Transform trashCounter = CounterTransformHandler.Instance.GetNearestMeatPattyCounterTransform(transform.position);
        return trashCounter.GetComponent<BaseCounter>();
    }
    private BaseCounter GetNearestStoveCounter()
    {
        Transform trashCounter = CounterTransformHandler.Instance.GetNearestStoveCounterTransform(transform.position);
        return trashCounter.GetComponent<BaseCounter>();
    }

    private void InteractWithCounter()
    {
        counter.Interact(this);
    }
    private void AlternateInteractWithCounter()
    {
        counter.InteractAlternate(this);
    }

    #endregion

    private void ChangeState(EmployeeState state)
    {
        if (this.state != state)
        {
            this.state = state;
        }
    }

    private void ChangeTask(EmployeeManager.EmployeeTask task)
    {
        if (currentEmployeeTask != task)
        {
            currentEmployeeTask = task;

            waitingRecipeSOList = DeliveryManager.Instance.GetWaitingRecipeSOList();
        }
    }
    public EmployeeManager.EmployeeTask GetEmployeeCurrentTask()
    {
        return currentEmployeeTask;
    }
    public void SetEmployeeTask(EmployeeManager.EmployeeTask task)
    {
        ChangeTask(task);
    }


    public void FireEmployee()
    {
        ChangeState(EmployeeState.Fired);
    }
    public bool IsWalking()
    {
        return isWalking;
    }
    private Vector3 GetARandomPositionAroundWaitingPoint()
    {
        return new Vector3(Random.Range(waitingPosition.x - 1, waitingPosition.x + 1), 0, Random.Range(waitingPosition.z - 1, waitingPosition.z + 1));
    }
    public EmployeeSO GetEmployeeSO()
    {
        return employeeSO;
    }


    #region Interface Functions
    //INTERFACE FUNCTIONS
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }
    public bool HasKitchenObject()
    {
        return kitchenObject ? true : false;
    }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (this.kitchenObject != null)
        {
           // OnPickSomething?.Invoke(transform);
        }
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return objectHoldPoint;
    }
    #endregion
}
