using UnityEngine;
using UnityEngine.InputSystem;

public class BirdInteraction : MonoBehaviour
{
    [Header("Bird & UI Setup")]
    [SerializeField] private GameObject birdPrefab; // Assign your SPAWNED bird prefab (from ImageTracker)
    [SerializeField] private GameObject statusUI;   // Your Status UI (Canvas)
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 0.5f, 0); // UI above bird
    [SerializeField] private float tapDetectionRadius = 0.3f; // Tap area around bird (adjust)

    private InputAction tapAction;

    void Start()
    {
        // Hide UI at start
        if (statusUI != null)
            statusUI.SetActive(false);

        // Set up tap/click input (mobile + PC)
        SetupTapInput();
    }

    // Set up cross-platform tap/click (no colliders needed)
    void SetupTapInput()
    {
        tapAction = new InputAction("Tap", InputActionType.Button);
        // Mobile touch tap
        tapAction.AddBinding("<TouchScreen>/primaryTouch/tap");
        // PC mouse click (testing)
        tapAction.AddBinding("<Mouse>/leftButton");
        tapAction.performed += OnBirdTapDetected;
        tapAction.Enable();
    }

    // Detect taps on the BIRD PREFAB (not the AR image)
    void OnBirdTapDetected(InputAction.CallbackContext context)
    {
        // Exit if bird prefab is missing/inactive
        if (birdPrefab == null || !birdPrefab.activeInHierarchy || statusUI == null)
        {
            Debug.LogWarning("Bird prefab/UI not assigned or bird is inactive!");
            return;
        }

        // Step 1: Get user's tap/click position (screen space)
        Vector2 inputPos = GetInputPosition();

        // Step 2: Convert bird's 3D world position to 2D screen position
        Vector3 birdScreenPos = Camera.main.WorldToScreenPoint(birdPrefab.transform.position);
        birdScreenPos.z = 0; // Ignore depth for 2D distance check

        // Step 3: Check if tap is within the bird's detection area
        float tapToBirdDistance = Vector2.Distance(
            new Vector2(birdScreenPos.x, birdScreenPos.y), 
            inputPos
        );

        // Scale radius with screen size (works on all devices)
        float scaledRadius = tapDetectionRadius * Screen.width;

        if (tapToBirdDistance < scaledRadius)
        {
            // Tap hit the bird → show/hide UI
            ToggleUI();
            Debug.Log($"Tapped the bird! Distance: {tapToBirdDistance} (radius: {scaledRadius})");
        }
        else
        {
            Debug.Log($"Tap outside bird (distance: {tapToBirdDistance} > {scaledRadius})");
        }
    }

    // Get tap/click position (cross-platform)
    Vector2 GetInputPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.isInProgress)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else
        {
            return Mouse.current.position.ReadValue();
        }
    }

    // Show/hide UI above the bird
    void ToggleUI()
    {
        statusUI.SetActive(!statusUI.activeSelf);

        if (statusUI.activeSelf)
        {
            // Position UI relative to the bird prefab
            statusUI.transform.position = birdPrefab.transform.position + uiOffset;
            // Rotate UI to face the camera (readable)
            statusUI.transform.LookAt(Camera.main.transform);
            statusUI.transform.Rotate(0, 180, 0); // Fix flipped text
        }
    }

    // Clean up input to prevent memory leaks
    void OnDestroy()
    {
        tapAction?.Dispose();
    }
}