using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternateText;
    [SerializeField] private TextMeshProUGUI pauseText;


    private void Start()
    {
        GameHandler.Instance.OnStateChanged += GameHandler_OnStateChanged;

        Show();

        moveUpText.text = InputHandler.Instance.GetBindingText(InputHandler.Binding.Move_UP);
        moveDownText.text = InputHandler.Instance.GetBindingText(InputHandler.Binding.Move_Down);
        moveLeftText.text = InputHandler.Instance.GetBindingText(InputHandler.Binding.Move_Left);
        moveRightText.text = InputHandler.Instance.GetBindingText(InputHandler.Binding.Move_Right);
        interactText.text = InputHandler.Instance.GetBindingText(InputHandler.Binding.Interact);
        interactAlternateText.text = InputHandler.Instance.GetBindingText(InputHandler.Binding.InteractAlt);
        pauseText.text = InputHandler.Instance.GetBindingText(InputHandler.Binding.Pause);
    }

    private void GameHandler_OnStateChanged()
    {
        if (!GameHandler.Instance.IsWaitingToStart())
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
