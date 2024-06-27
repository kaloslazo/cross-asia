using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotation : MonoBehaviour
{
    public float SkyboxSpeed = 1.0f;

    void Start()
    {
        
    }

    void Update()
    {  
       // Modify when non sun or sunset exists when clouds are needed
       // RenderSettings.skybox.SetFloat("_Rotation", Time.time * SkyboxSpeed);
    }
}
