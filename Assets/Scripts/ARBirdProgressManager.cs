using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARBirdProgressManager : MonoBehaviour
{
    // --------------------------
    // Core Configuration
    // --------------------------
    [Header("Objective Settings")]
    [SerializeField] private int totalBirdSpecies = 2; // Total birds to scan (set to 2 for your project)

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private GameObject taskFeedbackPopup;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject completionScreen;
    [SerializeField] private Button returnToStartButton;

    // --------------------------
    // Progress Tracking
    // --------------------------
    private int scannedBirdsCount = 0;
    private string[] scannedBirdNames; // Dynamic array (matches totalBirdSpecies)
    private bool isExperienceCompleted = false;

    // --------------------------
    // AR Reference
    // --------------------------
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    void Start()
    {
        // Critical: Initialize dynamic array (fixes index errors)
        scannedBirdNames = new string[totalBirdSpecies];

        // Initialize UI
        InitializeProgressUI();
        
        // Set up return button
        returnToStartButton.onClick.AddListener(ReturnToStart);
        
        // Attach AR image scan listener (with null check)
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
            trackedImageManager.enabled = true;
            Debug.Log("ARTrackedImageManager assigned and active!");
        }
        else
        {
            Debug.LogWarning("ARTrackedImageManager is NOT assigned! Assign it in the Inspector.", this);
        }

        // Hide feedback/completion screens by default
        taskFeedbackPopup.SetActive(false);
        completionScreen.SetActive(false);
    }

    // --------------------------
    // Clean Up Event Listener (Prevent Memory Leaks)
    // --------------------------
    void OnDestroy()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
        }
    }

    // --------------------------
    // Initialize Progress UI
    // --------------------------
    private void InitializeProgressUI()
    {
        // Set objective text (only scanning)
        objectiveText.text = $"Scan all {totalBirdSpecies} bird species!";

        // Reset progress bar (max = total birds to scan)
        progressBar.minValue = 0;
        progressBar.maxValue = totalBirdSpecies;
        progressBar.value = 0;
        UpdateProgressText();

        // Debug: Confirm UI init
        Debug.Log($"Progress UI Initialized | Max Progress: {progressBar.maxValue}");
    }

    // --------------------------
    // Track AR Image Scans (Auto-Detect Scanned Birds)
    // --------------------------
    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // Process NEWLY added tracked images (only scan once per bird)
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            // Skip invalid images
            if (trackedImage.referenceImage == null || string.IsNullOrEmpty(trackedImage.referenceImage.name))
            {
                Debug.LogWarning("Skipped invalid tracked image (null name/reference)");
                continue;
            }

            string birdName = trackedImage.referenceImage.name;
            Debug.Log($"Detected AR Image: {birdName}");

            // Only process if bird hasn't been scanned yet
            if (!IsBirdAlreadyScanned(birdName))
            {
                // Find first empty slot in the array (prevents index errors)
                int emptySlot = System.Array.IndexOf(scannedBirdNames, null);
                if (emptySlot != -1)
                {
                    // Mark bird as scanned
                    scannedBirdNames[emptySlot] = birdName;
                    scannedBirdsCount = Mathf.Clamp(scannedBirdsCount + 1, 0, totalBirdSpecies);
                    
                    // Show feedback and update progress
                    ShowTaskFeedback($"Scanned {birdName}!");
                    UpdateProgress();

                    // Debug: Confirm scan
                    Debug.Log($"Scanned {birdName} | Total Scanned: {scannedBirdsCount}/{totalBirdSpecies}");
                }
                else
                {
                    Debug.LogWarning($"No empty slots left in scannedBirdNames array (max: {totalBirdSpecies})");
                }
            }
            else
            {
                Debug.Log($"{birdName} already scanned - skipping");
            }
        }
    }

    // --------------------------
    // Update Progress (Force Bar Refresh)
    // --------------------------
    private void UpdateProgress()
    {
        // Update progress bar (clamp to avoid overfill)
        progressBar.value = Mathf.Clamp(scannedBirdsCount, 0, progressBar.maxValue);
        UpdateProgressText();

        // Debug: Confirm progress update
        Debug.Log($"Progress Updated | Bar Value: {progressBar.value} | Scanned Count: {scannedBirdsCount}");

        // Check if experience is complete
        CheckForCompletion();
    }

    // --------------------------
    // Update Progress Text (UI)
    // --------------------------
    private void UpdateProgressText()
    {
        progressText.text = $"Scanned: {scannedBirdsCount}/{totalBirdSpecies}";
    }

    // --------------------------
    // Check for Completion (All Birds Scanned)
    // --------------------------
    private void CheckForCompletion()
    {
        bool allScanned = scannedBirdsCount >= totalBirdSpecies;

        if (allScanned && !isExperienceCompleted)
        {
            Debug.Log("All birds scanned! Showing completion screen.");
            CompleteExperience();
        }
    }

    // --------------------------
    // Show Completion Screen (End State)
    // --------------------------
    private void CompleteExperience()
    {
        isExperienceCompleted = true;
        completionScreen.SetActive(true);
        
        // Disable AR tracking to prevent further scans
        if (trackedImageManager != null)
        {
            trackedImageManager.enabled = false;
        }
    }

    // --------------------------
    // Feedback Popup (Auto-Hide)
    // --------------------------
    private void ShowTaskFeedback(string message)
    {
        feedbackText.text = message;
        taskFeedbackPopup.SetActive(true);
        Invoke(nameof(HideFeedbackPopup), 2f);
    }

    private void HideFeedbackPopup()
    {
        taskFeedbackPopup.SetActive(false);
    }

    // --------------------------
    // Return to Start (Reset Experience)
    // --------------------------
    private void ReturnToStart()
    {
        // Hide completion screen
        completionScreen.SetActive(false);
        
        // Reset progress tracking
        scannedBirdsCount = 0;
        scannedBirdNames = new string[totalBirdSpecies];
        isExperienceCompleted = false;
        
        // Re-enable AR tracking
        if (trackedImageManager != null)
        {
            trackedImageManager.enabled = true;
        }
        
        // Reset UI
        InitializeProgressUI();
        Debug.Log("Progress reset - returned to start!");
    }

    // --------------------------
    // Helper: Check if Bird Already Scanned
    // --------------------------
    private bool IsBirdAlreadyScanned(string birdName)
    {
        if (string.IsNullOrEmpty(birdName)) return false;
        return System.Array.Exists(scannedBirdNames, name => name == birdName);
    }

    // --------------------------
    // Validate References (Edit Mode)
    // --------------------------
    void OnValidate()
    {
        if (objectiveText == null) Debug.LogError("ObjectiveText is missing! Assign it in the Inspector.", this);
        if (progressBar == null) Debug.LogError("ProgressBar is missing! Assign it in the Inspector.", this);
        if (progressText == null) Debug.LogError("ProgressText is missing! Assign it in the Inspector.", this);
        if (taskFeedbackPopup == null) Debug.LogError("TaskFeedbackPopup is missing! Assign it in the Inspector.", this);
        if (feedbackText == null) Debug.LogError("FeedbackText is missing! Assign it in the Inspector.", this);
        if (completionScreen == null) Debug.LogError("CompletionScreen is missing! Assign it in the Inspector.", this);
        if (returnToStartButton == null) Debug.LogError("ReturnToStartButton is missing! Assign it in the Inspector.", this);
        if (trackedImageManager == null) Debug.LogWarning("ARTrackedImageManager is not assigned! Assign it in the Inspector.", this);
        if (totalBirdSpecies < 1) Debug.LogWarning("TotalBirdSpecies must be at least 1!", this);
    }
}