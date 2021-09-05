using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCoords : MonoBehaviour
{
    [SerializeField]
    GameObject m_ARCamera;

    [SerializeField]
    Text m_TextX;

    [SerializeField]
    Text m_TextY;

    [SerializeField]
    Text m_TextZ;

    // Update is called once per frame
    void Update()
    {
        Vector3 p = m_ARCamera.transform.position;
        m_TextX.text = $"X: {p.x.ToString("F2")}m";
        m_TextY.text = $"Y: {p.y.ToString("F2")}m";
        m_TextZ.text = $"Z: {p.z.ToString("F2")}m";
    }
}
