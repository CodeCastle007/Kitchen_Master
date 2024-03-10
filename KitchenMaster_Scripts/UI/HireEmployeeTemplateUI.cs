using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HireEmployeeTemplateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI costText;

    [SerializeField] private Button hireButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button fireButton;

    private EmployeeSO employeeSO;

    public static event Action<EmployeeSO> OnAnyEmployeeHire;
    public static event Action<EmployeeSO> OnAnyEmployeeHireCancel;

    public static event Action<EmployeeSO> OnAnyEmployeeFire;
    public static event Action<EmployeeSO> OnAnyEmployeeFireCancel;

    private bool isHired;

    private void Awake()
    {
        fireButton.gameObject.SetActive(false);

        hireButton.onClick.AddListener(() => 
        {
            hireButton.interactable = false;
            cancelButton.interactable = true;

            OnAnyEmployeeHire?.Invoke(employeeSO);
        });

        cancelButton.onClick.AddListener(() =>
        {
            if (isHired)
            {
                fireButton.interactable = true;
                OnAnyEmployeeFireCancel(employeeSO);
            }
            else
            {
                hireButton.interactable = true;
                OnAnyEmployeeHireCancel.Invoke(employeeSO);
            }
            
            cancelButton.interactable = false;

            
        });

        fireButton.onClick.AddListener(() =>
        {
            fireButton.interactable = false;
            cancelButton.interactable = true;

            OnAnyEmployeeFire?.Invoke(employeeSO);
        });
    }

    private void Start()
    {
        cancelButton.interactable = false;
    }


    public void SetEmployeeSO(EmployeeSO employeeSO)
    {
        this.employeeSO = employeeSO;

        icon.sprite = employeeSO.icon;
        nameText.text = employeeSO.employeeName.ToString();
        costText.text = employeeSO.hireCost.ToString() + "$";
    }

    public EmployeeSO GetEmployeeSO()
    {
        return employeeSO;
    }
   
    public void EmployeeHired()
    {
        //Employee Hired
        isHired = true;

        cancelButton.interactable = false;

        fireButton.gameObject.SetActive(true);
        fireButton.interactable = true;
        hireButton.gameObject.SetActive(false);
    }

    public void EmployeeFired()
    {
        isHired = false;

        cancelButton.interactable = false;

        fireButton.gameObject.SetActive(false);
        hireButton.gameObject.SetActive(true);
        hireButton.interactable = true;
    }
    
}
