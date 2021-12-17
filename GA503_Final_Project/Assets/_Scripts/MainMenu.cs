using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [Space]
    [SerializeField] private CanvasGroup optionsScreen;
    [SerializeField] private Button closeOptionsButton;
    [Space]
    [SerializeField] private AudioMixer masterAudioMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void Awake()
    {
        startButton.onClick.AddListener((StartGame));
        optionsButton.onClick.AddListener(OpenOptions);
        closeOptionsButton.onClick.AddListener(CloseOptions);
        
        InitVolumeSlider(masterVolumeSlider, "MasterVol");
        InitVolumeSlider(musicVolumeSlider, "MusicVol");
        InitVolumeSlider(sfxVolumeSlider, "SfxVol");
    }

    private void InitVolumeSlider(Slider argSlider, string argVolumeParamName)
    {
        masterAudioMixer.GetFloat(argVolumeParamName, out float mixerDbValue);
        mixerDbValue = (mixerDbValue <= -80f) ? 0f : (mixerDbValue >= 0f)? 1f : Mathf.Pow(10, mixerDbValue) / -20f;
        argSlider.SetValueWithoutNotify(mixerDbValue);
        argSlider.onValueChanged.AddListener((value =>
        {
            float dbValue = (value <= 0f) ? -80f : Mathf.Log10(value) * 20f;
            masterAudioMixer.SetFloat(argVolumeParamName, dbValue);
        }));
    }

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    
    private void OpenOptions()
    {
        optionsScreen.gameObject.SetActive(true);
    }
    
    private void CloseOptions()
    {
        optionsScreen.gameObject.SetActive(false);
    }
}
