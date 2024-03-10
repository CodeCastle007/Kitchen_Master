using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressObject;
    private IHasProgress hasProgresss;
    [SerializeField] private Image progressBar;

    private void Start()
    {
        hasProgresss = hasProgressObject.GetComponent<IHasProgress>();
        if (hasProgresss == null)
        {
            Debug.LogError("Game Object " + hasProgressObject + "does not have IHasProgress interface implemented");
        }

        hasProgresss.OnProgressChanged += HasProgress_OnProgressChanged;
        progressBar.fillAmount = 0;
        Hide();
    }

    private void HasProgress_OnProgressChanged(float progress)
    {
        progressBar.fillAmount = progress;

        if(progressBar.fillAmount==0 || progressBar.fillAmount == 1)
        {
            Hide();
        }
        else
        {
            Show();
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
