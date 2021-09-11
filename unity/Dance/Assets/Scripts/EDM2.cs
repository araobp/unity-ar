using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class EDM2 : MonoBehaviour
{
    [SerializeField]
    ARPlaneManager m_ARPlaneManager;

    [SerializeField]
    ARRaycastManager m_ARRaycastManager;

    [SerializeField]
    float m_MinimumRaycastDistance = 0.5F;

    [SerializeField]
    Text m_TextDistance;

    float m_Distance = 0F;

    Vector2 m_aimPosition = new Vector2(Screen.width / 2, Screen.height / 2);

    Transform m_arCameraTransform;

    CommonData m_CommonData;

    // Raycast against planes and feature points
    const TrackableType trackableTypes =
        TrackableType.FeaturePoint |
        TrackableType.PlaneWithinPolygon;

    List<ARRaycastHit> m_hits = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        m_CommonData = GetComponent<CommonData>();

        m_arCameraTransform = m_CommonData.ARCamera.transform;

        StartCoroutine(UpdateDistance());
    }

    // Update is called once per frame
    IEnumerator UpdateDistance()
    {
        while (true)
        {
            if (m_ARPlaneManager != null)
            {
                float _distance = 0F;

                if (m_ARRaycastManager.Raycast(m_aimPosition, m_hits, trackableTypes))
                {
                    Vector3 point = m_hits[m_hits.Count - 1].pose.position;  // takes the furthest hit point
                    _distance = (point - m_arCameraTransform.position).magnitude;
                    if (_distance < m_MinimumRaycastDistance)
                    {
                        _distance = 0F;
                    }
                }

                m_CommonData.distance = _distance;

                if (_distance == 0F)
                {
                    m_TextDistance.text = "...";
                } else
                {
                    m_TextDistance.text = $"{_distance.ToString("F2")}m";
                }
            }
            yield return new WaitForSeconds(0.2F);
        }
    }

    float distance
    {
        get => m_Distance;
    }
}
