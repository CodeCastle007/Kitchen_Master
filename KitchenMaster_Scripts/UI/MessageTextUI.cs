using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageTextUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private float showTimer = 0;
    private float currentShowTimer;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        if(showTimer > 0)
        {
            currentShowTimer -= Time.deltaTime;
            if (currentShowTimer <= 0)
            {
                Hide();

                showTimer = 0;
            }
        }
    }

    public void Show(string text,float time)
    {
        container.gameObject.SetActive(true);

        messageText.text = text;

        showTimer = time;
        currentShowTimer = showTimer;
    }
    public void Hide()
    {
        container.gameObject.SetActive(false);

        showTimer = 0;
    }
}
