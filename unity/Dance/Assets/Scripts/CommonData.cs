using UnityEngine;

/// <summary>
/// Simple data container component to share state (like distance) across multiple scripts.
/// </summary>
public class CommonData: MonoBehaviour {

    /// <summary>
    /// The distance from the camera to the target hit point in meters.
    /// </summary>
    public float distance = 0F;
}
