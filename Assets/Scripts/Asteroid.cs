using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public List<AsteroidMaterialEntry> materials;

    public float ToleranceTemperature()
    {
        float sum = materials.Sum(m => m.amount);
        List<float> temps = new();

        foreach (var m in materials)
        {
            float t = m.material.baseTemperature * (1 + m.amount / sum);
            temps.Add(t);
        }

        return Mathf.Ceil(temps.Min() * 0.8f + temps.Average() * 0.2f);
    }
}
