using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI; // Add UI namespace (for buttons/panels)

public class BirdInteraction : MonoBehaviour
{
    [Header("Bird & UI Setup")]
    [SerializeField] private GameObject birdPrefab; 
    [SerializeField] private GameObject statusUI;   
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 0.5f, 0); 
    [SerializeField] private float tapDetectionRadius = 0.3f; 

    [Header("Mobile Fix (Critical!)")]
    [SerializeField] private Camera arCamera; 

    [Header("Detail Panel Buttons (Status UI Buttons)")]
    [SerializeField] private Button factsButton; // Assign "Facts" button in Status UI
    [SerializeField] private Button habitatButton; // Assign "Habitat" button in Status UI
    [SerializeField] private Button dietButton; // Assign "Diet" button in Status UI
    [SerializeField] private Button bodyPartButton; // Assign "Body Part" button in Status UI

    [Header("Detail Panels")]
    [SerializeField] private GameObject factsPanel; // Assign Facts detail panel
    [SerializeField] private GameObject habitatPanel; // Assign Habitat detail panel
    [SerializeField] private GameObject dietPanel; // Assign Diet detail panel
    [SerializeField] private GameObject bodyPartPanel; // Assign Body Part detail panel
    
    [Header("Unique Close Buttons (Per Panel)")]
    [SerializeField] private Button factsCloseButton;   // Assign Facts panel's close button
    [SerializeField] private Button habitatCloseButton; // Assign Habitat panel's close button
    [SerializeField] private Button dietCloseButton;    // Assign Diet panel's close button
    [SerializeField] private Button bodyPartCloseButton;// Assign Body Part panel's close button

    private InputAction tapAction;

    void Start()
    {
        // Hide UI at start
        if (statusUI != null)
            statusUI.SetActive(false);

        // Auto-find AR Camera if unassigned
        if (arCamera == null)
            arCamera = FindFirstObjectByType<ARCameraManager>()?.GetComponent<Camera>() ?? Camera.main;

        // Set up core systems
        SetupTapInput();
        HideAllPanels(); // Hide panels on start
        SetupDetailPanelButtons(); // Link show buttons
        SetupCloseButtons(); // Link unique close buttons
    }

    // Original: Set up tap input (unchanged)
    void SetupTapInput()
    {
        tapAction = new InputAction("Tap", InputActionType.Button);
        tapAction.AddBinding("<TouchScreen>/primaryTouch/tap");
        tapAction.AddBinding("<Mouse>/leftButton");
        tapAction.performed += OnBirdTapDetected;
        tapAction.Enable();
    }

    // Fixed: Link Status UI buttons to SHOW panel functions
    void SetupDetailPanelButtons()
    {
        // Link each Status UI button to show its panel (hide others first)
        factsButton?.onClick.AddListener(() => ShowSpecificPanel(factsPanel));
        habitatButton?.onClick.AddListener(() => ShowSpecificPanel(habitatPanel));
        dietButton?.onClick.AddListener(() => ShowSpecificPanel(dietPanel));
        bodyPartButton?.onClick.AddListener(() => ShowSpecificPanel(bodyPartPanel));
    }

    // Fixed: Link unique close buttons to CLOSE their panel
    void SetupCloseButtons()
    {
        // Each close button only closes its own panel (no global close unless needed)
        factsCloseButton?.onClick.AddListener(() => CloseSpecificPanel(factsPanel));
        habitatCloseButton?.onClick.AddListener(() => CloseSpecificPanel(habitatPanel));
        dietCloseButton?.onClick.AddListener(() => CloseSpecificPanel(dietPanel));
        bodyPartCloseButton?.onClick.AddListener(() => CloseSpecificPanel(bodyPartPanel));
    }

    // Fixed: Show ONE panel (hide others first)
    void ShowSpecificPanel(GameObject targetPanel)
    {
        if (targetPanel == null)
        {
            Debug.LogWarning("Target panel is not assigned!");
            return;
        }

        HideAllPanels(); // Hide other panels first
        targetPanel.SetActive(true);
        Debug.Log($"Showing panel: {targetPanel.name}");
    }

    // New: Close ONLY the target panel (unique close logic)
    void CloseSpecificPanel(GameObject targetPanel)
    {
        if (targetPanel == null)
        {
            Debug.LogWarning("Target panel is not assigned!");
            return;
        }

        targetPanel.SetActive(false);
        Debug.Log($"Closed panel: {targetPanel.name}");
    }

    // Fixed: Hide ALL panels (helper function)
    void HideAllPanels()
    {
        if (factsPanel != null) factsPanel.SetActive(false);
        if (habitatPanel != null) habitatPanel.SetActive(false);
        if (dietPanel != null) dietPanel.SetActive(false);
        if (bodyPartPanel != null) bodyPartPanel.SetActive(false);
    }

    // Original: Detect bird taps (unchanged)
    void OnBirdTapDetected(InputAction.CallbackContext context)
    {
        if (birdPrefab == null || !birdPrefab.activeInHierarchy || statusUI == null || arCamera == null)
        {
            Debug.LogWarning("Bird prefab/UI not assigned or bird is inactive!");
            return;
        }

        Vector2 inputPos = GetInputPosition();

        Vector3 birdScreenPos = arCamera.WorldToScreenPoint(birdPrefab.transform.position);
        birdScreenPos.z = 0;

        float tapToBirdDistance = Vector2.Distance(new Vector2(birdScreenPos.x, birdScreenPos.y), inputPos);
        float scaledRadius = tapDetectionRadius * Screen.width;

        if (tapToBirdDistance < scaledRadius)
        {
            ToggleUI();
            Debug.Log($"Tapped the bird! Distance: {tapToBirdDistance} (radius: {scaledRadius})");
        }
        else
        {
            Debug.Log($"Tap outside bird (distance: {tapToBirdDistance} > {scaledRadius})");
        }
    }

    // Original: Get input position (unchanged)
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

    // Original: Toggle Status UI (updated to hide panels)
    void ToggleUI()
    {
        statusUI.SetActive(!statusUI.activeSelf);
        
        // Hide all panels when Status UI is closed
        if (!statusUI.activeSelf)
        {
            HideAllPanels();
        }

        if (statusUI.activeSelf)
        {
            statusUI.transform.position = birdPrefab.transform.position + uiOffset;
            statusUI.transform.LookAt(arCamera.transform);
            statusUI.transform.Rotate(0, 180, 0);
        }
    }

    // Original: Clean up input (unchanged)
    void OnDestroy()
    {
        tapAction?.Dispose();
    }
}