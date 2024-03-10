using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTextManager : MonoBehaviour
{
    #region Singleton
    public static MessageTextManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [SerializeField] private MessageTextUI messageUI;



    private void Start()
    {
        Player.Instance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
    }

    private void Player_OnSelectedCounterChange(BaseCounter obj)
    {
        if (obj == null)
        {
            messageUI.Hide();
        }
        else
        {
            messageUI.Show(obj.GetTutorialMessage(), Mathf.Infinity);
        }
    }

    public void DisplayTutorialMessage(string message)
    {
        messageUI.Show(message,Mathf.Infinity);
    }
}
