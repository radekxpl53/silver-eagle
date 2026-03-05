using System;
using UnityEngine;

[Serializable]
public class ResourceDefinition
{
    public string Name;
    public int Stage;
    public float optimalTemp;
    public float tolerance;
    public Sprite Icon;
    public float[] weightsPerStage = new float[5];
} 

