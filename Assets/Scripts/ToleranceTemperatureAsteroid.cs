using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ToleranceTemperatureAsteroid : MonoBehaviour
{
    public Asteroid asteroid;

    public float CalculateTemperature()
    {
        float sum = asteroid.materials.Sum(m => m.amount);
        List<float> temps = new();

        foreach (var m in asteroid.materials)
        {
            float t = m.material.baseTemperature * (1 + m.amount / sum);
            temps.Add(t);
        }

        return Mathf.Ceil(temps.Min() * 0.8f + temps.Average() * 0.2f);
    }
}

