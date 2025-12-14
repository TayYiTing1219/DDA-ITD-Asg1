using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class BirdInteraction : MonoBehaviour
{
    [Header("Bird & Core UI Setup")]
    [SerializeField] private GameObject birdPrefab; 
    [SerializeField] private GameObject statusUI;   
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 0.5f, 0); 
    [SerializeField] private float tapDetectionRadius = 0.3f; 

    [Header("Habitat Setup")]
    [SerializeField] private GameObject riverBankPrefab; // Assign your animated river bank prefab
    [SerializeField] private Vector3 habitatOffset = new Vector3(2, 0, 0); // Position relative to bird

    [Header("Mobile Fix (Critical!)")]
    [SerializeField] private Camera arCamera; 

    // --------------------------
    // Main Status UI Buttons/Panels
    // --------------------------
    [Header("Main Detail Panel Buttons (Status UI)")]
    [SerializeField] private Button factsButton; 
    [SerializeField] private Button habitatButton; 
    [SerializeField] private Button dietButton; 
    [SerializeField] private Button bodyPartButton; 

    [Header("Main Detail Panels")]
    [SerializeField] private GameObject factsPanel; 
    [SerializeField] private GameObject habitatPanel; 
    [SerializeField] private GameObject dietPanel; 
    [SerializeField] private GameObject bodyPartPanel; // Parent Body Parts panel
    
    [Header("Main Panel Close Buttons")]
    [SerializeField] private Button factsCloseButton;   
    [SerializeField] private Button habitatCloseButton; 
    [SerializeField] private Button dietCloseButton;    
    [SerializeField] private Button bodyPartCloseButton;// Close parent Body Parts panel

    // --------------------------
    // Body Part Sub-Panels (Beak/Feathers/Wings)
    // --------------------------
    [Header("Body Part Sub-Buttons (Inside BodyPart Panel)")]
    [SerializeField] private Button beakButton;       // Assign Beak button (in BodyPart panel)
    [SerializeField] private Button feathersButton;   // Assign Feathers button (in BodyPart panel)
    [SerializeField] private Button wingsButton;      // Assign Wings button (in BodyPart panel)

    [Header("Body Part Sub-Panels")]
    [SerializeField] private GameObject beakUI;       // Assign Beak detail UI
    [SerializeField] private GameObject feathersUI;   // Assign Feathers detail UI
    [SerializeField] private GameObject wingsUI;      // Assign Wings detail UI

    [Header("Body Part Sub-Panel Close Buttons")]
    [SerializeField] private Button beakCloseButton;     // Assign Beak UI close button
    [SerializeField] private Button feathersCloseButton; // Assign Feathers UI close button
    [SerializeField] private Button wingsCloseButton;    // Assign Wings UI close button

    private InputAction tapAction;
    private GameObject spawnedRiverBank; // Track the instantiated river bank

    void Start()
    {
        // Hide core UI at start
        if (statusUI != null)
            statusUI.SetActive(false);

        // Auto-find AR Camera
        if (arCamera == null)
            arCamera = FindFirstObjectByType<ARCameraManager>()?.GetComponent<Camera>() ?? Camera.main;

        // Initialize river bank (hidden initially)
        if (riverBankPrefab != null)
        {
            spawnedRiverBank = Instantiate(riverBankPrefab);
            spawnedRiverBank.SetActive(false);
            spawnedRiverBank.transform.SetParent(birdPrefab.transform); // Parent to bird (critical!)
        }

        // Initialize core systems
        SetupTapInput();
        HideAllMainPanels();
        HideAllBodyPartSubPanels(); // Hide sub-panels on start

        // Link main panel buttons/close buttons
        SetupMainPanelButtons();
        SetupMainPanelCloseButtons();

        // Link body part sub-panel buttons/close buttons
        SetupBodyPartSubPanelButtons();
        SetupBodyPartSubPanelCloseButtons();
    }

    // Original: Tap input setup (unchanged)
    void SetupTapInput()
    {
        tapAction = new InputAction("Tap", InputActionType.Button);
        tapAction.AddBinding("<TouchScreen>/primaryTouch/tap");
        tapAction.AddBinding("<Mouse>/leftButton");
        tapAction.performed += OnBirdTapDetected;
        tapAction.Enable();
    }

    // --------------------------
    // Main Panel Logic (Facts/Habitat/Diet/BodyPart)
    // --------------------------
    void SetupMainPanelButtons()
    {
        factsButton?.onClick.AddListener(() => ShowSpecificMainPanel(factsPanel));
        habitatButton?.onClick.AddListener(ToggleHabitat); // Changed to trigger habitat
        dietButton?.onClick.AddListener(() => ShowSpecificMainPanel(dietPanel));
        bodyPartButton?.onClick.AddListener(() => ShowSpecificMainPanel(bodyPartPanel));
    }

    void SetupMainPanelCloseButtons()
    {
        factsCloseButton?.onClick.AddListener(() => CloseSpecificMainPanel(factsPanel));
        habitatCloseButton?.onClick.AddListener(() => 
        {
            CloseSpecificMainPanel(habitatPanel);
            HideRiverBank(); // Hide river bank when closing habitat panel
        });
        dietCloseButton?.onClick.AddListener(() => CloseSpecificMainPanel(dietPanel));
        bodyPartCloseButton?.onClick.AddListener(() => 
        {
            CloseSpecificMainPanel(bodyPartPanel);
            HideAllBodyPartSubPanels(); // Hide sub-panels when closing parent BodyPart panel
        });
    }

    // New method to handle habitat toggle
    void ToggleHabitat()
    {
        if (spawnedRiverBank == null)
        {
            Debug.LogWarning("River bank prefab not assigned!");
            return;
        }

        if (spawnedRiverBank.activeSelf)
        {
            HideRiverBank();
            CloseSpecificMainPanel(habitatPanel);
        }
        else
        {
            ShowRiverBank();
            ShowSpecificMainPanel(habitatPanel);
        }
    }

    void ShowRiverBank()
    {
        if (birdPrefab == null || spawnedRiverBank == null) 
        {
            Debug.LogError("Bird/River Bank prefab missing!");
            return;
        }

        // Log positions for debugging
        Debug.Log($"Bird Position: {birdPrefab.transform.position}");
        Debug.Log($"River Bank Position: {birdPrefab.transform.position + habitatOffset}");
        
        // Force-set position/scale/rotation
        spawnedRiverBank.transform.position = birdPrefab.transform.position + habitatOffset;
        spawnedRiverBank.transform.rotation = Quaternion.identity; // Reset rotation (no weird angles)
        spawnedRiverBank.transform.localScale = Vector3.one; // Force 1:1 scale
        spawnedRiverBank.SetActive(true);
        
        Debug.Log("River bank ACTIVATED – check position/scale in Scene view!");
    }

    void HideRiverBank()
    {
        if (spawnedRiverBank != null && spawnedRiverBank.activeSelf)
        {
            spawnedRiverBank.SetActive(false);
            Debug.Log("River bank hidden");
        }
    }

    void ShowSpecificMainPanel(GameObject targetPanel)
    {
        if (targetPanel == null)
        {
            Debug.LogWarning("Main panel not assigned!");
            return;
        }

        HideAllMainPanels();
        HideAllBodyPartSubPanels(); // Hide sub-panels when switching main panels
        targetPanel.SetActive(true);
        Debug.Log($"Showing main panel: {targetPanel.name}");
    }

    void CloseSpecificMainPanel(GameObject targetPanel)
    {
        if (targetPanel == null)
        {
            Debug.LogWarning("Main panel not assigned!");
            return;
        }

        targetPanel.SetActive(false);
        Debug.Log($"Closed main panel: {targetPanel.name}");
    }

    void HideAllMainPanels()
    {
        factsPanel?.SetActive(false);
        habitatPanel?.SetActive(false);
        dietPanel?.SetActive(false);
        bodyPartPanel?.SetActive(false);
    }

    // --------------------------
    // Body Part Sub-Panel Logic (Beak/Feathers/Wings)
    // --------------------------
    void SetupBodyPartSubPanelButtons()
    {
        // Link sub-buttons to show their specific UI (hide other sub-panels first)
        beakButton?.onClick.AddListener(() => ShowSpecificBodyPartSubPanel(beakUI));
        feathersButton?.onClick.AddListener(() => ShowSpecificBodyPartSubPanel(feathersUI));
        wingsButton?.onClick.AddListener(() => ShowSpecificBodyPartSubPanel(wingsUI));
    }

    void SetupBodyPartSubPanelCloseButtons()
    {
        // Unique close buttons for each sub-panel
        beakCloseButton?.onClick.AddListener(() => CloseSpecificBodyPartSubPanel(beakUI));
        feathersCloseButton?.onClick.AddListener(() => CloseSpecificBodyPartSubPanel(feathersUI));
        wingsCloseButton?.onClick.AddListener(() => CloseSpecificBodyPartSubPanel(wingsUI));
    }

    void ShowSpecificBodyPartSubPanel(GameObject targetSubPanel)
    {
        if (targetSubPanel == null)
        {
            Debug.LogWarning("Body part sub-panel not assigned!");
            return;
        }

        HideAllBodyPartSubPanels();
        targetSubPanel.SetActive(true);
        Debug.Log($"Showing body part sub-panel: {targetSubPanel.name}");
    }

    void CloseSpecificBodyPartSubPanel(GameObject targetSubPanel)
    {
        if (targetSubPanel == null)
        {
            Debug.LogWarning("Body part sub-panel not assigned!");
            return;
        }

        targetSubPanel.SetActive(false);
        Debug.Log($"Closed body part sub-panel: {targetSubPanel.name}");
    }

    void HideAllBodyPartSubPanels()
    {
        beakUI?.SetActive(false);
        feathersUI?.SetActive(false);
        wingsUI?.SetActive(false);
    }

    // --------------------------
    // Original Bird Tap/UI Logic (unchanged)
    // --------------------------
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

    void ToggleUI()
    {
        statusUI.SetActive(!statusUI.activeSelf);
        
        // Hide ALL panels (main + sub) and river bank when Status UI is closed
        if (!statusUI.activeSelf)
        {
            HideAllMainPanels();
            HideAllBodyPartSubPanels();
            HideRiverBank();
        }

        if (statusUI.activeSelf)
        {
            statusUI.transform.position = birdPrefab.transform.position + uiOffset;
            statusUI.transform.LookAt(arCamera.transform);
            statusUI.transform.Rotate(0, 180, 0);
        }
    }

    void OnDestroy()
    {
        tapAction?.Dispose();
        if (spawnedRiverBank != null)
            Destroy(spawnedRiverBank);
    }
}