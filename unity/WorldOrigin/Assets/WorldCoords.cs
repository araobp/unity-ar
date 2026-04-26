using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Monitors the Main Camera's world position and displays the coordinates on the UI.
/// </summary>
public class WorldCoords : MonoBehaviour
{
    // Serialized fields for UI Text components assigned in the Unity Inspector
    [SerializeField]
    Text _textX;

    [SerializeField]
    Text _textY;

    [SerializeField]
    Text _textZ;

    // Update is called once per frame to refresh coordinate values
    void Update()
    {
        // Capture the current world position of the main camera
        Vector3 position = Camera.main.transform.position;

        // Format the position components into strings with 2 decimal places and update the UI
        _textX.text = $"X: {position.x.ToString("F2")}m";
        _textY.text = $"Y: {position.y.ToString("F2")}m";
        _textZ.text = $"Z: {position.z.ToString("F2")}m";
    }
}
