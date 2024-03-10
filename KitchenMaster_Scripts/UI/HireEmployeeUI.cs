using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HireEmployeeUI : MonoBehaviour
{
    #region Singleton
    public static HireEmployeeUI Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion



    [SerializeField] private EmployeeListSO employeeListSO;

    [SerializeField] private Transform containerParent;
    [SerializeField] private Transform counterPurchaseUiTemplate;
    [SerializeField] private string messageText;

    private List<HireEmployeeTemplateUI> hireEmployeeTemplateUIList;
    private List<EmployeeSO> hiredEmployeeSOList;

    public event Action<List<EmployeeSO>> OnHiredEmployeeChange;

    private void Start()
    {
        counterPurchaseUiTemplate.gameObject.SetActive(false);
        hireEmployeeTemplateUIList = new List<HireEmployeeTemplateUI>();

        ManagerUI.Instance.OnPurchaseConfirm += Instance_OnPurchase;

        GenerateTemplates();
    }
    private void OnEnable()
    {
        if (MessageTextManager.Instance != null)
        {
            MessageTextManager.Instance.DisplayTutorialMessage(messageText);
        }
    }


    private void Instance_OnPurchase(float obj)
    {
        hiredEmployeeSOList = ManagerUI.Instance.GetHiredEmployeeSOList();

        SetTemplateValues();

        OnHiredEmployeeChange?.Invoke(hiredEmployeeSOList);
    }

    private void GenerateTemplates()
    {
        foreach (Transform child in containerParent)
        {
            if (child == counterPurchaseUiTemplate) continue;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < employeeListSO.employeeSOList.Count; i++)
        {
            Transform templateTransform = Instantiate(counterPurchaseUiTemplate, containerParent);

            templateTransform.GetComponent<HireEmployeeTemplateUI>().SetEmployeeSO(employeeListSO.employeeSOList[i]);
            hireEmployeeTemplateUIList.Add(templateTransform.GetComponent<HireEmployeeTemplateUI>());
            templateTransform.gameObject.SetActive(true);
        }

    }

    private void SetTemplateValues()
    {
        //Itterate through all templates
        foreach (HireEmployeeTemplateUI templateUI in hireEmployeeTemplateUIList)
        {
            //We will fire every employee by default
            templateUI.EmployeeFired();

            //Itterate through all hired employee list
            foreach (EmployeeSO employee in hiredEmployeeSOList)
            {
                //If this is the hired template of SO
                if (templateUI.GetEmployeeSO() == employee)
                {
                    //If any SO matches then set it to hired
                    templateUI.EmployeeHired();
                }
            }
        }
    }

    //public List<EmployeeSO> GetHiredEmployeeSoList()
    //{
    //    return hiredEmployeeSOList;
    //}
}
