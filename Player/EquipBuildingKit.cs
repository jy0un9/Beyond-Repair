using UnityEngine;
using UnityEngine.InputSystem;

public class EquipBuildingKit : Equip
{
    public GameObject buildingWindow;
    private BuildingRecipe currentRecipe;
    private BuildingPreview currentBuildingPreview;
    public float placemtntUpdateRate = 0.03f;
    private float lastPlacmentUpdateTime;
    public float placementMaxDistance = 5.0f;
    public LayerMask placementLayerMask;
    public Vector3 placementPosition;
    private bool canPlace;
    private float currentYRotation;
    public float rotateSpeed = 180.0f;
    public static EquipBuildingKit instance;
    private Camera cam;

    private void Awake()
    {
        instance = this;
        cam = Camera.main;
    }

    private void Start()
    {
        buildingWindow = FindObjectOfType<BuildingWindow>(true).gameObject;
    }
    public override void OnAttackInput()
    {
        if (currentRecipe == null || currentBuildingPreview == null || !canPlace)
            return;

        Instantiate(currentRecipe.spawnPrefab, currentBuildingPreview.transform.position, currentBuildingPreview.transform.rotation);
        AudioManager.Instance.PlayOneShot("Place");
        for(int x = 0; x < currentRecipe.cost.Length; x++)
        {
            Inventory.instance.TryRemoveItem(currentRecipe.cost[x].item, currentRecipe.cost[x].quantity);
        }
        Inventory.instance.UpdateUI();

        currentRecipe = null;
        Destroy(currentBuildingPreview.gameObject);
        currentBuildingPreview = null;
        canPlace = false;
        currentYRotation = 0;
    }


    public override void OnAltAttackInput()
    {
        if (currentBuildingPreview != null)
            Destroy(currentBuildingPreview.gameObject);

        buildingWindow.SetActive(true);
        PlayerController.instance.ToggleCurser(true);
    }

    public void SetNewBuildingRecipe(BuildingRecipe recipe)
    {
        currentRecipe = recipe;
        buildingWindow.SetActive(false);
        PlayerController.instance.ToggleCurser(false);

        currentBuildingPreview = Instantiate(recipe.previewPrefab).GetComponent<BuildingPreview>();
    }

    void Update()
    {
        if(currentRecipe != null && currentBuildingPreview != null && Time.time - lastPlacmentUpdateTime > placemtntUpdateRate)
        {
            lastPlacmentUpdateTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if(Physics.Raycast(ray,out hit,placementMaxDistance, placementLayerMask))
            {
                currentBuildingPreview.transform.position = hit.point;
                currentBuildingPreview.transform.up = hit.normal;
                currentBuildingPreview.transform.Rotate(new Vector3(0, currentYRotation, 0), Space.Self);

                if(!currentBuildingPreview.CollidingWithObjects())
                {
                    if (!canPlace)
                        currentBuildingPreview.CanPlace();

                    canPlace = true;
                }
                else
                {
                    if (canPlace)
                        currentBuildingPreview.CannotPlace();

                    canPlace = false;
                }
            }
        }

        if(Keyboard.current.rKey.isPressed)
        {
            currentYRotation += rotateSpeed * Time.deltaTime;

            if (currentYRotation > 360.0f)
            {
                currentYRotation = 0.0f;
            }
        }
    }

    void OnDestroy()
    {
        if (currentBuildingPreview != null)
            Destroy(currentBuildingPreview.gameObject);
    }
}