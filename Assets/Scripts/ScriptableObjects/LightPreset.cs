using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Light Preset")]
[System.Serializable]
public class LightPreset : ScriptableObject
{
    public Gradient AmibientColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;
}
