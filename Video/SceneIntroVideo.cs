using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SceneIntroVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string videoFileName;
    public PlayerInput playerInput;
    public EventSystem eventSystem;
    [SerializeField] private GameObject videoCanvas;
    public bool IsVideoPlaying { get; private set; }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Save"))
        {
            videoCanvas.SetActive(false);
        }
        else
        {
            StartCoroutine(PlayVideoAndPauseGame());
        }
    }

    private void SetAudioSourcesPauseState(bool pause)
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            if (pause)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
            }
        }
    }

    private IEnumerator PlayVideoAndPauseGame()
    {
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName + ".mp4").Replace("\\", "/");
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Time.timeScale = 0f;
        SetAudioSourcesPauseState(true);
        PlayerController.instance.enabled = false;
        playerInput.enabled = false;
        eventSystem.enabled = false;
        PauseMenu.Instance.PauseEnabled = false;
        TutorialManager.Instance.SetTutorialsPaused(true);
        IsVideoPlaying = true;
        videoPlayer.Play();

        while (videoPlayer.isPlaying)
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }

        IsVideoPlaying = false;
        videoCanvas.SetActive(false);
        TutorialManager.Instance.SetTutorialsPaused(false);
        SetAudioSourcesPauseState(false);
        PlayerController.instance.enabled = true;
        playerInput.enabled = true;
        eventSystem.enabled = true;
        PauseMenu.Instance.PauseEnabled = true;
        Time.timeScale = 1f;
    }
}