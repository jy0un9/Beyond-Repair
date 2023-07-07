public class Forge : Buildings, IInteractable
{
    public ForgeWindow forgeWindow;
    private PlayerController player;

    private void Start()
    {
        forgeWindow = FindObjectOfType<ForgeWindow>(true);
        player = FindObjectOfType<PlayerController>();
        AudioManager.Instance.Play("Fire", transform);
    }
    public string GetInteractPrompt()
    {
        return "Forge";
    }

    public void OnInteract()
    {
        forgeWindow.gameObject.SetActive(true);
        player.ToggleCurser(true);
    }
}
