using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation; // Only add this line for AR Camera

public class BirdInteraction : MonoBehaviour
{
    [Header("Bird & UI Setup")]
    [SerializeField] private GameObject birdPrefab; 
    [SerializeField] private GameObject statusUI;   
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 0.5f, 0); 
    [SerializeField] private float tapDetectionRadius = 0.3f; 

    [Header("Mobile Fix (Critical!)")]
    [SerializeField] private Camera arCamera; // Assign AR Camera (XR Origin > Camera Offset > Main Camera)
    private InputAction tapAction;

    void Start()
    {
        if (statusUI != null)
            statusUI.SetActive(false);

        // Auto-find AR Camera if unassigned (fix Camera.main on mobile)
        if (arCamera == null)
            arCamera = FindFirstObjectByType<ARCameraManager>()?.GetComponent<Camera>() ?? Camera.main;

        SetupTapInput();
    }

    // Fixed: Use raw touch press (mobile-friendly, replace "tap" binding)
    void SetupTapInput()
    {
        tapAction = new InputAction("Tap", InputActionType.Button);
        // Mobile: Raw touch press (more reliable than "tap" on mobile)
        tapAction.AddBinding("<TouchScreen>/primaryTouch/press");
        // PC: Keep mouse click
        tapAction.AddBinding("<Mouse>/leftButton");
        tapAction.performed += OnBirdTapDetected;
        tapAction.Enable();
    }

    void OnBirdTapDetected(InputAction.CallbackContext context)
    {
        if (birdPrefab == null || !birdPrefab.activeInHierarchy || statusUI == null || arCamera == null)
        {
            Debug.LogWarning("Bird prefab/UI/AR Camera missing or bird inactive!");
            return;
        }

        Vector2 inputPos = GetInputPosition();

        // Fixed: Use arCamera instead of Camera.main (mobile AR fix)
        Vector3 birdScreenPos = arCamera.WorldToScreenPoint(birdPrefab.transform.position);
        birdScreenPos.z = 0;

        float tapToBirdDistance = Vector2.Distance(new Vector2(birdScreenPos.x, birdScreenPos.y), inputPos);
        // Fixed: Reduce scaling (0.5x) to make tap area mobile-friendly
        float scaledRadius = tapDetectionRadius * Screen.width * 0.5f; 

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

    // Fixed: Add null check for Touchscreen/Mouse (prevent mobile crashes)
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

    void ToggleUI()
    {
        statusUI.SetActive(!statusUI.activeSelf);

        if (statusUI.activeSelf)
        {
            statusUI.transform.position = birdPrefab.transform.position + uiOffset;
            // Fixed: Use arCamera instead of Camera.main (mobile AR fix)
            statusUI.transform.LookAt(arCamera.transform);
            statusUI.transform.Rotate(0, 180, 0);
        }
    }

    // Fixed: Disable input before disposing (prevent mobile memory leaks)
    void OnDestroy()
    {
        if (tapAction != null)
        {
            tapAction.Disable();
            tapAction.Dispose();
        }
    }
}