using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    #region Singleton
    public static ManagerUI Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        confirmButton.onClick.AddListener(() =>
        {
            if (totalCost >= 0 && totalCost <= CashManager.Instance.GetTotalCash())
            {
                //We have the desired amount
                OnPurchaseConfirm?.Invoke(totalCost);

                SetTotalCost(0);

                RevertConfirmButtonColor();
            }
            else
            {
                //We donot have sufficient cash
            }
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });

        purchaseResourceButton.onClick.AddListener(() =>
        {
            ActivatePanel(purchaseUIPanel);
        });

        selectRecipesButton.onClick.AddListener(() =>
        {
            ActivatePanel(selectRecipeUiPanel);
        });

        hireEmployeeButton.onClick.AddListener(() =>
        {
            ActivatePanel(hireEmployeePanel);
        });

        employeeTaskManageButton.onClick.AddListener(() => {
            ActivatePanel(employeeTaskManagerPanel);
        });
    }
    #endregion


    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI totalCostText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    [Space]
    [Header("Panel Buttons")]
    [SerializeField] private Button purchaseResourceButton;
    [SerializeField] private Button selectRecipesButton;
    [SerializeField] private Button hireEmployeeButton;
    [SerializeField] private Button employeeTaskManageButton;

    [Space]
    [Header("UI Panels")]
    [SerializeField] private Transform purchaseUIPanel;
    [SerializeField] private Transform selectRecipeUiPanel;
    [SerializeField] private Transform hireEmployeePanel;
    [SerializeField] private Transform employeeTaskManagerPanel;

    private float totalCost = 0;
    //Hol all hired employees
    private List<EmployeeSO> hiredEmployeeSOList;

    public event Action OnManagerUIShow;
    public event Action OnManagerUIHide;

    public event Action<float> OnPurchaseConfirm;

    private void Start(){

        ManagerCounter.Instance.OnManagerCounterInteraction += ManagerCounter_OnManagerCounterInteraction;

        //For calculating and setting the total cost of purchse
        ResourcePurchaseTemplateUI.OnAnyProductCountIncrease += ResourcePurchaseTemplateUI_OnAnyProductCountIncrease;
        ResourcePurchaseTemplateUI.OnAnyProductCountDecrease += ResourcePurchaseTemplateUI_OnAnyProductCountDecrease;

        //For calculating and setting total cost of employee purchase
        HireEmployeeTemplateUI.OnAnyEmployeeHire += HireEmployeeTemplateUI_OnAnyEmployeeHire;
        HireEmployeeTemplateUI.OnAnyEmployeeHireCancel += HireEmployeeTemplateUI_OnAnyEmployeeHireCancel;
        hiredEmployeeSOList = new List<EmployeeSO>();

        //for setting list of hired employees
        HireEmployeeTemplateUI.OnAnyEmployeeFire += HireEmployeeTemplateUI_OnAnyEmployeeFire;
        HireEmployeeTemplateUI.OnAnyEmployeeFireCancel += HireEmployeeTemplateUI_OnAnyEmployeeFireCancel;

        GameHandler.Instance.OnStateChanged += GameHandler_OnStateChanged;

        RecipeSelectionManager.Instance.OnAvailableRecipeChange += RecipeSelectionManager_OnAvailableRecipeChange;

        EmployeeTaskManagerUI.Instance.OnTaskSimilarityChange += EmployeeTaskManagerUI_OnTaskSimilarityChange;

        Hide();

        SetTotalCost(0);

        ActivatePanel(selectRecipeUiPanel);
    }

    private void EmployeeTaskManagerUI_OnTaskSimilarityChange()
    {
        if (EmployeeTaskManagerUI.Instance.HasTaskSimilarity())
        {
            closeButton.interactable = false;
            confirmButton.interactable = false;
        }
        else
        {
            closeButton.interactable = true;
            confirmButton.interactable = true;
        }
    }

    private void RecipeSelectionManager_OnAvailableRecipeChange()
    {
        //We will not allow player to close the panel if no recipe is selected
        if (RecipeSelectionManager.Instance.HasRecipesAvaialable())
        {
            closeButton.interactable = true;
        }
        else
        {
            closeButton.interactable = false;
        }
    }

    private void GameHandler_OnStateChanged()
    {
        if (GameHandler.Instance.IsGameStopped() && !RecipeSelectionManager.Instance.HasRecipesAvaialable())
        {
            Show();
        }
    }

    private void ResourcePurchaseTemplateUI_OnAnyProductCountDecrease(KitchenObjectSO obj)
    {
        SetTotalCost(totalCost - obj.cost_dollars);
    }
    private void ResourcePurchaseTemplateUI_OnAnyProductCountIncrease(KitchenObjectSO obj)
    {
        SetTotalCost(totalCost + obj.cost_dollars);
    }

    private void HireEmployeeTemplateUI_OnAnyEmployeeHireCancel(EmployeeSO obj)
    {
        SetTotalCost(totalCost - obj.hireCost);
        hiredEmployeeSOList.Remove(obj);
    }
    private void HireEmployeeTemplateUI_OnAnyEmployeeHire(EmployeeSO obj)
    {
        SetTotalCost(totalCost + obj.hireCost);
        hiredEmployeeSOList.Add(obj);
    }

    private void HireEmployeeTemplateUI_OnAnyEmployeeFireCancel(EmployeeSO obj)
    {
        hiredEmployeeSOList.Add(obj);
    }
    private void HireEmployeeTemplateUI_OnAnyEmployeeFire(EmployeeSO obj)
    {
        hiredEmployeeSOList.Remove(obj);
        ChangeConfirmButtonColor();
    }


    private void ManagerCounter_OnManagerCounterInteraction()
    {
        Show();
    }

    private void SetTotalCost(float cost)
    {
        if (totalCost != cost)
        {
            ChangeConfirmButtonColor();
        }


        totalCost = cost;
        //If the cost exceeds the available cash
        //We will change the color
        if (totalCost > CashManager.Instance.GetTotalCash())
        {
            totalCostText.color = Color.red;
        }
        else
        {
            totalCostText.color = Color.white;
        }

        totalCostText.text = "Total Cost: " + totalCost + "$";
    }


    private void ActivatePanel(Transform uiPanel)
    {
        purchaseUIPanel.gameObject.SetActive(false);
        selectRecipeUiPanel.gameObject.SetActive(false);
        hireEmployeePanel.gameObject.SetActive(false);
        employeeTaskManagerPanel.gameObject.SetActive(false);

        uiPanel.gameObject.SetActive(true);
    }


    public List<EmployeeSO> GetHiredEmployeeSOList()
    {
        return hiredEmployeeSOList;
    }

    public void ChangeConfirmButtonColor()
    {
        confirmButton.image.color = Color.red;
    }
    public void RevertConfirmButtonColor()
    {
        confirmButton.image.color = Color.white;
    }

    private void Show(){
        container.SetActive(true);

        OnManagerUIShow?.Invoke();
    }
    private void Hide(){
        container.SetActive(false);

        //Resetting the total cost
        SetTotalCost(0);

        OnManagerUIHide?.Invoke();
    }
}
