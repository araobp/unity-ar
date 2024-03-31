using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] Button buttonR;

    [SerializeField] Button buttonL;

    List<GameObject> panoramas = null;
    int idx = 0;

    // Start is called before the first frame update
    void Start()
    {
        panoramas = GameObject.FindGameObjectsWithTag("panorama").ToList();
        panoramas = panoramas.OrderBy(x => x.name).ToList();
        panoramas.ToList().ForEach(p =>
        {
            p.SetActive(false);
            Debug.Log(p.ToString());
        });
        panoramas[idx].SetActive(true);

        buttonR.onClick.AddListener(Forward);
        buttonL.onClick.AddListener(Back);

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Forward()
    {
        panoramas[idx].SetActive(false);
        idx += 1;
        if (idx >= panoramas.Count - 1)
        {
            idx = panoramas.Count - 1;
        }
        panoramas[idx].SetActive(true);
    }

    public void Back()
    {
        panoramas[idx].SetActive(false);
        idx -= 1;
        if (idx < 0)
        {
            idx = 0;
        }
        panoramas[idx].SetActive(true);
    }
}
