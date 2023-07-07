using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsPanel;
    public InputActionAsset inputActions;
    public PlayerController playerMovement;
    public AudioMixer audioMixer;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Toggle vSyncToggle;

    private InputAction _escapeAction;
    private bool _isPaused = false;
    private bool _settingsOpen = false;
    private Resolution[] _resolutions;
    private static PauseMenu _instance;
    public bool PauseEnabled { get; set; } = true;

    public static PauseMenu Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PauseMenu>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _escapeAction = inputActions.FindActionMap("PauseMenu").FindAction("TogglePause");
        _escapeAction.performed += _ => TogglePause();
    }

    private void Start()
    {
        PopulateDropdowns();
        LoadSettings();

    }

    private void OnEnable()
    {
        _escapeAction.Enable();
    }

    private void OnDisable()
    {
        _escapeAction.Disable();

    }

    public void TogglePause()
    {
        SceneIntroVideo introVideo = FindObjectOfType<SceneIntroVideo>();
        if (introVideo != null && introVideo.IsVideoPlaying)
        {
            return;
        }
        if (!PauseEnabled) return;

        _isPaused = !_isPaused;
        pauseMenuUI.SetActive(_isPaused);
        AudioManager.Instance.PlayOneShot("Bag");

        if (_isPaused)
        {
            Pause();
            if (_settingsOpen)
            {
                OnSettingsButton();
            }
        }
        else
        {
            Resume();
        }
    }


    public bool IsPauseMenuOpen()
    {
        return _isPaused;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerMovement.enabled = true;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        _isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        playerMovement.enabled = false;
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Tittle");
    }

    public void OnSettingsButton()
    {
        _settingsOpen = !_settingsOpen;
        settingsPanel.SetActive(_settingsOpen);
        if (_settingsOpen)
        {
            LoadSettings();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void PopulateDropdowns()
    {
        if (resolutionDropdown == null || screenModeDropdown == null || masterVolumeSlider == null || vSyncToggle == null)
        {
            return;
        }
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
        masterVolumeSlider.onValueChanged.RemoveAllListeners();
        masterVolumeSlider.value = GameSettings.Instance.MasterVolume;
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        GameSettings.Instance.IsInitialLoad = false;
        vSyncToggle.isOn = GameSettings.Instance.VSync == 1;
    }

    public void SaveSettings()
    {
        GameSettings.Instance.SaveSettings(
            masterVolumeSlider.value,
            resolutionDropdown.value,
            screenModeDropdown.value,
            vSyncToggle.isOn ? 1 : 0
        );
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
        GameSettings.Instance.SaveSettings(
            GameSettings.Instance.MasterVolume,
            GameSettings.Instance.ResolutionIndex,
            GameSettings.Instance.ScreenModeIndex,
            GameSettings.Instance.VSync
        );
        GameSettings.Instance.ApplySettings();
    }

    public void OnScreenModeDropdownChanged(int index)
    {
        GameSettings.Instance.SetScreenModeIndex(index);
        GameSettings.Instance.SaveSettings(
            GameSettings.Instance.MasterVolume,
            GameSettings.Instance.ResolutionIndex,
            GameSettings.Instance.ScreenModeIndex,
            GameSettings.Instance.VSync
        );
        GameSettings.Instance.ApplySettings();
    }
}