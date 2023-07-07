using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public Button continueButton;
    public GameObject loadingPanel;
    public Slider loadingBar;
    public Image blackBackgroundImage;
    public GameObject settingsPanel;
    public TMP_Dropdown resolutionDropdown;
    public Slider masterVolumeSlider;
    public Toggle vSyncToggle;
    public TMP_Dropdown screenModeDropdown;
    private bool _settingsOpen = false;
    private Resolution[] _resolutions;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Save"))
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.gameObject.SetActive(false);
        }
        
        loadingPanel.SetActive(false);
        settingsPanel.SetActive(false);
        AudioManager.Instance.Play("Adventure1", transform);
        GameSettings.Instance.ApplySettings();
        SetupButtonHoverAudio();
    }

    private void Awake()
    {
        PopulateDropdowns();
        LoadSettings();
    }

    private void PopulateDropdowns()
    {
        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;
        _resolutions = GameSettings.Instance.FilteredResolutions.ToArray();

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            resolutionOptions.Add(option);

            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        List<string> screenModeOptions = new List<string>
        {
            "Full Screen",
            "Windowed"
        };

        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(screenModeOptions);
    }

    private void LoadSettings()
    {
        resolutionDropdown.value = GameSettings.Instance.ResolutionIndex;
        screenModeDropdown.value = GameSettings.Instance.ScreenModeIndex;
        masterVolumeSlider.value = GameSettings.Instance.MasterVolume;
        vSyncToggle.isOn = GameSettings.Instance.VSync == 1;
        masterVolumeSlider.onValueChanged.RemoveAllListeners();
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        GameSettings.Instance.IsInitialLoad = false;
    }

    public void SaveSettings()
    {
        GameSettings.Instance.SaveSettings(masterVolumeSlider.value, resolutionDropdown.value, screenModeDropdown.value,
            vSyncToggle.isOn ? 1 : 0);
        GameSettings.Instance.ApplySettings();
    }

    public void OnVSyncToggle(bool enabled)
    {
        SaveSettings();
    }

    public void SetMasterVolume(float volume)
    {
        GameSettings.Instance.UpdateMasterVolume(volume);
        GameSettings.Instance.myMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        SaveSettings();
        GameSettings.Instance.ApplySettings();
    }

    public void OnResolutionDropdownChanged(int index)
    {
        GameSettings.Instance.SetResolutionIndex(index);
        GameSettings.Instance.SaveSettings(GameSettings.Instance.MasterVolume, GameSettings.Instance.ResolutionIndex, GameSettings.Instance.ScreenModeIndex, GameSettings.Instance.VSync);
        GameSettings.Instance.ApplySettings();
    }

    public void OnScreenModeDropdownChanged(int index)
    {
        GameSettings.Instance.SetScreenModeIndex(index);
        GameSettings.Instance.SaveSettings(GameSettings.Instance.MasterVolume, GameSettings.Instance.ResolutionIndex, GameSettings.Instance.ScreenModeIndex, GameSettings.Instance.VSync);
        GameSettings.Instance.ApplySettings();
    }

    public void OnContinueButton()
    {
        StartCoroutine(LoadSceneAsync("Island_New", true));
    }

    public void OnNewGameButton()
    {
        PlayerPrefs.DeleteKey("Save");
        StartCoroutine(LoadSceneAsync("Island_New", false));
    }

    public void OnSettingsButton()
    {
        _settingsOpen = !_settingsOpen;
        settingsPanel.SetActive(_settingsOpen);
    }

    public void PlayAudioOnHover()
    {
        AudioManager.Instance.PlayOneShot("ButtonHover1");
    }

    public void StopAudioOnHoverExit()
    {
        AudioManager.Instance.Stop("ButtonHover1");
    }

    private void SetupButtonHoverAudio()
    {
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            pointerEnterEntry.callback.AddListener((eventData) => PlayAudioOnHover());
            eventTrigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            pointerExitEntry.callback.AddListener((eventData) => StopAudioOnHoverExit());
            eventTrigger.triggers.Add(pointerExitEntry);
        }
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool isContinue)
    {
        loadingPanel.SetActive(true);

        yield return StartCoroutine(FadeInBackground(0.5f));

        if (!isContinue)
        {
            PlayerPrefs.DeleteKey("Save");
        }

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            loadingBar.value = progress;

            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator FadeInBackground(float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            blackBackgroundImage.color = new Color(blackBackgroundImage.color.r, blackBackgroundImage.color.g,
                blackBackgroundImage.color.b, alpha);
            yield return null;
        }
    }
}