public class CookingPot : Buildings, IInteractable
{
    public CookingWindow cookingWindow;
    public CookingFire cookingFire;
    private PlayerController player;

    private void Start()
    {
        cookingWindow = FindObjectOfType<CookingWindow>(true);
        player = FindObjectOfType<PlayerController>();
    }
    
    public string GetInteractPrompt()
    {
        return cookingFire.IsLit() ? "Cook" : "Light fire to cook";
    }

    public void OnInteract()
    {
        if (cookingFire.IsLit())
        {
            cookingWindow.gameObject.SetActive(true);
            player.ToggleCurser(true);
        }
    }
}