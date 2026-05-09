using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Manages multiple AR markers, instantiating a corresponding prefab for each reference image.
/// Ensures that only one object is active at a time and maintains a vertical (upright) orientation.
/// </summary>
public class MultipleMarkers : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _prefabs; // List of prefabs to associate with marker names

    [SerializeField]
    private ARTrackedImageManager _arTrackedImageManager; // Reference to the ARTrackedImageManager component

    [SerializeField]
    private Button _toggleButton; // UI Button to show/hide the active object

    // Stores instantiated objects mapped by the name of their reference image
    private readonly Dictionary<string, GameObject> _arObjects = new Dictionary<string, GameObject>();
    private GameObject _activeArObject = null; // Reference to the currently visible AR object

    void Start()
    {
        // Subscribe to the trackables changed event using AR Foundation 6.x syntax
        if (_arTrackedImageManager != null)
        {
            _arTrackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        }

        // Instantiate all prefabs at startup and hide them
        foreach (var prefab in _prefabs)
        {
            var instance = Instantiate(prefab);
            instance.name = prefab.name; // Set name to match reference image name
            instance.SetActive(false);
            _arObjects[instance.name] = instance;
        }
    }

    void OnDisable()
    {
        // Unsubscribe from the event when the component is disabled
        if (_arTrackedImageManager != null)
        {
            _arTrackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
        }
    }

    /// <summary>
    /// Handles changes in tracked images (added, updated, or removed).
    /// </summary>
    void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        ARTrackedImage priorityImage = null;

        // Check all currently tracked images to find one with an active tracking state
        foreach (var trackedImage in _arTrackedImageManager.trackables)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                priorityImage = trackedImage;
                break; // Select the first successfully tracked image as priority
            }
        }

        if (priorityImage != null)
        {
            // If a tracked image is found, ensure its corresponding object is active and updated
            SwitchActiveObject(priorityImage);
        }
        else
        {
            // If no images are currently "Tracking", update the pose of the last active object if possible
            UpdateActiveObjectPoseOnly();
        }
    }

    /// <summary>
    /// Switches visibility to the target image's object and hides the previous one.
    /// </summary>
    void SwitchActiveObject(ARTrackedImage targetImage)
    {
        if (_arObjects.TryGetValue(targetImage.referenceImage.name, out GameObject newObject))
        {
            if (_activeArObject != null && _activeArObject != newObject)
            {
                _activeArObject.SetActive(false);
            }

            UpdateObjectPose(newObject, targetImage);
            newObject.SetActive(true);
            _activeArObject = newObject;
        }
    }

    /// <summary>
    /// Corrects rotation here to ensure the object always stands vertically (upright).
    /// </summary>
    void UpdateObjectPose(GameObject obj, ARTrackedImage trackedImage)
    {
        // 1. Align the position with the center of the marker
        Vector3 position = trackedImage.transform.position;

        // 2. Extract only the Y-axis rotation (heading) and set X and Z to 0.
        // This ensures the object stands straight even if the marker is on a tilted surface.
        float rotationY = trackedImage.transform.rotation.eulerAngles.y;
        Quaternion verticalRotation = Quaternion.Euler(0, rotationY, 0);

        obj.transform.SetPositionAndRotation(position, verticalRotation);
        
        // Note: Parenting the object to the marker would cause it to inherit the marker's tilt.
        // To maintain verticality, it is safer to keep SetParent(null) or update the pose manually.
        obj.transform.SetParent(null); 
    }

    /// <summary>
    /// Updates the pose of the currently active object if its marker is still being detected.
    /// </summary>
    void UpdateActiveObjectPoseOnly()
    {
        if (_activeArObject == null) return;

        foreach (var trackedImage in _arTrackedImageManager.trackables)
        {
            if (trackedImage.referenceImage.name == _activeArObject.name)
            {
                UpdateObjectPose(_activeArObject, trackedImage);
                break;
            }
        }
    }
}
