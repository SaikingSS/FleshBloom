using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightMenager : MonoBehaviour
{
    [Range(0, 24)] public float timeOfDay;
    public float inGameTimeSclae = 1f; //90min Day scale is "0.00444"
    public bool progressTime;
    //public bool startTimeTime;
    public float startTime = 12f;
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightPreset curPreset;


    private void Update()
    {
        if(curPreset == null )
        {
            return;
        }
        if (Application.isPlaying && progressTime)
        {
            timeOfDay += Time.deltaTime * inGameTimeSclae;
            timeOfDay %= 24;
            UpdateLighting(timeOfDay / 24f);
        }
        else
        {
            UpdateLighting(timeOfDay / 24f);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = curPreset.AmibientColor.Evaluate(timePercent);
        RenderSettings.fogColor = curPreset.FogColor.Evaluate(timePercent);

        if(directionalLight != null)
        {
            directionalLight.color = curPreset.DirectionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
    }



    private void OnValidate()
    {
        if(directionalLight != null)
        {
            return;
        }
        if(RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                if(light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
}
