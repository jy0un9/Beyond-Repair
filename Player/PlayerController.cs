using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6;
    [SerializeField] float jumpForce = 90;
    [SerializeField] LayerMask groundLayerMask;
    private Vector2 currentMovementInput;

    [Header("Look")]
    public Transform cameraContainer;
    [SerializeField] float minXLook = -80;
    [SerializeField] float maxXLook = 80;
    [SerializeField] float lookSensitivity = 0.2f;
    private Vector2 mouseDelta;
    private float camCurXRot;

    [Header("Swimming")]
    [SerializeField] float swimSpeed = 4;
    [SerializeField] float SinkSpeed = 1.2f;
    [SerializeField] float SwimVelocityY = 0.02f;
    [SerializeField] float swimDrag = 15;
    [SerializeField] GameObject oxygenBar;
    [SerializeField] Slider oxygenFillBar;
    [SerializeField] ParticleSystem splash;
    private bool isSwimming = false;

    [HideInInspector]
    public bool canLook = true;
    private Vector3 velocity = Vector3.zero;

    private Rigidbody rig;
    public static PlayerController instance;
    bool isJumping = false;
    public bool fire = false;

    public TextMeshProUGUI fillWaterMessage; 
    public PlayerInput playerInput;
    public List<GameObject> uiWindows;
    private float oxygenBarActiveTime = 0f;
    private float oxygenBarInactiveTime = 0f;
    private float oxygenBarInactiveCooldown = 0.2f;
    private float lastSplashTime = -1f;
    private bool hasPlayedChokeSound = false;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        instance = this;
        oxygenFillBar.value = 100;
        playerInput = GetComponent<PlayerInput>();
    }
    private bool IsAnyUIWindowActive()
    {
        foreach (GameObject uiWindow in uiWindows)
        {
            if (uiWindow.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    
    private void UpdatePlayerInput()
    {
        if (IsAnyUIWindowActive())
        {
            playerInput.actions["CycleQuickSlots"].Disable();
        }
        else
        {
            playerInput.actions["CycleQuickSlots"].Enable();
        }
    }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        AudioManager.Instance.FadeIn("Adventure1", 2f);
    }

    void FixedUpdate()
    {
        Move();
        
        if (isJumping)
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
            isJumping = false;
        }
    }
    void Update()
    {
        if (isSwimming)
        {
            oxygenBar.SetActive(true);
            oxygenFillBar.value -= 4 * Time.deltaTime;
            oxygenBarActiveTime += Time.deltaTime;

            if (oxygenBarActiveTime > 25f)
            {
                PlayerNeeds.instance.sleep.Subtract(500f * Time.deltaTime);
            }

            oxygenBarInactiveTime = 0f;
        }
        else
        {
            oxygenBar.SetActive(false);
            oxygenFillBar.value += 10 * Time.deltaTime;
            oxygenBarInactiveTime += Time.deltaTime;

            if (oxygenBarInactiveTime > oxygenBarInactiveCooldown)
            {
                oxygenBarActiveTime = 0f;
            }
        }

        if (oxygenFillBar.value <= 15)
        {
            Choke();
        }
        else
        {
            AudioManager.Instance.Stop("Choke");
            hasPlayedChokeSound = false;
        }

        if (oxygenFillBar.value <= 0)
        {
            PlayerNeeds.instance.Die();
        }
        if (Keyboard.current.eKey.wasPressedThisFrame && fillWaterMessage.gameObject.activeInHierarchy)
        {
            FillWaterBottle();
        }
        UpdatePlayerInput();
    }

    private void Choke()
    {
        if (!hasPlayedChokeSound)
        {
            AudioManager.Instance.PlayOneShot("Choke");
            hasPlayedChokeSound = true;
        }
    }
    
    private void FillWaterBottle()
    {
        Inventory.instance.ReplaceItem(Inventory.instance.waterBottleEmpty, Inventory.instance.seaWaterBottle);
        fillWaterMessage.gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if(canLook == true)
        {
            CameraLook();
        }
    }

    private void PlaySplashSound()
    {
        float currentTime = Time.time;
        if (currentTime - lastSplashTime >= 1f)
        {
            AudioManager.Instance.PlayOneShot("Splash");
            lastSplashTime = currentTime;
        }
    }

    private void Move()
    {
        Vector3 dir = transform.forward * currentMovementInput.y + transform.right * currentMovementInput.x;
        if (isSwimming)
        {
            if (PlayerNeeds.instance.sleep.currentValue <= 0)
            {
                rig.velocity += Vector3.down * SinkSpeed * Time.fixedDeltaTime;
            }
            else
            {
                PlayerNeeds.instance.sleep.Subtract(1f * Time.deltaTime);
                rig.drag = swimDrag;
                dir *= swimSpeed;
                dir.y = rig.velocity.y / SinkSpeed;
                dir.y += camCurXRot * SwimVelocityY;
            }
        }
        else
        {
            rig.drag = 0;
            dir *= moveSpeed;
            dir.y = rig.velocity.y;
        }
        rig.velocity = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterVolume"))
        {
            isSwimming = true;
            splash.Play();
            TutorialManager.Instance.TaskCompleted("SWIMMING", 0.5f);

            AudioManager.Instance.FadeIn("Underwater", 0.2f);
            PlaySplashSound();
        }

        if (other.CompareTag("FillWaterVolume"))
        {
            bool hasWaterBottleEmpty = Inventory.instance.HasItems(Inventory.instance.waterBottleEmpty, 1);
            if (hasWaterBottleEmpty)
            {
                fillWaterMessage.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WaterVolume"))
        {
            isSwimming = false;
            splash.Play();
            PlaySplashSound();
            AudioManager.Instance.FadeOut("Underwater", 1f);
        }

        if (other.CompareTag("FillWaterVolume"))
        {
            fillWaterMessage.gameObject.SetActive(false);
        }
    }
    
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity,0);
    }
    
    public void OnFireInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            fire = true;
        }
    }
    
    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();

    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            currentMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            currentMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            if (context.phase == InputActionPhase.Started)
            {
                isJumping = true;
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                isJumping = false;
            }
        }
    }
    public void OnSprintInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            moveSpeed = 8;
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            moveSpeed = 3;
        }
    }    
    
    public void OnLoadInput(InputAction.CallbackContext context)
    {

    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f)+(Vector3.up*0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f)+(Vector3.up*0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f)+(Vector3.up*0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f)+(Vector3.up*0.01f), Vector3.down)
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if(Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }
    
    public void ToggleCurser(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}