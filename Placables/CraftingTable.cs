public class CraftingTable : Buildings, IInteractable
{
    public CraftingWindow craftingWindow;
    private PlayerController player;

    private void Start()
    {
        craftingWindow = FindObjectOfType<CraftingWindow>(true);
        player = FindObjectOfType<PlayerController>();
    }
    public string GetInteractPrompt()
    {
        return "Craft";
    }

    public void OnInteract()
    {
        craftingWindow.gameObject.SetActive(true);
        player.ToggleCurser(true);
    }
}
