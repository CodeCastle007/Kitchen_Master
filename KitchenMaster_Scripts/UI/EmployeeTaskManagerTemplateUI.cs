using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeTaskManagerTemplateUI : MonoBehaviour
{
    private EmployeeSO employeeSO;
    private Employee employee;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TMP_Dropdown taskSelectorDropdown;

    private EmployeeManager.EmployeeTask currentTask;
    private EmployeeManager.EmployeeTask[] taskArray;

    public static event Action OnAnyEmployeeUiTaskChange;

    private void Awake()
    {
        taskSelectorDropdown.onValueChanged.AddListener((int value) =>
        {
            currentTask = taskArray[value];

            OnAnyEmployeeUiTaskChange?.Invoke();
        });
    }

    private void Start()
    {
        taskArray = (EmployeeManager.EmployeeTask[]) Enum.GetValues(typeof(EmployeeManager.EmployeeTask));

        if (employee != null)
        {
            currentTask = employee.GetEmployeeCurrentTask();
        }
        else
        {
            currentTask = EmployeeManager.EmployeeTask.Unassigned;
        }

        ManagerUI.Instance.OnPurchaseConfirm += ManagerUI_OnPurchaseConfirm;
    }

    private void ManagerUI_OnPurchaseConfirm(float obj)
    {
        if (employee != null)
        {
            employee.SetEmployeeTask(currentTask);
        }
    }

    public void SetEmployeeSO(EmployeeSO employeeSO,Employee employee)
    {
        this.employeeSO = employeeSO;
        this.employee = employee;

        icon.sprite = employeeSO.icon;
        nameText.text = employeeSO.employeeName;

        SetTaskDropDown();
    }
    private void SetTaskDropDown()
    {
        List<string> tasks = new List<string>();

        foreach (EmployeeManager.EmployeeTask task in Enum.GetValues(typeof(EmployeeManager.EmployeeTask)))
        {
            tasks.Add(task.ToString());

            taskSelectorDropdown.ClearOptions();
            taskSelectorDropdown.AddOptions(tasks);

            taskSelectorDropdown.value = (int)employee.GetEmployeeCurrentTask();
        }
    }
    
    public EmployeeManager.EmployeeTask GetEmployeeTask()
    {
        return currentTask;
    }
}
