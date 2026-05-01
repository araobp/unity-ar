using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField]
    Dropdown _DropdownPrefabs;

    [SerializeField]
    RawImage _RawImageAim;

    [SerializeField]
    CommonData _CommonData;

    [SerializeField]
    Text _TextDistance;

    GameObject _Instance;

    List<GameObject> _ListMarkers = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
    }

    public void OnDropdownValueChanged() // Renamed for clarity, as it's likely tied to the dropdown.
    {
        if (_Instance != null)
        {
            Destroy(_Instance);
        }

        _RawImageAim.enabled = true;
        _TextDistance.enabled = true;
    }

    public void PlaceObject()
    {
        if (_CommonData.distance != 0F)
        {
            string s = _DropdownPrefabs.options[_DropdownPrefabs.value].text;
            string[] id = s.Split(':');
            string prefabName = id[0];
            string markerId = id[1];

            GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}"); // Consider caching prefabs if they are loaded frequently.

            Transform t = Camera.main.transform;

            Vector3 cameraPos = t.position;
            Vector3 cameraForward = t.forward;
            Vector3 hitPoint = cameraPos + cameraForward * _CommonData.distance;
            Vector3 p = hitPoint;

            Vector3 pos = cameraPos;
            pos.y = 0F;
            p.y = 0F;
            Vector3 toward = pos - p;

            if (_Instance != null)
            {
                Destroy(_Instance);
            }

            _Instance = Instantiate(prefab, hitPoint, Quaternion.LookRotation(toward.normalized, Vector3.up));

            GameObject m = GameObject.FindGameObjectWithTag("Markers");
            foreach (Transform markerTransform in m.transform)
            {
                GameObject obj = markerTransform.gameObject;
                if (obj.name == $"Marker{markerId}")
                {
                    Vector3 shift = -markerTransform.localPosition;
                    _Instance.transform.Translate(shift);
                }
            }
        }

        _RawImageAim.enabled = false;
        _TextDistance.enabled = false;
    }
}
