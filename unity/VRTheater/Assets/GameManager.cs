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

    const string THEATER_LAYER = "Theater";

    List<GameObject> m_Panoramas = null;
    int m_Idx = 0;

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // Screen control
        m_Panoramas = GameObject.FindGameObjectsWithTag("panorama").ToList();
        m_Panoramas = m_Panoramas.OrderBy(x => x.name).ToList();

        m_Panoramas.ToList().ForEach(p =>
        {
            p.SetActive(false);
            Debug.Log(p.ToString());
        });

        m_Panoramas[m_Idx].SetActive(true);


        // Button control
        buttonR.onClick.AddListener(Forward);
        buttonL.onClick.AddListener(Back);

        // Toggle control
        toggle.onValueChanged.AddListener(delegate {
            ToggleScreenOnly(toggle);
        });

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Forward()
    {
        m_Panoramas[m_Idx].SetActive(false);
        m_Idx += 1;
        if (m_Idx >= m_Panoramas.Count - 1)
        {
            m_Idx = m_Panoramas.Count - 1;
        }
        m_Panoramas[m_Idx].SetActive(true);
    }

    public void Back()
    {
        m_Panoramas[m_Idx].SetActive(false);
        m_Idx -= 1;
        if (m_Idx < 0)
        {
            m_Idx = 0;
        }
        m_Panoramas[m_Idx].SetActive(true);
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
