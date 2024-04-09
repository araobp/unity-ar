using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField]
    Dropdown m_DropdownPrefabs;

    [SerializeField]
    RawImage m_RawImageAim;

    [SerializeField]
    Text m_TextDistance;

    [SerializeField]
    float m_Speed = 2F;

    GameObject m_Instance;

    List<GameObject> m_ListMarkers = new List<GameObject>();

    CommonData m_CommonData;

    // Start is called before the first frame update
    void Start()
    {
        m_CommonData = GetComponent<CommonData>();
    }

    void Update()
    {
    }

    public void OnValueChanged()
    {
        if (m_Instance != null)
        {
            Destroy(m_Instance);
        }

        m_RawImageAim.enabled = true;
        m_TextDistance.enabled = true;
    }

    public void PlaceObject()
    {
        if (m_CommonData.distance != 0F)
        {
            string s = m_DropdownPrefabs.options[m_DropdownPrefabs.value].text;
            string[] id = s.Split(':');
            string prefabName = id[0];
            string markerId = id[1];

            GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");

            Transform t = m_CommonData.ARCamera.transform;

            Vector3 cameraPos = t.position;
            Vector3 cameraForward = t.forward;
            Vector3 hitPoint = cameraPos + cameraForward * m_CommonData.distance;
            Vector3 p = hitPoint;

            Vector3 pos = cameraPos;
            pos.y = 0F;
            p.y = 0F;
            Vector3 toward = pos - p;

            if (m_Instance != null)
            {
                Destroy(m_Instance);
            }

            m_Instance = Instantiate(prefab, hitPoint, Quaternion.LookRotation(toward.normalized, Vector3.up));

            GameObject m = GameObject.FindGameObjectWithTag("Markers");
            foreach (Transform markerTransform in m.transform)
            {
                GameObject obj = markerTransform.gameObject;
                if (obj.name == $"Marker{markerId}")
                {
                    Vector3 shift = -markerTransform.localPosition;
                    m_Instance.transform.Translate(shift);
                }
            }
        }

        m_RawImageAim.enabled = false;
        m_TextDistance.enabled = false;
    }
}
