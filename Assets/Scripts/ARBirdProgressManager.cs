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
    [SerializeField] private int totalBirdSpecies = 3; // Total birds to scan (Kingfisher, Peacock, Duck)
    [SerializeField] private bool requireFeeding = true; // Require feeding each bird to complete

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
    private int fedBirdsCount = 0;
    private string[] scannedBirdNames = new string[3]; // Track which birds are scanned
    private bool[] fedBirdStatus = new bool[3]; // Track if each scanned bird is fed
    private bool isExperienceCompleted = false;

    // --------------------------
    // AR Reference (Optional)
    // --------------------------
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    void Start()
    {
        // Initialize UI
        InitializeProgressUI();
        
        // Set up return button
        returnToStartButton.onClick.AddListener(ReturnToStart);
        
        // FIX: Add null check BEFORE accessing trackablesChanged
        if (trackedImageManager != null)
        {
            // For AR Foundation 6.0+: Use trackablesChanged instead of trackedImagesChanged
            trackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
            
            // Optional: Enable the tracked image manager (ensure it's active)
            trackedImageManager.enabled = true;
        }
        else
        {
            Debug.LogWarning("ARTrackedImageManager is not assigned! Assign it in the Inspector.", this);
        }

        // Hide feedback/completion screens by default
        taskFeedbackPopup.SetActive(false);
        completionScreen.SetActive(false);
    }

    // --------------------------
    // Clean Up Event Listener (Critical to Avoid Memory Leaks)
    // --------------------------
    void OnDestroy()
    {
        // FIX: Remove event listener when the object is destroyed
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
        // Set main objective text
        string objective = requireFeeding 
            ? $"Scan & feed all {totalBirdSpecies} bird species!" 
            : $"Scan all {totalBirdSpecies} bird species!";
        objectiveText.text = objective;

        // Reset progress bar/text
        progressBar.maxValue = requireFeeding ? (totalBirdSpecies * 2) : totalBirdSpecies;
        progressBar.value = 0;
        UpdateProgressText();
    }

    // --------------------------
    // Track AR Image Scans (Auto-Detect Scanned Birds)
    // --------------------------
    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // Loop through NEWLY added tracked images (only scan once per bird)
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            // FIX: Check if the image is valid (not null/empty)
            if (trackedImage.referenceImage == null || string.IsNullOrEmpty(trackedImage.referenceImage.name))
            {
                continue; // Skip invalid images
            }

            string birdName = trackedImage.referenceImage.name;
            
            // Only process if the bird hasn't been scanned yet
            if (!IsBirdAlreadyScanned(birdName))
            {
                // Mark bird as scanned (prevent duplicates)
                scannedBirdNames[scannedBirdsCount] = birdName;
                scannedBirdsCount = Mathf.Clamp(scannedBirdsCount + 1, 0, totalBirdSpecies);
                
                // Show feedback popup
                ShowTaskFeedback($"Scanned {birdName}! Now feed it!");
                
                // Update progress
                UpdateProgress();
            }
        }

        // Optional: Handle updated images (in case the bird moves)
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                // Bird is still visible (no action needed for progress)
            }
        }
    }

    // --------------------------
    // Public Methods (Call These from Bird Interaction Scripts)
    // --------------------------
    // Call this when a bird is fed (from your feeding mechanic)
    public void MarkBirdAsFed(string birdName)
    {
        if (isExperienceCompleted) return;
        
        // Find index of scanned bird
        int birdIndex = System.Array.IndexOf(scannedBirdNames, birdName);
        if (birdIndex != -1 && !fedBirdStatus[birdIndex])
        {
            fedBirdStatus[birdIndex] = true;
            fedBirdsCount = Mathf.Clamp(fedBirdsCount + 1, 0, totalBirdSpecies);
            
            // Show feedback popup
            ShowTaskFeedback($"Fed {birdName}! ❤️");
            
            // Update progress
            UpdateProgress();
        }
    }

    // --------------------------
    // Progress Logic
    // --------------------------
    private void UpdateProgress()
    {
        // Calculate progress (scan = 1 point, feed = 1 point per bird if required)
        int currentProgress = scannedBirdsCount;
        if (requireFeeding) currentProgress += fedBirdsCount;
        
        // Update UI (clamp to max value to avoid overfilling)
        progressBar.value = Mathf.Clamp(currentProgress, 0, progressBar.maxValue);
        UpdateProgressText();

        // Check if experience is complete
        CheckForCompletion();
    }

    private void UpdateProgressText()
    {
        string progressDetails = requireFeeding
            ? $"Scanned: {scannedBirdsCount}/{totalBirdSpecies} | Fed: {fedBirdsCount}/{scannedBirdsCount}"
            : $"Scanned: {scannedBirdsCount}/{totalBirdSpecies}";
        
        progressText.text = progressDetails;
    }

    private void CheckForCompletion()
    {
        // Completion condition: All birds scanned + (all fed if required)
        bool allScanned = scannedBirdsCount >= totalBirdSpecies;
        bool allFed = fedBirdsCount >= totalBirdSpecies;
        bool completionMet = requireFeeding ? (allScanned && allFed) : allScanned;

        if (completionMet && !isExperienceCompleted)
        {
            CompleteExperience();
        }
    }

    // --------------------------
    // Feedback & Completion
    // --------------------------
    private void ShowTaskFeedback(string message)
    {
        feedbackText.text = message;
        taskFeedbackPopup.SetActive(true);
        
        // Auto-hide popup after 2 seconds
        Invoke(nameof(HideFeedbackPopup), 2f);
    }

    private void HideFeedbackPopup()
    {
        taskFeedbackPopup.SetActive(false);
    }

    private void CompleteExperience()
    {
        isExperienceCompleted = true;
        
        // Show completion screen (pause AR interaction)
        completionScreen.SetActive(true);
        
        // Optional: Disable AR tracking to prevent further interaction
        if (trackedImageManager != null)
        {
            trackedImageManager.enabled = false;
        }
    }

    // --------------------------
    // Return to Start (Reset Experience)
    // --------------------------
    private void ReturnToStart()
    {
        // Hide completion screen
        completionScreen.SetActive(false);
        
        // Reset progress
        scannedBirdsCount = 0;
        fedBirdsCount = 0;
        scannedBirdNames = new string[totalBirdSpecies];
        fedBirdStatus = new bool[totalBirdSpecies];
        isExperienceCompleted = false;
        
        // Re-enable AR tracking
        if (trackedImageManager != null)
        {
            trackedImageManager.enabled = true;
        }
        
        // Reset UI
        InitializeProgressUI();
    }

    // --------------------------
    // Helper Methods
    // --------------------------
    private bool IsBirdAlreadyScanned(string birdName)
    {
        if (string.IsNullOrEmpty(birdName)) return false;
        return System.Array.Exists(scannedBirdNames, name => name == birdName);
    }

    // Optional: Validate UI references in Edit Mode
    void OnValidate()
    {
        if (objectiveText == null) Debug.LogError("ObjectiveText is missing!", this);
        if (progressBar == null) Debug.LogError("ProgressBar is missing!", this);
        if (progressText == null) Debug.LogError("ProgressText is missing!", this);
        if (trackedImageManager == null) Debug.LogWarning("ARTrackedImageManager is not assigned!", this);
    }
}