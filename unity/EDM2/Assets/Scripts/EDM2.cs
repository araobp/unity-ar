using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Handles electronic distance measurement by raycasting against the AR Point Cloud.
/// </summary>
public class EDM2 : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager _arRaycastManager;

    [SerializeField]
    TMP_Dropdown _dropdown;

    [SerializeField]
    Text _textDistance;

    // The radius of the central circle for feature point detection, as a fraction of screen width.
    [SerializeField]
    [Range(0.01f, 0.5f)] // Allow radius from 1% to 50% of screen width
    float _screenCircleRadiusFraction = 0.1f; // Default to 1/10th of screen width

    // Cached list to store raycast hits to avoid garbage collection allocations every frame.
    readonly List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    void Start()
    {
        // Verify that the necessary AR component is assigned.
        if (_arRaycastManager == null)
        {
            Debug.LogError("ARRaycastManager is not assigned.");
        }
    }

    void Update()
    {
        // Exit early if dependencies are missing or if there is no main camera.
        if (_arRaycastManager == null || _textDistance == null || Camera.main == null)
            return;

        // Calculate the screen center point to use as the "crosshair" for measurement.
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        float distance = 0f;
        bool hasValidHit = false;

        // Calculate the radius of the central circle in pixels.
        float screenRadius = Screen.width * _screenCircleRadiusFraction;

        // Determine the target trackable type based on the Dropdown's current label.
        TrackableType hitTypes = TrackableType.None;
        if (_dropdown != null)
        {
            string selectedLabel = _dropdown.options[_dropdown.value].text;
            if (selectedLabel == "Plane")
            {
                // Using PlaneWithinPolygon for the most accurate surface measurement.
                hitTypes = TrackableType.PlaneWithinPolygon;
            }
            else if (selectedLabel == "Point Cloud")
            {
                hitTypes = TrackableType.FeaturePoint;
            }
        }

        // Cast a ray from the screen center using the selected hit types.
        if (hitTypes != TrackableType.None && _arRaycastManager.Raycast(screenCenter, _hits, hitTypes))
        {
            // Check if we are specifically measuring against the Point Cloud.
            bool isPointCloud = hitTypes == TrackableType.FeaturePoint;

            // If Point Cloud, we iterate backwards to find the furthest point first.
            // If Planes, we iterate forwards to find the closest point first.
            for (int i = 0; i < _hits.Count; i++)
            {
                int targetIndex = isPointCloud ? (_hits.Count - 1 - i) : i;
                var hit = _hits[targetIndex];

                Vector3 hitPosition = hit.pose.position;
                Vector3 cameraPosition = Camera.main.transform.position;

                // Explicitly calculate the Euclidean distance between the camera and the hit point.
                float currentDistance = Vector3.Distance(cameraPosition, hitPosition);

                // Project the hit position back to screen space to check if it falls within the desired circle.
                Vector2 hitScreenPosition = Camera.main.WorldToScreenPoint(hitPosition);

                // Calculate the distance from the hit's screen position to the screen center.
                float distToScreenCenter = Vector2.Distance(hitScreenPosition, screenCenter);

                // Validate the measurement against the screen-space radius.
                if (distToScreenCenter <= screenRadius)
                {
                    distance = currentDistance;
                    hasValidHit = true;

                    // Since we want the closest distance and the list is sorted, 
                    // we break at the first hit that meets the criteria.
                    break;
                }
            }
        }

        // Update the UI: show formatted distance if valid, otherwise show placeholder.
        _textDistance.text = hasValidHit ? $"{distance:F2}m" : "...";
    }
}
