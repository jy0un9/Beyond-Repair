using System.Collections;
using UnityEngine;
using UniStorm;

public class Bed : Buildings, IInteractable
{
    public int wakeupTime;
    public float startSleepTime;
    public float endSleepTime;
    public float sleepTogive;
    private GameObject fadeBlack;

    private void Start()
    {
        fadeBlack = GameObject.FindWithTag("Fade");
    }

    public string GetInteractPrompt()
    {
        return CanSleep() ? "sleep" : "Can not Sleep";
    }

    public void OnInteract()
    {
        if (CanSleep())
        {
            fadeBlack.GetComponent<Animator>().SetTrigger("Sleep");
            PlayerNeeds.instance.sleep.Add(600);
            PlayerNeeds.instance.health.Add(600);
            StartCoroutine(SleepWait());
        }
    }

    IEnumerator SleepWait()
    {
        yield return new WaitForSeconds(2);
        fadeBlack.GetComponent<Animator>().ResetTrigger("Sleep");
        UniStormManager.Instance.SetTime(7, 0);
        StartCoroutine(SaveGame());
    }

    IEnumerator SaveGame()
    {
        yield return new WaitForSeconds(2);
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        gameManager.GetComponent<SaveManager>().Save();
    }

    bool CanSleep()
    {
        return UniStormSystem.Instance.Hour >= startSleepTime || UniStormSystem.Instance.Hour <= endSleepTime;
    }
}