using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class FemaleBirdInteraction : MonoBehaviour
{
    [Header("Bird & Core UI Setup")]
    [SerializeField] private GameObject birdPrefab; 
    [SerializeField] private GameObject statusUI;   
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 0.5f, 0); 
    [SerializeField] private float tapDetectionRadius = 0.3f; 

    [Header("Habitat Setup")]
    [SerializeField] private GameObject riverBankPrefab; 
    [SerializeField] private Vector3 habitatOffset = new Vector3(2, 0, 0); 

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
    [SerializeField] private GameObject bodyPartPanel; 
    
    [Header("Main Panel Close Buttons")]
    [SerializeField] private Button factsCloseButton;   
    [SerializeField] private Button habitatCloseButton; 
    [SerializeField] private Button dietCloseButton;    
    [SerializeField] private Button bodyPartCloseButton;

    // --------------------------
    // Body Part Sub-Panels (Beak/Feathers/Wings)
    // --------------------------
    [Header("Body Part Sub-Buttons (Inside BodyPart Panel)")]
    [SerializeField] private Button beakButton;       
    [SerializeField] private Button feathersButton;   
    [SerializeField] private Button wingsButton;      

    [Header("Body Part Sub-Panels")]
    [SerializeField] private GameObject beakUI;       
    [SerializeField] private GameObject feathersUI;   
    [SerializeField] private GameObject wingsUI;      

    [Header("Body Part Sub-Panel Close Buttons")]
    [SerializeField] private Button beakCloseButton;     
    [SerializeField] private Button feathersCloseButton; 
    [SerializeField] private Button wingsCloseButton;    

    private InputAction tapAction;
    private GameObject spawnedRiverBank; 

    void Start()
    {
        // Critical: Validate ALL required fields first
        ValidateRequiredFields();

        // Hide core UI at start (with null check)
        if (statusUI != null)
            statusUI.SetActive(false);

        // Auto-find AR Camera (fallback to main camera if needed)
        if (arCamera == null)
        {
            arCamera = FindFirstObjectByType<ARCameraManager>()?.GetComponent<Camera>();
            if (arCamera == null) arCamera = Camera.main;
            Debug.Log(arCamera != null ? "AR Camera found" : "AR Camera NOT found - using main camera");
        }

        // Initialize core systems (only if valid)
        if (IsSetupValid())
        {
            SetupTapInput();
            HideAllMainPanels();
            HideAllBodyPartSubPanels(); 
            SetupMainPanelButtons();
            SetupMainPanelCloseButtons();
            SetupBodyPartSubPanelButtons();
            SetupBodyPartSubPanelCloseButtons();
        }
        else
        {
            Debug.LogError("Critical setup missing - script disabled!");
            this.enabled = false; // Disable script to prevent further errors
        }
    }

    // --------------------------
    // Critical Validation (Fix Null References)
    // --------------------------
    private void ValidateRequiredFields()
    {
        // Check core bird/UI
        if (birdPrefab == null) Debug.LogError("REQUIRED: Bird Prefab is not assigned!");
        if (statusUI == null) Debug.LogError("REQUIRED: Status UI is not assigned!");
        
        // Check main panels/buttons
        if (factsPanel == null) Debug.LogError("REQUIRED: Facts Panel is not assigned!");
        if (habitatPanel == null) Debug.LogError("REQUIRED: Habitat Panel is not assigned!");
        if (dietPanel == null) Debug.LogError("REQUIRED: Diet Panel is not assigned!");
        if (bodyPartPanel == null) Debug.LogError("REQUIRED: Body Part Panel is not assigned!");
        
        // Check habitat
        if (riverBankPrefab == null) Debug.LogWarning("Habitat: River Bank Prefab is not assigned!");
        
        // Check buttons (warn only - not critical)
        if (factsButton == null) Debug.LogWarning("Button: Facts Button is not assigned!");
        if (habitatButton == null) Debug.LogWarning("Button: Habitat Button is not assigned!");
        if (dietButton == null) Debug.LogWarning("Button: Diet Button is not assigned!");
        if (bodyPartButton == null) Debug.LogWarning("Button: Body Part Button is not assigned!");
    }

    private bool IsSetupValid()
    {
        // Minimum required to run
        return birdPrefab != null && statusUI != null && arCamera != null;
    }

    // --------------------------
    // Input Setup (Defensive)
    // --------------------------
    void SetupTapInput()
    {
        tapAction = new InputAction("Tap", InputActionType.Button);
        tapAction.AddBinding("<TouchScreen>/primaryTouch/press");
        tapAction.AddBinding("<Mouse>/leftButton");
        tapAction.performed += OnBirdTapDetected;
        tapAction.Enable();
        Debug.Log("Tap input setup complete");
    }

    // --------------------------
    // Main Panel Logic (Full Null Protection)
    // --------------------------
    void SetupMainPanelButtons()
    {
        // Clear existing listeners first (prevent duplicate calls)
        factsButton?.onClick.RemoveAllListeners();
        habitatButton?.onClick.RemoveAllListeners();
        dietButton?.onClick.RemoveAllListeners();
        bodyPartButton?.onClick.RemoveAllListeners();

        // Add listeners with null checks
        if (factsButton != null && factsPanel != null)
            factsButton.onClick.AddListener(() => ShowSpecificMainPanel(factsPanel));
        
        if (habitatButton != null)
            habitatButton.onClick.AddListener(ToggleHabitat);
        
        if (dietButton != null && dietPanel != null)
            dietButton.onClick.AddListener(() => ShowSpecificMainPanel(dietPanel));
        
        if (bodyPartButton != null && bodyPartPanel != null)
            bodyPartButton.onClick.AddListener(() => ShowSpecificMainPanel(bodyPartPanel));
    }

    void SetupMainPanelCloseButtons()
    {
        // Clear existing listeners
        factsCloseButton?.onClick.RemoveAllListeners();
        habitatCloseButton?.onClick.RemoveAllListeners();
        dietCloseButton?.onClick.RemoveAllListeners();
        bodyPartCloseButton?.onClick.RemoveAllListeners();

        // Add listeners with null checks
        if (factsCloseButton != null && factsPanel != null)
            factsCloseButton.onClick.AddListener(() => CloseSpecificMainPanel(factsPanel));
        
        if (habitatCloseButton != null && habitatPanel != null)
            habitatCloseButton.onClick.AddListener(() => 
            {
                CloseSpecificMainPanel(habitatPanel);
                HideRiverBank(); 
            });
        
        if (dietCloseButton != null && dietPanel != null)
            dietCloseButton.onClick.AddListener(() => CloseSpecificMainPanel(dietPanel));
        
        if (bodyPartCloseButton != null && bodyPartPanel != null)
            bodyPartCloseButton.onClick.AddListener(() => 
            {
                CloseSpecificMainPanel(bodyPartPanel);
                HideAllBodyPartSubPanels(); 
            });
    }

    // --------------------------
    // Habitat Logic (Full Defensive Checks)
    // --------------------------
    void ToggleHabitat()
    {
        if (birdPrefab == null)
        {
            Debug.LogWarning("Cannot toggle habitat: Bird Prefab is null!");
            return;
        }

        if (riverBankPrefab == null)
        {
            Debug.LogWarning("Cannot toggle habitat: River Bank Prefab is null!");
            return;
        }

        // Handle existing river bank
        if (spawnedRiverBank != null)
        {
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
        else
        {
            // Instantiate new river bank (with parent)
            spawnedRiverBank = Instantiate(riverBankPrefab);
            spawnedRiverBank.SetActive(false);
            spawnedRiverBank.transform.SetParent(birdPrefab.transform); // Parent immediately
            ShowRiverBank();
            ShowSpecificMainPanel(habitatPanel);
        }
    }

    void ShowRiverBank()
    {
        if (birdPrefab == null || !birdPrefab.activeInHierarchy)
        {
            Debug.LogError("Cannot show river bank: Bird Prefab is inactive/null!");
            return;
        }
        
        if (spawnedRiverBank == null)
        {
            Debug.LogError("Cannot show river bank: Spawned river bank is null!");
            return;
        }

        // Set position/scale/rotation relative to active bird
        spawnedRiverBank.transform.position = birdPrefab.transform.position + habitatOffset;
        spawnedRiverBank.transform.rotation = Quaternion.identity;
        spawnedRiverBank.transform.localScale = Vector3.one;
        spawnedRiverBank.SetActive(true);
        
        Debug.Log($"River bank activated at: {spawnedRiverBank.transform.position}");
    }

    void HideRiverBank()
    {
        if (spawnedRiverBank != null && spawnedRiverBank.activeSelf)
        {
            spawnedRiverBank.SetActive(false);
            Debug.Log("River bank hidden");
        }
    }

    // --------------------------
    // Panel Logic (Full Null Protection)
    // --------------------------
    void ShowSpecificMainPanel(GameObject targetPanel)
    {
        if (targetPanel == null)
        {
            Debug.LogWarning("Cannot show panel: Target panel is null!");
            return;
        }

        HideAllMainPanels();
        HideAllBodyPartSubPanels(); 
        targetPanel.SetActive(true);
        Debug.Log($"Showing panel: {targetPanel.name}");
    }

    void CloseSpecificMainPanel(GameObject targetPanel)
    {
        if (targetPanel == null)
        {
            Debug.LogWarning("Cannot close panel: Target panel is null!");
            return;
        }

        targetPanel.SetActive(false);
        Debug.Log($"Closed panel: {targetPanel.name}");
    }

    void HideAllMainPanels()
    {
        if (factsPanel != null) factsPanel.SetActive(false);
        if (habitatPanel != null) habitatPanel.SetActive(false);
        if (dietPanel != null) dietPanel.SetActive(false);
        if (bodyPartPanel != null) bodyPartPanel.SetActive(false);
    }

    // --------------------------
    // Body Part Sub-Panel Logic (Full Null Protection)
    // --------------------------
    void SetupBodyPartSubPanelButtons()
    {
        // Clear existing listeners
        beakButton?.onClick.RemoveAllListeners();
        feathersButton?.onClick.RemoveAllListeners();
        wingsButton?.onClick.RemoveAllListeners();

        // Add listeners with null checks
        if (beakButton != null && beakUI != null)
            beakButton.onClick.AddListener(() => ShowSpecificBodyPartSubPanel(beakUI));
        
        if (feathersButton != null && feathersUI != null)
            feathersButton.onClick.AddListener(() => ShowSpecificBodyPartSubPanel(feathersUI));
        
        if (wingsButton != null && wingsUI != null)
            wingsButton.onClick.AddListener(() => ShowSpecificBodyPartSubPanel(wingsUI));
    }

    void SetupBodyPartSubPanelCloseButtons()
    {
        // Clear existing listeners
        beakCloseButton?.onClick.RemoveAllListeners();
        feathersCloseButton?.onClick.RemoveAllListeners();
        wingsCloseButton?.onClick.RemoveAllListeners();

        // Add listeners with null checks
        if (beakCloseButton != null && beakUI != null)
            beakCloseButton.onClick.AddListener(() => CloseSpecificBodyPartSubPanel(beakUI));
        
        if (feathersCloseButton != null && feathersUI != null)
            feathersCloseButton.onClick.AddListener(() => CloseSpecificBodyPartSubPanel(feathersUI));
        
        if (wingsCloseButton != null && wingsUI != null)
            wingsCloseButton.onClick.AddListener(() => CloseSpecificBodyPartSubPanel(wingsUI));
    }

    void ShowSpecificBodyPartSubPanel(GameObject targetSubPanel)
    {
        if (targetSubPanel == null)
        {
            Debug.LogWarning("Cannot show sub-panel: Target sub-panel is null!");
            return;
        }

        HideAllBodyPartSubPanels();
        targetSubPanel.SetActive(true);
        Debug.Log($"Showing sub-panel: {targetSubPanel.name}");
    }

    void CloseSpecificBodyPartSubPanel(GameObject targetSubPanel)
    {
        if (targetSubPanel == null)
        {
            Debug.LogWarning("Cannot close sub-panel: Target sub-panel is null!");
            return;
        }

        targetSubPanel.SetActive(false);
        Debug.Log($"Closed sub-panel: {targetSubPanel.name}");
    }

    void HideAllBodyPartSubPanels()
    {
        if (beakUI != null) beakUI.SetActive(false);
        if (feathersUI != null) feathersUI.SetActive(false);
        if (wingsUI != null) wingsUI.SetActive(false);
    }

    // --------------------------
    // Tap Detection (Enhanced Validation)
    // --------------------------
    void OnBirdTapDetected(InputAction.CallbackContext context)
    {
        // Skip if context is invalid
        if (!context.performed) return;

        // Full validation before processing tap
        if (birdPrefab == null || !birdPrefab.activeInHierarchy)
        {
            Debug.LogWarning("Tap ignored: Bird Prefab is null/inactive!");
            return;
        }
        
        if (statusUI == null)
        {
            Debug.LogWarning("Tap ignored: Status UI is null!");
            return;
        }
        
        if (arCamera == null)
        {
            Debug.LogWarning("Tap ignored: AR Camera is null!");
            return;
        }

        // Calculate tap position/distance
        Vector2 inputPos = GetInputPosition();
        Vector3 birdScreenPos = arCamera.WorldToScreenPoint(birdPrefab.transform.position);
        birdScreenPos.z = 0;

        float tapToBirdDistance = Vector2.Distance(new Vector2(birdScreenPos.x, birdScreenPos.y), inputPos);
        float scaledRadius = tapDetectionRadius * Screen.width;

        // Only process valid taps
        if (tapToBirdDistance < scaledRadius)
        {
            ToggleUI();
            Debug.Log($"Valid tap on {birdPrefab.name} (distance: {tapToBirdDistance:F2} / {scaledRadius:F2})");
        }
        else
        {
            Debug.Log($"Tap outside {birdPrefab.name} (distance: {tapToBirdDistance:F2} > {scaledRadius:F2})");
        }
    }

    // --------------------------
    // Input Position (Defensive)
    // --------------------------
    Vector2 GetInputPosition()
    {
        // Prioritize touch input (mobile)
        if (Touchscreen.current != null && Touchscreen.current.enabled && Touchscreen.current.primaryTouch.isInProgress)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        // Fallback to mouse (editor/PC)
        else if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        
        // Default to zero if no input
        return Vector2.zero;
    }

    // --------------------------
    // UI Toggle (Full Protection)
    // --------------------------
    void ToggleUI()
    {
        if (statusUI == null)
        {
            Debug.LogError("Cannot toggle UI: Status UI is null!");
            return;
        }
        
        if (birdPrefab == null || !birdPrefab.activeInHierarchy)
        {
            Debug.LogError("Cannot toggle UI: Bird Prefab is inactive/null!");
            return;
        }

        // Toggle UI state
        bool newState = !statusUI.activeSelf;
        statusUI.SetActive(newState);
        
        // Hide all content if UI is closed
        if (!newState)
        {
            HideAllMainPanels();
            HideAllBodyPartSubPanels();
            HideRiverBank();
            Debug.Log("Status UI closed - all content hidden");
        }
        else
        {
            // Position UI correctly (with rotation fix)
            statusUI.transform.position = birdPrefab.transform.position + uiOffset;
            statusUI.transform.LookAt(arCamera.transform);
            statusUI.transform.rotation = Quaternion.Euler(0, statusUI.transform.rotation.eulerAngles.y + 180, 0);
            Debug.Log($"Status UI opened for {birdPrefab.name} at {statusUI.transform.position}");
        }
    }

    // --------------------------
    // Cleanup (Safe Disposal)
    // --------------------------
    void OnDestroy()
    {
        // Safely dispose input action
        if (tapAction != null)
        {
            tapAction.Disable();
            tapAction.Dispose();
        }

        // Safely destroy river bank
        if (spawnedRiverBank != null)
        {
            Destroy(spawnedRiverBank);
            spawnedRiverBank = null;
        }
    }

    // Optional: Validate fields in Edit Mode (catch errors before play)
    void OnValidate()
    {
        if (birdPrefab == null) Debug.LogError("Bird Prefab is required!", this);
        if (statusUI == null) Debug.LogError("Status UI is required!", this);
    }
}