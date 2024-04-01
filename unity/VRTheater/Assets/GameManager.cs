using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] Button buttonR;

    [SerializeField] Button buttonL;

    [SerializeField] Toggle toggle;

    [SerializeField] Camera camera;

    [SerializeField] GameObject screen;

    const string THEATER_LAYER = "Theater";

    List<GameObject> m_Panoramas = null;
    int m_Idx = 0;

    List<Object> m_Pictures = null;

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // Button control
        buttonR.onClick.AddListener(Forward);
        buttonL.onClick.AddListener(Back);

        // Toggle control
        toggle.onValueChanged.AddListener(delegate {
            ToggleScreenOnly(toggle);
        });

        var pictures = Resources.LoadAll("Panorama", typeof(Texture2D)).ToList();
        m_Pictures = pictures.OrderBy(x => x.name).ToList();
        Texture2D tex = (Texture2D)m_Pictures[0];
        screen.GetComponent<Renderer>().material.SetTexture("_Texture2D", tex);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Forward()
    {
        m_Idx += 1;
        if (m_Idx >= m_Pictures.Count - 1)
        {
            m_Idx = m_Pictures.Count - 1;
        }
        Texture2D tex = (Texture2D)m_Pictures[m_Idx];
        screen.GetComponent<Renderer>().material.SetTexture("_Texture2D", tex);
    }

    public void Back()
    {
        m_Idx -= 1;
        if (m_Idx < 0)
        {
            m_Idx = 0;
        }
        Texture2D tex = (Texture2D)m_Pictures[m_Idx];
        screen.GetComponent<Renderer>().material.SetTexture("_Texture2D", tex);
    }

    void ToggleScreenOnly(Toggle t)
    {
        if (t.isOn)  // Screen Only mode
        {
            camera.cullingMask &= ~(1 << LayerMask.NameToLayer(THEATER_LAYER));  // Turn off the layer
        }
        else
        {
            camera.cullingMask |= 1 << LayerMask.NameToLayer(THEATER_LAYER);  // Turn on the layer
        }
    }

}
