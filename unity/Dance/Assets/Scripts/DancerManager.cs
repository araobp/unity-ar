using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the instantiation, placement, and animation control of the dancer character.
/// Interacts with the AR environment through CommonData.
/// </summary>
public class DancerManager : MonoBehaviour
{
    [SerializeField]
    RawImage _rawImageAim;

    [SerializeField]
    Text _textDistance;

    [SerializeField]
    Slider _sliderScale;

    [SerializeField]
    Dropdown _dropdownCharacter;

    // Reference to the currently instantiated character
    GameObject _instance;

    CommonData _commonData;

    // Cache for the animator of the current instance
    Animator _animator;

    /// <summary>
    /// Calculates character scale based on UI slider value.
    /// </summary>
    Vector3 scale
    {
        get => Vector3.one * (_sliderScale.value / 10F);
    }

    void Start()
    {
        _commonData = GetComponent<CommonData>();
    }

    /// <summary>
    /// Removes the current dancer instance and resets the UI state.
    /// </summary>
    public void Clear()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }

        _rawImageAim.enabled = true;
        _textDistance.enabled = true;
    }

    /// <summary>
    /// Spawns a character prefab at the target AR location and makes it face the camera.
    /// </summary>
    public void Place()
    {
        // Only allow placement if a valid distance has been measured
        if (_commonData.distance != 0F)
        {
            // Dynamically load prefab based on selected dropdown text
            string character = _dropdownCharacter.options[_dropdownCharacter.value].text;
            GameObject prefab = Resources.Load<GameObject>(character);

            Transform t = Camera.main.transform;

            // Calculate world position based on camera forward vector and measured distance
            Vector3 cameraPos = t.position;
            Vector3 cameraForward = t.forward;
            Vector3 hitPoint = cameraPos + cameraForward * _commonData.distance;
            Vector3 p = hitPoint;

            Vector3 pos = cameraPos;
            pos.y = 0F;
            p.y = 0F;
            Vector3 toward = pos - p;

            // Cleanup existing instance before creating a new one
            if (_instance != null)
            {
                _animator = null;
                Destroy(_instance);
            }

            // Instantiate and rotate character to face the camera on the horizontal plane
            _instance = Instantiate(prefab, hitPoint, Quaternion.LookRotation(toward.normalized, Vector3.up));
            _instance.transform.localScale = scale;
            
            // Retrieve Animator component from the spawned object
            _animator = _instance.GetComponent<Animator>();
            if (_animator == null)
            {
                _animator = _instance.GetComponentInChildren<Animator>();
            }
        }

        _rawImageAim.enabled = false;
        _textDistance.enabled = false;
    }

    /// <summary>
    /// Updates the scale of the character instance in real-time based on the slider.
    /// </summary>
    public void ScaleChange()
    {
        if (_instance != null && _sliderScale != null)
        {
            _instance.transform.localScale = scale;
        }
    }

    /// <summary>
    /// Triggers the "Dance" animation on the character.
    /// </summary>
    public void Dance()
    {
        if (_instance != null)
        {
            _animator.SetTrigger("Dance");
        }
    }

    /// <summary>
    /// Triggers the "Turn" animation on the character.
    /// </summary>
    public void Turn()
    {
        if (_instance != null)
        {
            _animator.SetTrigger("Turn");
        }
    }

    /// <summary>
    /// Triggers the "Kick" animation on the character.
    /// </summary>
    public void Kick()
    {
        if (_instance != null)
        {
            _animator.SetTrigger("Kick");
        }
    }

    /// <summary>
    /// Triggers the "Jump" animation on the character.
    /// </summary>
    public void Jump()
    {
        if (_instance != null)
        {
            _animator.SetTrigger("Jump");
        }
    }

}
