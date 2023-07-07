public class Anvil : Buildings, IInteractable
{
    public AnvilWindow anvilWindow;
    private PlayerController player;

    private void Start()
    {
        anvilWindow = FindObjectOfType<AnvilWindow>(true);
        player = FindObjectOfType<PlayerController>();
    }
    public string GetInteractPrompt()
    {
        return "Anvil";
    }

    public void OnInteract()
    {
        anvilWindow.gameObject.SetActive(true);
        player.ToggleCurser(true);
    }
}
