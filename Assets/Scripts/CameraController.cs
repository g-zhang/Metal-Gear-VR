using UnityEngine;
using System.Collections;

public enum CameraType
{
    overhead, //overhead camera for when snake is running state
    fpv, //first person camera used for first person view
    crawl, //first person camera used for when crawling under obstacles
    size
};

public class CameraController : MonoBehaviour {
    //this singleton class controls which cameras get activated/deactivated

    static public CameraController S;

    public bool SeeScriptForCameraOrder;
    public GameObject[] cameras = new GameObject[(int)CameraType.size];

    void Awake()
    {
        S = this;
    }

    public void SwitchCameraTo(CameraType cam)
    {
        for(int i = 0; i < (int)CameraType.size; i++)
        {
            if( i == (int)cam)
            {
                cameras[i].GetComponent<Camera>().enabled = true;
            } else
            {
                cameras[i].GetComponent<Camera>().enabled = false;
            }
        }
    }
}
