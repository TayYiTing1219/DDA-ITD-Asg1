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
    // Main Status UI Buttons/Panels (unchanged)
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
    // Body Part Sub-Panels (Beak/Feathers/Wings) (unchanged)
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
    private GameObject spawnedRiverBank; // Track the instantiated river bank

    void Start()
    {
        // Hide core UI at start
        if (statusUI != null)
            statusUI.SetActive(false);

        // Auto-find AR Camera
        if (arCamera == null)
            arCamera = FindFirstObjectByType<ARCameraManager>()?.GetComponent<Camera>() ?? Camera.main;

        // DO NOT instantiate river bank at Start() (fix #4)
        // spawnedRiverBank = Instantiate(riverBankPrefab); // REMOVED
        
        // Initialize core systems
        SetupTapInput();
        HideAllMainPanels();
        HideAllBodyPartSubPanels(); 

        // Link buttons (unchanged)
        SetupMainPanelButtons();
        SetupMainPanelCloseButtons();
        SetupBodyPartSubPanelButtons();
        SetupBodyPartSubPanelCloseButtons();
    }

    // Original: Tap input setup (unchanged)
    void SetupTapInput()
    {
        tapAction = new InputAction("Tap", InputActionType.Button);
        tapAction.AddBinding("<TouchScreen>/primaryTouch/press");
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
            HideRiverBank(); 
        });
        dietCloseButton?.onClick.AddListener(() => CloseSpecificMainPanel(dietPanel));
        bodyPartCloseButton?.onClick.AddListener(() => 
        {
            CloseSpecificMainPanel(bodyPartPanel);
            HideAllBodyPartSubPanels(); 
        });
    }

    // Fixed: ToggleHabitat (dynamic river bank instantiation)
    void ToggleHabitat()
    {
        // Fix #1/#4: Recheck birdPrefab and riverBankPrefab EVERY time habitat is pressed
        if (birdPrefab == null)
        {
            Debug.LogWarning("Bird prefab not assigned!");
            return;
        }
        if (riverBankPrefab == null)
        {
            Debug.LogWarning("River bank prefab not assigned!");
            return;
        }

        // If river bank exists: toggle it
        if (spawnedRiverBank != null)
        {
            if (spawnedRiverBank.activeSelf)
            {
                HideRiverBank();
                CloseSpecificMainPanel(habitatPanel);
            }
            else
            {
                ShowRiverBank(); // Re-position existing river bank
                ShowSpecificMainPanel(habitatPanel);
            }
        }
        else
        {
            // Instantiate river bank ONLY when needed (fix #4)
            spawnedRiverBank = Instantiate(riverBankPrefab);
            spawnedRiverBank.SetActive(false);
            ShowRiverBank(); // Show it immediately
            ShowSpecificMainPanel(habitatPanel);
        }
    }

    // Fixed: ShowRiverBank (dynamic parenting + recheck birdPrefab)
    void ShowRiverBank()
    {
        // Fix #2: Recheck birdPrefab is valid (active + not null)
        if (birdPrefab == null || !birdPrefab.activeInHierarchy) 
        {
            Debug.LogError("Bird prefab is null or inactive!");
            return;
        }
        if (spawnedRiverBank == null) 
        {
            Debug.LogError("River Bank prefab not instantiated!");
            return;
        }

        // Log positions for debugging
        Debug.Log($"Bird Position: {birdPrefab.transform.position}");
        Debug.Log($"River Bank Position: {birdPrefab.transform.position + habitatOffset}");
        
        // Fix #1: Reparent river bank to CURRENT birdPrefab (male/female)
        spawnedRiverBank.transform.SetParent(birdPrefab.transform); 
        
        // Force-set position/scale/rotation
        spawnedRiverBank.transform.position = birdPrefab.transform.position + habitatOffset;
        spawnedRiverBank.transform.rotation = Quaternion.identity; 
        spawnedRiverBank.transform.localScale = Vector3.one; 
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

    // Remaining panel logic (unchanged)
    void ShowSpecificMainPanel(GameObject targetPanel)
    {
        if (targetPanel == null)
        {
            Debug.LogWarning("Main panel not assigned!");
            return;
        }

        HideAllMainPanels();
        HideAllBodyPartSubPanels(); 
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
    // Body Part Sub-Panel Logic (unchanged)
    // --------------------------
    void SetupBodyPartSubPanelButtons()
    {
        beakButton?.onClick.AddListener(() => ShowSpecificBodyPartSubPanel(beakUI));
        feathersButton?.onClick.AddListener(() => ShowSpecificBodyPartSubPanel(feathersUI));
        wingsButton?.onClick.AddListener(() => ShowSpecificBodyPartSubPanel(wingsUI));
    }

    void SetupBodyPartSubPanelCloseButtons()
    {
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
    // Fixed: Tap Detection (dynamic bird check)
    // --------------------------
    void OnBirdTapDetected(InputAction.CallbackContext context)
    {
        // Fix #2: Recheck birdPrefab is active/valid EVERY tap
        if (birdPrefab == null || !birdPrefab.activeInHierarchy)
        {
            Debug.LogWarning("Bird prefab is null or inactive (female/male)!");
            return;
        }
        if (statusUI == null || arCamera == null)
        {
            Debug.LogWarning("Status UI/AR Camera not assigned!");
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

    // Fixed: Input position (unchanged)
    Vector2 GetInputPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.enabled)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        return Vector2.zero;
    }

    // Fixed: ToggleUI (dynamic bird position)
    void ToggleUI()
    {
        // Fix #2: Recheck birdPrefab before positioning UI
        if (birdPrefab == null || !birdPrefab.activeInHierarchy)
        {
            Debug.LogWarning("Cannot position UI: bird prefab is inactive/null!");
            return;
        }

        statusUI.SetActive(!statusUI.activeSelf);
        
        if (!statusUI.activeSelf)
        {
            HideAllMainPanels();
            HideAllBodyPartSubPanels();
            HideRiverBank();
        }
        else
        {
            // Position UI relative to CURRENT birdPrefab (male/female)
            statusUI.transform.position = birdPrefab.transform.position + uiOffset;
            statusUI.transform.LookAt(arCamera.transform);
            statusUI.transform.Rotate(0, 180, 0);
        }
    }

    // Cleanup (unchanged)
    void OnDestroy()
    {
        tapAction?.Dispose();
        if (spawnedRiverBank != null)
            Destroy(spawnedRiverBank);
    }
}