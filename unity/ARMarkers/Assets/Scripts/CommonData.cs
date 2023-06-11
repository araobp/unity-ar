using UnityEngine;

public class CommonData: MonoBehaviour {

    [SerializeField]
    GameObject m_ARCamera;

    public GameObject ARCamera {
        get => m_ARCamera;
    }

    public float distance = 0F;

    private void Start()
    {
       
    }
}
