using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;
    private GameObject currentInterectGameObject;
    private IInteractable currentInteractable;
    public TextMeshProUGUI promptText;
    private Camera cam;
    public static bool active;

    private void Start()
    {
        cam = Camera.main;
    }
    //?
    private void FixedUpdate()
    {
        if(Time.time -lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if(hit.collider.gameObject != currentInterectGameObject)
                {
                    currentInterectGameObject = hit.collider.gameObject;
                    currentInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                    active = true;
                }
            }
            else
            {
                currentInteractable = null;
                currentInterectGameObject = null;
                
                if (promptText != null)
                {
                    promptText.gameObject.SetActive(false);
                }
                active = false;
            }
        }
    }

    void SetPromptText()
    {
        if (promptText.gameObject != null && currentInteractable != null)
        {
            promptText.gameObject.SetActive(true);
            promptText.text = string.Format("<b>[E]</b> {0}", currentInteractable.GetInteractPrompt());
        }
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && currentInteractable != null)
        {
            currentInteractable.OnInteract();
            currentInterectGameObject = null;
            currentInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}

public interface IInteractable
{
    string GetInteractPrompt();
    void OnInteract();
}