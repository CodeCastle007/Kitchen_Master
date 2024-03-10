using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDownTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownTimerText;

    private void Start()
    {
        GameHandler.Instance.OnStateChanged += GameHandler_OnStateChanged;

        Hide();
    }

    private void GameHandler_OnStateChanged()
    {
        if(GameHandler.Instance.IsCountingDownToStart())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {
        countDownTimerText.text = Mathf.Ceil(GameHandler.Instance.GetCountDownToStartTimer()).ToString();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
}
