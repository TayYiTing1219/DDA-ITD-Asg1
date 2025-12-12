using UnityEngine;
using UnityEngine.UI;

public class BirdInteraction : MonoBehaviour
{
    // Assign your UI panel in the Unity Inspector
    public GameObject birdDetailUI; 
    // Assign the close button from the UI panel
    public Button closeButton; 

    void Start()
    {
        // Add a click event to the close button
        closeButton.onClick.AddListener(HideUI);
        // Make sure UI is hidden at the start
        birdDetailUI.SetActive(false);
    }

    void Update()
    {
        // Detect left mouse click (or mobile touch)
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the camera to the click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if the clicked object is the bird
                if (hit.collider.gameObject == this.gameObject)
                {
                    ShowUI();
                }
            }
        }
    }

    // Show the specific UI
    void ShowUI()
    {
        birdDetailUI.SetActive(true);
        // Optional: Position the UI near the bird (for AR immersion)
        Vector3 birdScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        birdDetailUI.GetComponent<RectTransform>().position = birdScreenPos + new Vector3(0, 50, 0); // Offset to avoid overlapping the bird
    }

    // Hide the UI when clicking "Close"
    void HideUI()
    {
        birdDetailUI.SetActive(false);
    }
}