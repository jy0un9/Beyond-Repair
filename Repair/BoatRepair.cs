using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatRepair : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public struct RequiredItem
    {
        public ItemData item;
        public int quantity;
    }

    public RequiredItem[] requiredItems;
    public SceneTransitions sceneTransitions;
    public GameObject requiredItemsPanel;
    public List<Image> iconPlaceholders;
    public PlayerController playerController;
    public Button repairButton;
    private Inventory _playerInventory;

    private void Awake()
    {
        requiredItemsPanel.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(DelayedStart());
        requiredItemsPanel.SetActive(false);
    }
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1);
        _playerInventory = FindObjectOfType<Inventory>();
        UpdateIconPlaceholders();
        UpdateRepairButtonVisibility();
    }

    public string GetInteractPrompt()
    {
        return "Boat";
    }

    public void OnInteract()
    {
        AudioManager.Instance.PlayOneShot("Bag");

        requiredItemsPanel.SetActive(!requiredItemsPanel.activeSelf);

        if (requiredItemsPanel.activeSelf)
        {
            UpdateIconPlaceholders();
            UpdateRepairButtonVisibility();
            playerController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            playerController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    private bool HasRequiredItems()
    {
        foreach (RequiredItem requiredItem in requiredItems)
        {
            if (!_playerInventory.HasItems(requiredItem.item, requiredItem.quantity))
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateIconPlaceholders()
    {
        for (int i = 0; i < requiredItems.Length; i++)
        {
            RequiredItem requiredItem = requiredItems[i];

            iconPlaceholders[i].sprite = requiredItem.item.icon;
            iconPlaceholders[i].color = _playerInventory.HasItems(requiredItem.item, requiredItem.quantity) ? Color.green : Color.red;
        }
    }

    private void UpdateRepairButtonVisibility()
    {
        repairButton.gameObject.SetActive(HasRequiredItems());
    }
    
    public void Close()
    {
        AudioManager.Instance.PlayOneShot("Bag");
        requiredItemsPanel.SetActive(false);
        playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void RepairBoat()
    {
        AudioManager.Instance.PlayOneShot("Click");
        requiredItemsPanel.SetActive(false);
        sceneTransitions.LoadSceneWithName("FinalCutScene");
    }
    
    public void Hint()
    {
        AudioManager.Instance.PlayOneShot("Click");
        TutorialManager.Instance.TaskCompleted("BOAT", 0.1f);
        Close();
    }
}