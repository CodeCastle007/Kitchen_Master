using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmployeeTaskManagerUI : MonoBehaviour
{
    public static EmployeeTaskManagerUI Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private Transform containerParent;
    [SerializeField] private Transform employeeTaskManagerUITemplate;
    [SerializeField] private string messageText;

    private List<EmployeeSO> hiredEmployeeSOList;
    private List<Employee> hiredEmployeeList;

    //Hold list for templates and their tasks to prevent duplicate tasks
    private List<EmployeeTaskManagerTemplateUI> templateUiList;
    private List<EmployeeManager.EmployeeTask> templateUITasksList;

    public event Action OnTaskSimilarityChange;

    private bool hasSimilarTasks = false;


    private void Start()
    {
        hiredEmployeeSOList = new List<EmployeeSO>();
        hiredEmployeeList = new List<Employee>();

        templateUiList = new List<EmployeeTaskManagerTemplateUI>();
        templateUITasksList = new List<EmployeeManager.EmployeeTask>();

        EmployeeManager.Instance.OnHiredEmployeeChanged += GetHiredEmployeeList;

        EmployeeTaskManagerTemplateUI.OnAnyEmployeeUiTaskChange += EmployeeTaskManagerTemplateUI_OnAnyEmployeeUiTaskChange;

        employeeTaskManagerUITemplate.gameObject.SetActive(false);
        GetHiredEmployeeList();
    }
    private void OnEnable()
    {
        if (MessageTextManager.Instance != null)
        {
            MessageTextManager.Instance.DisplayTutorialMessage(messageText);
        }
    }


    private void EmployeeTaskManagerTemplateUI_OnAnyEmployeeUiTaskChange()
    {
        for (int i = 0; i < templateUiList.Count; i++)
        {
            templateUITasksList[i] = templateUiList[i].GetEmployeeTask();
        }

        CheckTaskSimilarity();
        ManagerUI.Instance.ChangeConfirmButtonColor();
    }

    private void GetHiredEmployeeList()
    {
        hiredEmployeeSOList = EmployeeManager.Instance.GetHiredEmployeeSOList();
        hiredEmployeeList = EmployeeManager.Instance.GetHiredEmployeeList();

        if (hiredEmployeeSOList.Count > 0)
        {
            Invoke("GenerateTemplates",.2f);
        }
        else
        {
            Invoke("ClearTemplates",.1f);
        }
    }

    private void GenerateTemplates()
    {
        ClearTemplates();

        for (int i = 0; i < hiredEmployeeSOList.Count; i++)
        {
            Transform templateTransform = Instantiate(employeeTaskManagerUITemplate, containerParent);

            templateTransform.GetComponent<EmployeeTaskManagerTemplateUI>().SetEmployeeSO(hiredEmployeeSOList[i], hiredEmployeeList[i]);

            //Set values of template list and their respective deffault tasks (unassigned)
            templateUiList.Add(templateTransform.GetComponent<EmployeeTaskManagerTemplateUI>());
            templateUITasksList.Add(EmployeeManager.EmployeeTask.Unassigned);

            templateTransform.gameObject.SetActive(true);
        }
    }

    private void ClearTemplates()
    {
        foreach (Transform child in containerParent)
        {
            if (child == employeeTaskManagerUITemplate) continue;
            Destroy(child.gameObject);
        }

        templateUiList.Clear();
        templateUITasksList.Clear();
    }

    private void CheckTaskSimilarity()
    {
        hasSimilarTasks = false;
        for (int i = 0; i < templateUiList.Count; i++)
        {
            for (int j = i+1; j < templateUiList.Count; j++)
            {
                if(templateUiList[i].GetEmployeeTask() != EmployeeManager.EmployeeTask.Unassigned && templateUiList[j].GetEmployeeTask() != EmployeeManager.EmployeeTask.Unassigned)
                {
                    if (templateUiList[i].GetEmployeeTask() == templateUiList[j].GetEmployeeTask())
                    {
                        hasSimilarTasks = true;
                    }
                }
            }
        }
        OnTaskSimilarityChange?.Invoke(); 
    }

    public bool HasTaskSimilarity()
    {
        return hasSimilarTasks;
    }
}
