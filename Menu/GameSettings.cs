using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public float MasterVolume { get; private set; }
    public int ResolutionIndex { get; private set; }
    public int ScreenModeIndex { get; private set; }
    public int VSync { get; private set; }
    public AudioMixer myMixer;
    public bool IsInitialLoad { get; set; } = true;
    private Resolution[] resolutions { get; set; }
    public List<Resolution> FilteredResolutions;

    private void Awake()
    {
        resolutions = Screen.resolutions;
        FilteredResolutions = FilterResolutions(resolutions);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private static List<Resolution> FilterResolutions(IEnumerable<Resolution> allResolutions)
    {
        List<Resolution> result = new List<Resolution>();
        int[] desiredHeights = { 720, 1080, 1440, 2160 };

        foreach (Resolution res in allResolutions)
        {
            if (System.Array.IndexOf(desiredHeights, res.height) != -1)
            {
                bool exists = false;
                foreach (Resolution existingRes in result)
                {
                    if (existingRes.width == res.width && existingRes.height == res.height)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    result.Add(res);
                }
            }
        }

        return result;
    }

    public void UpdateMasterVolume(float newVolume)
    {
        MasterVolume = newVolume;
    }

    public void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        ResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", -1);
        ScreenModeIndex = PlayerPrefs.GetInt("ScreenModeIndex", -1);
        VSync = PlayerPrefs.GetInt("VSync", 0);

        ApplySettings();
    }

    public void SaveSettings(float masterVolume, int resolutionIndex, int screenModeIndex, int vSync)
    {
        if (!IsInitialLoad)
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
            PlayerPrefs.SetInt("ScreenModeIndex", screenModeIndex);
            PlayerPrefs.SetInt("VSync", vSync);
            PlayerPrefs.Save();
        }
    }

    public void SetResolutionIndex(int index)
    {
        ResolutionIndex = index;
    }

    public void SetScreenModeIndex(int index)
    {
        ScreenModeIndex = index;
    }

    public void ApplySettings()
    {
        myMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20);
        QualitySettings.vSyncCount = VSync;

        if (ResolutionIndex >= 0 && ResolutionIndex < FilteredResolutions.Count)
        {
            Screen.SetResolution(FilteredResolutions[ResolutionIndex].width, FilteredResolutions[ResolutionIndex].height, Screen.fullScreenMode);
        }

        if (ScreenModeIndex == 0)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (ScreenModeIndex == 1)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}