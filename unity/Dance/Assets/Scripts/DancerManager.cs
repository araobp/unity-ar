using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DancerManager : MonoBehaviour
{
    [SerializeField]
    RawImage m_RawImageAim;

    [SerializeField]
    Text m_TextDistance;

    [SerializeField]
    Slider m_SliderScale;

    [SerializeField]
    Dropdown m_DropdownCharacter;

    GameObject m_Instance;

    CommonData m_CommonData;

    Animator m_Animator;

    Vector3 scale
    {
        get => Vector3.one * (m_SliderScale.value / 10F);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_CommonData = GetComponent<CommonData>();
    }

    public void Clear()
    {
        if (m_Instance != null)
        {
            Destroy(m_Instance);
        }

        m_RawImageAim.enabled = true;
        m_TextDistance.enabled = true;
    }

    public void Place()
    {
        if (m_CommonData.distance != 0F)
        {
            string character = m_DropdownCharacter.options[m_DropdownCharacter.value].text;
            GameObject prefab = Resources.Load<GameObject>(character);

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
                m_Animator = null;
                Destroy(m_Instance);
            }

            m_Instance = Instantiate(prefab, hitPoint, Quaternion.LookRotation(toward.normalized, Vector3.up));
            m_Instance.transform.localScale = scale;
            m_Animator = m_Instance.GetComponent<Animator>();
            if (m_Animator == null)
            {
                m_Animator = m_Instance.GetComponentInChildren<Animator>();
            }
        }

        m_RawImageAim.enabled = false;
        m_TextDistance.enabled = false;
    }

    public void ScaleChange()
    {
        if (m_Instance != null && m_SliderScale != null)
        {
            m_Instance.transform.localScale = scale;
        }
    }

    public void Dance()
    {
        if (m_Instance != null)
        {
            m_Animator.SetTrigger("Dance");
        }
    }

    public void Turn()
    {
        if (m_Instance != null)
        {
            m_Animator.SetTrigger("Turn");
        }
    }

    public void Kick()
    {
        if (m_Instance != null)
        {
            m_Animator.SetTrigger("Kick");
        }
    }

    public void Jump()
    {
        if (m_Instance != null)
        {
            m_Animator.SetTrigger("Jump");
        }
    }

}
