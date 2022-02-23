using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLightManager : MonoBehaviour
{
    private static MainLightManager mainLight;
    public static MainLightManager Instance
    {
        get 
        {
            if (mainLight == null)
                mainLight = GameObject.FindObjectOfType<MainLightManager>();
            return mainLight;
        }
    }

    private void Awake()
    {
        if (mainLight == null)
        {
            mainLight = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (mainLight != this)
            Destroy(this.gameObject);
    }
}
