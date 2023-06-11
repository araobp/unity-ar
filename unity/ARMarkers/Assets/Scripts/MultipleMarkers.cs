using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

// Reference: https://qiita.com/OKsaiyowa/items/29504242ec74cb5dfb04
public class MultipleMarkers : MonoBehaviour
{
    [SerializeField]
    GameObject[] _prefabs;

    [SerializeField]
    ARTrackedImageManager _arTrackedImageManager;

    [SerializeField]
    Button _buttonToggle;

    GameObject _currentArObject = null;

    readonly Dictionary<string, GameObject> _arObjects = new Dictionary<string, GameObject>();

    bool _clear = false;

    public void OnToggle()
    {
        _clear = !_clear;
        if (_currentArObject != null)
        {
            _currentArObject.SetActive(!_clear);
        }
    }

    void Start()
    {
        _arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

        for (var i = 0; i < _prefabs.Length; i++)
        {
            var prefab = Instantiate(_prefabs[i]);
            prefab.name = _prefabs[i].name;
            _arObjects[prefab.name] = prefab;
            prefab.SetActive(false);
        }
    }

    void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void ActivateARObject(ARTrackedImage trackedImage)
    {
        var arObject = _arObjects[trackedImage.referenceImage.name];
        var imageMarkerTransform = trackedImage.transform;

        var markerFrontRotation = imageMarkerTransform.rotation * Quaternion.Euler(90f, 0f, 0f);
        arObject.transform.SetPositionAndRotation(imageMarkerTransform.transform.position, markerFrontRotation);
        arObject.transform.SetParent(imageMarkerTransform);
        if (trackedImage.trackingState == TrackingState.Tracking || trackedImage.trackingState == TrackingState.Limited)
        {
            arObject.SetActive(true);
            _currentArObject = arObject;
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            if (_currentArObject != null)
            {
                _currentArObject.SetActive(false);
            }
            ActivateARObject(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            ActivateARObject(trackedImage);
        }
    }

}