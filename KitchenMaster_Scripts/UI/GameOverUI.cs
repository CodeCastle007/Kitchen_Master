using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeDeliveredText;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });

        restartButton.onClick.AddListener(() =>
        {
            ResetStaticDataMembers();

            Loader.LoadScene(Loader.Scene.GameScene);
        });
    }

    private void Start()
    {
        GameHandler.Instance.OnStateChanged += GameHandler_OnStateChanged;

        Hide();
    }

    private void GameHandler_OnStateChanged()
    {
        if (GameHandler.Instance.IsGameOver())
        {
            Show();
            recipeDeliveredText.text = DeliveryManager.Instance.GetDeliveredRecipeCount().ToString();
        }
        else
        {
            Hide();
        }
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void ResetStaticDataMembers()
    {
        DeliveryTable.ResetStaticData();
        CuttingCounter.ResetStaticData();
        BaseCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
    }
}
