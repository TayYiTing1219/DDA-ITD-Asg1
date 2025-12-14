using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class BirdInteraction : MonoBehaviour
{
    [Header("Bird & UI Setup")]
    [SerializeField] private GameObject birdPrefab; 
    [SerializeField] private GameObject statusUI;   
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 0.5f, 0); 
    [SerializeField] private float tapDetectionRadius = 0.3f; 

    [Header("Mobile Fix (Critical!)")]
    [SerializeField] private Camera arCamera; 

    [Header("Detail Panels (Assign in Inspector)")]
    [SerializeField] private GameObject factsPanel; 
    [SerializeField] private GameObject habitatPanel; 
    [SerializeField] private GameObject dietPanel; 
    [SerializeField] private GameObject bodyPartPanel; 

    private InputAction tapAction;

    void Start()
    {
        if (statusUI != null)
            statusUI.SetActive(false);

        if (arCamera == null)
            arCamera = FindFirstObjectByType<ARCameraManager>()?.GetComponent<Camera>() ?? Camera.main;

        SetupTapInput();
        HideAllPanels(); // Hide panels at start
    }

    // Original: Tap input setup
    void SetupTapInput()
    {
        tapAction = new InputAction("Tap", InputActionType.Button);
        tapAction.AddBinding("<TouchScreen>/primaryTouch/tap");
        tapAction.AddBinding("<Mouse>/leftButton");
        tapAction.performed += OnBirdTapDetected;
        tapAction.Enable();
    }

    // Original: Bird tap detection
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

    // Original: Get input position
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

    // Original: Toggle Status UI
    void ToggleUI()
    {
        statusUI.SetActive(!statusUI.activeSelf);
        HideAllPanels();

        if (statusUI.activeSelf)
        {
            statusUI.transform.position = birdPrefab.transform.position + uiOffset;
            statusUI.transform.LookAt(arCamera.transform);
            statusUI.transform.Rotate(0, 180, 0);
        }
    }

    // === NEW: Assignable Panel Functions (Use in On Click Inspector) ===
    // Show panel (assign to panel buttons in Inspector)
    public void ShowFactsPanel() => ShowSinglePanel(factsPanel);
    public void ShowHabitatPanel() => ShowSinglePanel(habitatPanel);
    public void ShowDietPanel() => ShowSinglePanel(dietPanel);
    public void ShowBodyPartPanel() => ShowSinglePanel(bodyPartPanel);

    // Close panel (assign to each panel's Close button in Inspector)
    public void CloseFactsPanel() => factsPanel?.SetActive(false);
    public void CloseHabitatPanel() => habitatPanel?.SetActive(false);
    public void CloseDietPanel() => dietPanel?.SetActive(false);
    public void CloseBodyPartPanel() => bodyPartPanel?.SetActive(false);

    // Helper: Show one panel, hide others
    private void ShowSinglePanel(GameObject targetPanel)
    {
        HideAllPanels();
        targetPanel?.SetActive(true);
    }

    // Helper: Hide all panels
    private void HideAllPanels()
    {
        factsPanel?.SetActive(false);
        habitatPanel?.SetActive(false);
        dietPanel?.SetActive(false);
        bodyPartPanel?.SetActive(false);
    }

    // Original: Clean up input
    void OnDestroy()
    {
        tapAction?.Dispose();
    }
}