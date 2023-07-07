using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject needs;
    [SerializeField] private TextMeshProUGUI tutorialHeader;
    [SerializeField] private TextMeshProUGUI tutorialBody;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private InputActionReference actionKey;
    [SerializeField] private List<GameObject> triggerZones;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float closeMessageDelay = 3.0f;
    [SerializeField] private float panelScaleDuration = 0.5f;

    public bool isTutorialActive = false;
    private Queue<(string, string)> tutorialMessages;
    private Dictionary<string, bool> taskCompletionStatus = new Dictionary<string, bool>();
    private bool canCloseMessage = false;
    private PlayerController playerController;
    private WaitForSecondsRealtime typingWait;
    private WaitForSecondsRealtime closeMessageWait;
    private bool isTyping = false;
    private bool skipTyping = false;
    public bool disableTutorials = false;
    public bool pauseTutorials = false;

    [System.Serializable]
    public class TriggerZone
    {
        public string taskName;
        public float delay;
        public string playerTag = "Player";
        public Collider triggerCollider;
        public bool hasTriggered;
    }
    public void SetTutorialsPaused(bool paused)
    {
        pauseTutorials = paused;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        tutorialMessages = new Queue<(string, string)>();
        tutorialPanel.SetActive(false);
        actionKey.action.performed += ActionKeyPressed;

        playerController = PlayerController.instance;
        typingWait = new WaitForSecondsRealtime(typingSpeed);
        closeMessageWait = new WaitForSecondsRealtime(closeMessageDelay);
    
        if (PlayerPrefs.HasKey("Save"))
        {
            taskCompletionStatus["Start"] = true;
            taskCompletionStatus["CONTROLS"] = true;
            taskCompletionStatus["BUILDING"] = true;
            taskCompletionStatus["WATER"] = true;
            taskCompletionStatus["BED"] = true;
            taskCompletionStatus["PICKAXE"] = true;
            taskCompletionStatus["SWIMMING"] = true;
        }
        else
        {
            TutorialManager.Instance.TaskCompleted("Start", 0.5f);
        }
    }

    public void CheckForTrigger(Collider other, TriggerZone zone)
    {
        if (!zone.hasTriggered && other.CompareTag(zone.playerTag))
        {
            zone.hasTriggered = true;
            TaskCompleted(zone.taskName, zone.delay);
        }
    }

    private void OnDestroy()
    {
        actionKey.action.performed -= ActionKeyPressed;
    }

    private void ActionKeyPressed(InputAction.CallbackContext context)
    {
        if (isTutorialActive)
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else if (canCloseMessage)
            {
                HideTutorial();
            }
        }
    }

    public void ShowTutorial(string header, string body)
    {
        if (isTutorialActive)
        {
            tutorialMessages.Enqueue((header, body));
            return;
        }

        isTutorialActive = true;
        SetTutorialText(header, body);
        StartCoroutine(ShowTimerAndScalePanel(Vector3.zero, Vector3.one, panelScaleDuration));
        Time.timeScale = 0f;
        PlayerController.instance.ToggleCurser(true);
    }

    private IEnumerator ShowTimerAndScalePanel(Vector3 startScale, Vector3 endScale, float duration)
    {
        tutorialPanel.SetActive(true);
        needs.SetActive(false);
        StartCoroutine(ScaleTutorialPanel(startScale, endScale, duration));
        float timer = closeMessageDelay;
        
        while (timer > 0f)
        {
            int seconds = Mathf.FloorToInt(timer);
            int milliseconds = Mathf.FloorToInt((timer - seconds) * 100);
            timerText.text = $"{seconds}:{milliseconds:00}";
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }

        timerText.text = "Press left mouse to close";
        canCloseMessage = true;
    }

    private IEnumerator ScaleTutorialPanel(Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            tutorialPanel.transform.localScale =
                Vector3.Lerp(startScale, endScale, Mathf.SmoothStep(0, 1, elapsedTime / duration));
            yield return null;
        }

        tutorialPanel.transform.localScale = endScale;
    }

    private void SetTutorialText(string header, string body)
    {
        tutorialHeader.text = header;
        StartCoroutine(TypeText(body));
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        canCloseMessage = false;
        tutorialBody.text = "";
        foreach (char letter in text.ToCharArray())
        {
            tutorialBody.text += letter;
            if (skipTyping)
            {
                tutorialBody.text = text;
                break;
            }
            yield return typingWait;
        }
        isTyping = false;
        skipTyping = false;
    }


    private void HideTutorial()
    {
        if (tutorialMessages.Count > 0)
        {
            (string header, string body) = tutorialMessages.Dequeue();
            SetTutorialText(header, body);
        }
        else
        {
            needs.SetActive(true);
            isTutorialActive = false;
            canCloseMessage = false;
            StartCoroutine(ScaleTutorialPanel(tutorialPanel.transform.localScale, Vector3.zero, panelScaleDuration));
            Time.timeScale = 1f;
            playerController.ToggleCurser(false);
        }
    }

    public void TaskCompleted(string taskName, float delay = 0f)
    {
        if (disableTutorials || pauseTutorials) return;

        StartCoroutine(ShowTutorialWithDelay(taskName, delay));
    }


    private IEnumerator ShowTutorialWithDelay(string taskName, float delay)
    {
        yield return new WaitForSeconds(delay);
        while (pauseTutorials)
        {
            yield return null;
        }
        if (taskName == "BOAT" || !taskCompletionStatus.ContainsKey(taskName) || !taskCompletionStatus[taskName])
        {
            switch (taskName)
            {
                case "Start":
                    ShowTutorial("START",
                        $"You're stranded on an island after a fierce storm. To survive, maintain your health, hunger, " +
                        $"thirst, and energy levels while exploring the island for food, water, and essential crafting materials. " +
                        $"Goblins have stolen the boat parts needed to escape, so finding them is crucial. " +
                        $"\nKeep track of your levels on the display in the top left corner of the screen, and replenish them by eating, " +
                        $"drinking, and crafting a bed to rest. Repair the boat and escape to win the game.");
                    break;                     
                case "CONTROLS":
                    ShowTutorial("CONTROLS",
                        $" Move    WASD" +
                        $"\n\nJump    Space" +
                        $"\n\nInteract    E" +
                        $"\n\nAction    Left Mouse" +
                        $"\n\nInventory    Tab" +
                        $"\n\nCycle equipped items    Mouse wheel");
                    break;                        
                case "BUILDING":
                    ShowTutorial("BUILDING",
                        $"The table ahead has all the items required to build a crafting table and a cooking pot." +
                        $"\n\nThese are essential items for crafting and cooking food, with the building hammer equipped, right click to open the building menu." +
                        $"\n\nBuild a crafting table and a cooking pot." +
                        $"\n\nPress R to rotate the item before placing it.");
                    break;                    
                case "WATER":
                    ShowTutorial("WATER", $"Once your bottle is empty, it can be refilled with seawater. " +
                                          "To turn the seawater into clean drinking water, you can use thermal distillation at the cooking pot. " +
                                          "This process heats the seawater, causing it to evaporate and separate from any impurities, leaving you with pure, safe drinking water. " +
                                          "So, make sure to refill your water bottle and utilize the cooking pot for your survival needs.\n\n" +
                                          "You will need to add logs to the fire before using the cooking pot.");
                    break;                        
                case "SWIMMING":
                    ShowTutorial("SWIMMING",
                        $"When swimming be careful not to run out of oxygen or stamina. " +
                        $"\n\nKeep an eye on your oxygen levels, which are displayed in the bottom of your screen." +
                        $"\n\nAlso, your stamina will start to rapidly deplete if you swim for too long. Make sure to surface before it runs out completely.");
                    break;
                
                case "BED":
                    ShowTutorial("BED", $"Dont forget to build a bed, sleeping saves your progress and to restorers stamina." +
                                        "\n\nUse the building hammer to build a bed." +
                                        "\n\nGoblins come out at night, so make sure to sleep before nightfall.");
                    break;
                case "PICKAXE":
                    ShowTutorial("PICKAXE", $"With the pick axe you can mine stone and ore." +
                                            "\n\nCopper is often found near water due to geological processes that create copper-rich deposits in areas with high water content, " +
                                            "while iron is found near large rock formations due to the mineralization process that can create iron-rich rocks. " +
                                            "Coal forms from compressed organic material and is commonly found in grassy areas.");
                    break;
                case "BOAT":
                    ShowTutorial("BOAT", $"Gas Can " +
                                         $"\nEnemy camp " +
                                         $"\n\nGears " +
                                         $"\nFoot of the large tree " +
                                         $"\n\nMotor " +
                                         $"\nBeach cave " +
                                         $"\n\nWheel " +
                                         $"\nIn front of the waterfall ");
                    break;
            }

            if (taskName != "BOAT")
            {
                taskCompletionStatus[taskName] = true;
            }        
        }
    }
}