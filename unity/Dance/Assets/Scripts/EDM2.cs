using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Manages distance measurement in AR space and updates shared data.
/// Uses raycasting from the center of the screen to calculate distance to planes or feature points.
/// </summary>
public class EDM2 : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the AR Plane Manager")]
    ARPlaneManager _arPlaneManager;

    [SerializeField]
    [Tooltip("Reference to the AR Raycast Manager")]
    ARRaycastManager _arRaycastManager;

    [SerializeField]
    [Tooltip("The minimum distance required for a valid measurement")]
    float _minimumRaycastDistance = 0.5F;

    [SerializeField]
    [Tooltip("UI Text to display the measured distance")]
    Text _textDistance;

    // Internal storage for the measured distance
    float _distance = 0F;

    // Screen position for the raycast (center of the screen)
    Vector2 _aimPosition = new Vector2(Screen.width / 2, Screen.height / 2);

    // Reference to the main AR camera's transform
    Transform _arCameraTransform;

    CommonData _commonData;

    // Reusable list to store raycast hits
    List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    void Start()
    {
        _commonData = GetComponent<CommonData>();

        _arCameraTransform = Camera.main.transform;

        StartCoroutine(UpdateDistance());
    }

    /// <summary>
    /// Coroutine that periodically performs a raycast to update the current distance.
    /// </summary>
    IEnumerator UpdateDistance()
    {
        while (true)
        {
            if (_arPlaneManager != null)
            {
                float currentDistance = 0F;

                if (_arRaycastManager.Raycast(_aimPosition, _hits, TrackableType.PlaneWithinPolygon))
                {
                    // Retrieve the furthest hit point among detected trackables
                    Vector3 point = _hits[_hits.Count - 1].pose.position;
                    currentDistance = (point - _arCameraTransform.position).magnitude;
                    
                    // Reset distance if it falls below the minimum threshold
                    if (currentDistance < _minimumRaycastDistance)
                    {
                        currentDistance = 0F;
                    }
                }

                _commonData.distance = currentDistance;

                if (currentDistance == 0F)
                {
                    _textDistance.text = "...";
                } else
                {
                    _textDistance.text = $"{currentDistance.ToString("F2")}m";
                }
            }
            // Update measurement every 0.2 seconds to save performance
            yield return new WaitForSeconds(0.2F);
        }
    }

    /// <summary>
    /// Public access to the measured distance.
    /// </summary>
    public float distance
    {
        get => _distance;
    }
}
