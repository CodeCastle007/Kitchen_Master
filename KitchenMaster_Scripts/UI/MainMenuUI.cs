using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button optionsButton;

    [SerializeField] private AudioClip audioClip;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            Loader.LoadScene(Loader.Scene.GameScene);

            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, 1f);
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, 1f);
        });

        optionsButton.onClick.AddListener(() =>
        {
            OptionsUI.Instance.Show();
            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, 1f);
        });

        Time.timeScale = 1;
    }
}
