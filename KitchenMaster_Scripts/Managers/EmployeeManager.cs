using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EmployeeManager : MonoBehaviour
{
    #region Singleton
    public static EmployeeManager Instance { get; private set; }
    private void Awake()
    {
        hiredEmployeeSOList = new List<EmployeeSO>();
        Instance = this;
    }
    #endregion

    private List<EmployeeSO> hiredEmployeeSOList;
    private List<Employee> hiredEmployeeList;

    [SerializeField] private Transform employeeSpawnPoint;
    [SerializeField] private Transform waitingPoint;

    private float activateDelay = 2f;
    public event Action OnHiredEmployeeChanged;

    public enum EmployeeTask { Unassigned,Dish_Washer,Make_Drinks,Make_Fries,Make_Patties}
    private EmployeeTask employeeTasks;
    private void Start()
    {
        hiredEmployeeList = new List<Employee>();
        HireEmployeeUI.Instance.OnHiredEmployeeChange += Instance_OnHiredEmployeeChange;
    }

    private void Instance_OnHiredEmployeeChange(List<EmployeeSO> obj)
    {
        activateDelay = 0;

        //We will first check if we have any employee that we fired 
        CheckFiredEmployee(obj);
        InstantitaeEmployeeTransformFromList(obj);
        ActivateEmployeeTransformFromList(hiredEmployeeList);
        OnHiredEmployeeChanged?.Invoke();
    }

    //Responsible of instantiating the prefab of employee in scene
    private void InstantitaeEmployeeTransformFromList(List<EmployeeSO> employeeSOList)
    {
        for (int i = 0; i < employeeSOList.Count; i++)
        {
            //Check if we already instantitaed the prefab of this SO or not
            if (!hiredEmployeeSOList.Contains(employeeSOList[i]))
            {
                SpawnEmployee(employeeSOList[i].employeePrefab, employeeSpawnPoint.position);
            }
        }
        //Create a copy of list instead of passing it by refrence
        hiredEmployeeSOList = new List<EmployeeSO>(employeeSOList);
    }
    private void SpawnEmployee(Transform prefab, Vector3 position)
    {
        Transform employeeTransfrom = Instantiate(prefab, position, Quaternion.identity);
        employeeTransfrom.gameObject.SetActive(false);
        hiredEmployeeList.Add(employeeTransfrom.GetComponent<Employee>());
    }

    private void ActivateEmployeeTransformFromList(List<Employee> employeeList)
    {
        for (int i = 0; i < employeeList.Count; i++)
        {
            activateDelay = i;

            StartCoroutine(ActivateEmployeeTransform(employeeList[i].transform));
        }
    }
    private IEnumerator ActivateEmployeeTransform(Transform employee)
    {

        yield return new WaitForSeconds(activateDelay);

        employee.gameObject.SetActive(true);
    }

    private void CheckFiredEmployee(List<EmployeeSO> employeeSOList)
    {
        //Get track of all fired employees
        List<Employee> firedEmployees = new List<Employee>();

        //Itterate through the previously hired employee list
        foreach (EmployeeSO employeeSO in hiredEmployeeSOList)
        {
            //If the new list contain this SO 
            if (employeeSOList.Contains(employeeSO))
            {
                //This employee is still hired
            }
            else
            {
                //THis employee is fired
                Debug.Log(employeeSO.employeeName + " is fired");

                //We will cycle through all hired employees to find the fired one
                foreach (Employee employee in hiredEmployeeList)
                {
                    if (employeeSO == employee.GetEmployeeSO())
                    {
                        //Call function from employee class to notify it is fired
                        employee.FireEmployee();

                        //hiredEmployeeList.Remove(employee);
                        firedEmployees.Add(employee);
                    }
                }

                //hiredEmployeeSOList.Remove(employeeSO);
            }
        }

        foreach (Employee employee in firedEmployees)
        {
            //Just checking for safety
            if (hiredEmployeeList.Contains(employee))
            {
                hiredEmployeeList.Remove(employee);
            }
        }
    }

    public Vector3 GetWaitingPointPos()
    {
        return waitingPoint.position;
    }

    public List<EmployeeSO> GetHiredEmployeeSOList()
    {
        return hiredEmployeeSOList;
    }
    public List<Employee> GetHiredEmployeeList()
    {
        return hiredEmployeeList;
    }

    public Transform GetEmployeeSpawnPoint()
    {
        return employeeSpawnPoint;
    }
}
