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
    GameObject m_ARCamera;

    [SerializeField]
    float m_MinimumRaycastDistance = 0.5F;

    [SerializeField]
    Text m_TextDistance;

    Vector2 m_aimPosition = new Vector2(Screen.width / 2, Screen.height / 2);

    Transform m_arCameraTransform;

    // Raycast against planes and feature points
    const TrackableType trackableTypes =
        TrackableType.FeaturePoint |
        TrackableType.PlaneWithinPolygon;

    List<ARRaycastHit> m_hits = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        m_arCameraTransform = m_ARCamera.transform;   
    }

    // Update is called once per frame
    void Update()
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

            if (_distance == 0)
            {
                m_TextDistance.text = "...";
            } else
            {
                m_TextDistance.text = $"{_distance.ToString("F2")}m";
            }
        }
    }
}
