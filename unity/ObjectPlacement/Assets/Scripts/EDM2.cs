using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class EDM2 : MonoBehaviour
{
    [SerializeField]
    ARPlaneManager _ARPlaneManager;

    [SerializeField]
    ARRaycastManager _ARRaycastManager;

    [SerializeField]
    float _MinimumRaycastDistance = 0.5F;

    [SerializeField]
    Text _TextDistance;

    float _Distance = 0F;

    Vector2 _aimPosition = new Vector2(Screen.width / 2, Screen.height / 2);

    Transform _arCameraTransform;

    CommonData _CommonData;

    // Raycast against planes and feature points
    const TrackableType trackableTypes =
        TrackableType.FeaturePoint |
        TrackableType.PlaneWithinPolygon;

    List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        _CommonData = GetComponent<CommonData>();

        _arCameraTransform = Camera.main.transform;

        StartCoroutine(UpdateDistance());
    }

    // Update is called once per frame
    IEnumerator UpdateDistance()
    {
        while (true)
        {
            if (_ARPlaneManager != null)
            {
                float _distance = 0F;

                if (_ARRaycastManager.Raycast(_aimPosition, _hits, trackableTypes))
                {
                    Vector3 point = _hits[_hits.Count - 1].pose.position;  // takes the furthest hit point
                    _distance = (point - _arCameraTransform.position).magnitude;
                    if (_distance < _MinimumRaycastDistance)
                    {
                        _distance = 0F;
                    }
                }

                _CommonData.distance = _distance;

                if (_distance == 0F)
                {
                    _TextDistance.text = "...";
                } else
                {
                    _TextDistance.text = $"{_distance.ToString("F2")}m";
                }
            }
            yield return new WaitForSeconds(0.2F);
        }
    }

    float distance
    { // This property is not used anywhere in the provided code. Consider removing it if not needed.
        get => _Distance;
    }
}
