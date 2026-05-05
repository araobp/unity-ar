using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Manages the VR Theater experience, including panorama image switching 
/// and UI interactions for toggling environment visibility.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    // The crosshair UI element shown during placement
    [SerializeField] RawImage _rawImageAim;
    // Button used to confirm placement of the theater
    [SerializeField] Button _buttonPlace;
    // Button used to remove the current theater and restart placement
    [SerializeField] Button _buttonReposition;
    // Optional text element for displaying messages to the user.
    [SerializeField] TMP_Text _textMessage;
    // Navigation buttons to cycle through panorama images
    [SerializeField] Button _buttonR;
    [SerializeField] Button _buttonL;
    // Toggle to hide the environment and show only the screen
    [SerializeField] Toggle _toggle;

    [Header("AR & Prefabs")]
    // Handles raycasting against AR planes
    [SerializeField] ARRaycastManager _raycastManager;
    // The theater environment prefab to be instantiated
    [SerializeField] GameObject _vrTheaterPrefab;

    GameObject _vrTheater;       // Reference to the active theater instance
    Renderer _screenRenderer;    // Cached renderer of the screen within the theater
    Camera _mainCamera;          // Cached reference to the main camera
    List<Texture2D> _pictures;   // List of loaded panorama textures
    int _idx = 0;                // Current texture index

    const string THEATER_LAYER = "Theater"; // Layer name for visibility toggling

    // Pre-calculate shader property ID for performance
    // Note: this shader is my original, so the property name is hardcoded here.
    // If you change the shader, update this string accordingly.
    static readonly int TexturePropID = Shader.PropertyToID("_Texture2D");

    void Start()
    {
        // Ensure the application remains in landscape mode
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        _mainCamera = Camera.main;

        // Load all textures from "Resources/Panorama" and sort them alphabetically
        _pictures = Resources.LoadAll<Texture2D>("Panorama")
            .OrderBy(p => p.name)
            .ToList();

        // Setup button listeners
        _buttonPlace.onClick.AddListener(PlaceTheater);
        _buttonR.onClick.AddListener(Forward);
        _buttonReposition.onClick.AddListener(RepositionTheater);
        _buttonL.onClick.AddListener(Back);

        // Setup toggle listener for layer visibility
        _toggle.onValueChanged.AddListener(_ => ToggleScreenOnly(_toggle));

        // Initially hide the reposition button until the theater is placed
        _buttonReposition.gameObject.SetActive(false);

        // Ensure initial UI state is set correctly
        SetPlacementUIVisibility(true);
    }

    void Update()
    {
        // Only show guidance if the theater hasn't been placed yet
        if (_vrTheater == null && _textMessage != null)
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            // Check if an AR plane is detected at the center of the screen
            if (_raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                _textMessage.text = "Ready to place";
            }
            else
            {
                _textMessage.text = "Please scan floors or walls. Move your phone slowly.";
            }
        }
    }

    /// <summary>
    /// Updates the main screen material with the texture at the current index.
    /// </summary>
    void UpdateScreenTexture()
    {
        if (_screenRenderer == null)
        {
            Debug.LogWarning("Screen renderer not found.");
            return;
        }
        // Apply the selected texture to the screen's material
        _screenRenderer.material.SetTexture(TexturePropID, _pictures[_idx]);
    }

    /// <summary>
    /// Advances to the next panorama image and updates the screen material.
    /// </summary>
    public void Forward()
    {
        _idx = Mathf.Min(_idx + 1, _pictures.Count - 1); // Clamp to max index
        UpdateScreenTexture();
    }

    /// <summary>
    /// Returns to the previous panorama image and updates the screen material.
    /// </summary>
    public void Back()
    {
        _idx = Mathf.Max(_idx - 1, 0); // Clamp to min index
        UpdateScreenTexture();
    }

    /// <summary>
    /// Raycasts from the center of the screen to AR planes and instantiates the VR Theater at the hit position.
    /// </summary>
    public void PlaceTheater()
    {
        // Calculate the center of the screen in pixels
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        // Perform the raycast against AR planes (TrackableType.PlaneWithinPolygon)
        if (_raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            // Use the pose (position and rotation) of the first hit detected
            Pose hitPose = hits[0].pose;

            // Destroy the existing theater if it was already placed
            if (_vrTheater != null)
            {
                Destroy(_vrTheater);
            }

            // Instantiate the theater prefab at the hit location
            _vrTheater = Instantiate(_vrTheaterPrefab, hitPose.position, hitPose.rotation);

            // Find the object named "SCREEN" in the instantiated prefab and cache its renderer
            _screenRenderer = _vrTheater.GetComponentsInChildren<Renderer>(true)
                .FirstOrDefault(r => r.name == "SCREEN");

            // Switch UI to active mode and hide placement guidance
            SetPlacementUIVisibility(false);

            // Apply the current panorama texture to the newly created theater
            UpdateScreenTexture();
        }
    }

    /// <summary>
    /// Toggles the visibility of the "Theater" layer using the main camera's culling mask.
    /// </summary>
    void ToggleScreenOnly(Toggle t)
    {
        int layer = LayerMask.NameToLayer(THEATER_LAYER);
        if (t.isOn)
        {
            // Turn OFF the theater layer in the camera's culling mask (bitwise AND NOT)
            _mainCamera.cullingMask &= ~(1 << layer);
        }
        else
        {
            // Turn ON the theater layer in the camera's culling mask (bitwise OR)
            _mainCamera.cullingMask |= 1 << layer;
        }
    }

    /// <summary>
    /// Destroys the instantiated VR Theater and makes the placement UI visible again.
    /// This allows the user to reposition the theater.
    /// </summary>
    public void RepositionTheater()
    {
        if (_vrTheater != null)
        {
            Destroy(_vrTheater);
            _vrTheater = null;
            _screenRenderer = null;
        }

        // Switch UI back to "Placement mode"
        SetPlacementUIVisibility(true);
    }

    /// <summary>
    /// Sets the visibility of UI elements based on whether the theater is being placed or is already active.
    /// </summary>
    /// <param name="visible">True if placement UI should be shown; False for reposition UI.</param>
    void SetPlacementUIVisibility(bool visible)
    {
        _buttonPlace.gameObject.SetActive(visible);
        _rawImageAim.gameObject.SetActive(visible);

        // Hide the guidance text when the theater is placed
        if (_textMessage != null)
        {
            _textMessage.gameObject.SetActive(visible);
        }

        // Show reposition button only when the theater is active
        _buttonReposition.gameObject.SetActive(!visible);
    }
}
