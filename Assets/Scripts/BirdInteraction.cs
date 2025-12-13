using UnityEngine;
using UnityEngine.InputSystem; 

public class BirdInteraction : MonoBehaviour
{
    [SerializeField]
    public GameObject statusUI; 
    public Vector3 uiOffset = new Vector3(0.2f, 0, 0); 

    private InputAction clickAction; 

    [Header("Debug Settings")]
    public bool enableDebugLogs = true;

    void Awake()
    {
        LogDebug("=== BirdInteraction Script Initialized ===");

        if (statusUI != null)
        {
            statusUI.SetActive(false);
            LogDebug($"Status UI hidden at start (UI Object: {statusUI.name})");
        }
        else
        {
            LogDebug("⚠️ Status UI is NOT assigned! Assign it in the Inspector.", isError: true);
        }

        clickAction = new InputAction("Click", binding: "<Mouse>/leftButton");
        clickAction.AddBinding("<TouchScreen>/primaryTouch/tap");
        clickAction.performed += OnClickPerformed; 
        clickAction.Enable();
        LogDebug("Input System initialized (tap/click detection enabled)");
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        LogDebug("📱 Tap/Click detected!");

        if (Camera.main == null)
        {
            LogDebug("❌ Camera.main is NULL! No raycast possible.", isError: true);
            return;
        }

        Vector2 inputPos = Mouse.current.position.ReadValue();
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.isInProgress)
        {
            inputPos = Touchscreen.current.primaryTouch.position.ReadValue();
            LogDebug($"Touch position: X={inputPos.x}, Y={inputPos.y} (mobile)");
        }
        else
        {
            LogDebug($"Mouse click position: X={inputPos.x}, Y={inputPos.y} (PC)");
        }

        Ray ray = Camera.main.ScreenPointToRay(inputPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            LogDebug($"✅ Raycast hit object: {hit.collider.gameObject.name} (Tag: {hit.collider.gameObject.tag})");
            
            // Check if tapped object is the bird OR ITS CHILDREN
            if (hit.collider.gameObject == this.gameObject || hit.collider.gameObject.transform.IsChildOf(this.transform))
            {
                LogDebug($"🎉 Tapped the bird (or its part)! (Object: {hit.collider.gameObject.name})");
                ToggleStatusUI();
            }
            else
            {
                LogDebug($"❌ Tapped wrong object (not the bird) — hit {hit.collider.gameObject.name}");
            }
        }
        else
        {
            LogDebug("❌ Raycast hit NOTHING (no collider detected in scene)");
        }
    }

    void ToggleStatusUI()
    {
        if (statusUI == null)
        {
            LogDebug("❌ Cannot toggle UI — statusUI is NULL!", isError: true);
            return;
        }

        bool isActive = statusUI.activeSelf;
        statusUI.SetActive(!isActive);

        if (!isActive)
        {
            statusUI.transform.position = transform.position + uiOffset;
            statusUI.transform.LookAt(Camera.main.transform);
            statusUI.transform.Rotate(0, 180, 0);
            
            LogDebug($"📥 UI SHOWN — Position: {statusUI.transform.position} (Offset: {uiOffset})");
        }
        else
        {
            LogDebug($"📤 UI HIDDEN");
        }
    }

    void OnDestroy()
    {
        clickAction.Dispose(); 
        LogDebug("=== BirdInteraction Script Cleaned Up ===");
    }

    private void LogDebug(string message, bool isError = false)
    {
        if (!enableDebugLogs) return;

        if (isError)
        {
            Debug.LogError($"[BirdInteraction] {message}");
        }
        else
        {
            Debug.Log($"[BirdInteraction] {message}");
        }
    }

}