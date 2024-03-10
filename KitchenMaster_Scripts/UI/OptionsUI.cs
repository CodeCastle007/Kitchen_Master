using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    private const string PLAYERPREF_GRAPHICS_QUALITY_INDEX = "QualitySetting";
    private const string PLAYERPREF_RESOLUTION_INDEX = "Resolution";

    public static OptionsUI Instance { get; private set; }

    private const string PLAYERPREFS_MUSIC_MANAGER = "MusicVolume";
    private const string PLAYERPREFS_SOUND_VOLUME = "SoundVolume";

    private const string FPS_COUNTER = "FpsCounter";

    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Dropdown graphicsQualityDropDown;
    [SerializeField] private TMP_Dropdown resolutionDropDown;

    [SerializeField] private Toggle fpsToggle;
    [SerializeField] private Transform fpsCounterTransform;

    private Resolution[] resolutions;

    public event Action<float> OnSoundValueChanged;
    public event Action<float> OnMusicVolumeChanged;
    private void Awake()
    {
        Instance = this;

        soundSlider.onValueChanged.AddListener((float value) =>
        {
            OnSoundValueChanged?.Invoke(value);
            //SoundManager.Instance.SetVolume(value);

            PlayerPrefs.SetFloat(PLAYERPREFS_SOUND_VOLUME, value);
            PlayerPrefs.Save();
        });

        musicSlider.onValueChanged.AddListener((float value) =>
        {
            OnMusicVolumeChanged?.Invoke(value);
            // MusicManager.Instance.SetVolume(value);

            PlayerPrefs.SetFloat(PLAYERPREFS_MUSIC_MANAGER, value);
            PlayerPrefs.Save();
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });

        graphicsQualityDropDown.onValueChanged.AddListener((int value) =>
        {
            QualitySettings.SetQualityLevel(value);

            PlayerPrefs.SetInt(PLAYERPREF_GRAPHICS_QUALITY_INDEX, value);
            PlayerPrefs.Save();
        });

        resolutionDropDown.onValueChanged.AddListener((int value) =>
        {
            Resolution resolution = resolutions[value];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            PlayerPrefs.SetInt(PLAYERPREF_RESOLUTION_INDEX, value);
            PlayerPrefs.Save();
        });

        fpsToggle.onValueChanged.AddListener((bool value) =>
        {
            if (value)
            {
                PlayerPrefs.SetInt(FPS_COUNTER, 1);
            }
            else
            {
                PlayerPrefs.SetInt(FPS_COUNTER, 0);
            }
            fpsCounterTransform.gameObject.SetActive(value);
        });
    }

    private void Start()
    {
        GameHandler gameHanlder = GameHandler.Instance;
        if (gameHanlder)
        {
            gameHanlder.OnUnPause += GameHandler_OnUnPause;
        }

        SetresolutionDropDown();
        UpdateVisual();
        Hide();
    }

    private void SetresolutionDropDown()
    {
        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();
        List<string> resolutionOptions = new List<string>();

        //int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(option);
        }

        resolutionDropDown.AddOptions(resolutionOptions);
        resolutionDropDown.RefreshShownValue();
    }

    private void UpdateVisual()
    {
        soundSlider.value = PlayerPrefs.GetFloat(PLAYERPREFS_SOUND_VOLUME, .5f);
        musicSlider.value = PlayerPrefs.GetFloat(PLAYERPREFS_MUSIC_MANAGER, .3f);

        int index = PlayerPrefs.GetInt(PLAYERPREF_GRAPHICS_QUALITY_INDEX, 2);
        QualitySettings.SetQualityLevel(index);
        graphicsQualityDropDown.value = index;

        index = PlayerPrefs.GetInt(PLAYERPREF_RESOLUTION_INDEX, resolutions.Length - 1);
        resolutionDropDown.value = index;
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);


        int activate = PlayerPrefs.GetInt(FPS_COUNTER, 1);
        fpsToggle.isOn = (activate == 1) ? true : false;
        //fpsCounterTransform.gameObject.SetActive(fpsToggle.isOn);
    }

    private void GameHandler_OnUnPause()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
