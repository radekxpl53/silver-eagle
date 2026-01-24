using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Asteroid : MonoBehaviour
{
    public List<AsteroidMaterialEntry> materials;
    public TextMeshProUGUI info;

    private List<float> GetWeightedTemps()
    {
        float sum = materials.Sum(m => m.amount);
        return materials
            .Select(m => m.material.baseTemperature * (1 + m.amount / sum))
            .ToList();
    }


    public float ToleranceTemperature()
    {
        var temps = GetWeightedTemps();
        return Mathf.Ceil(temps.Min() * 0.8f + temps.Average() * 0.2f);
    }

    public float CalculateTemperature()
    {
        var temps = GetWeightedTemps();
        return Mathf.Ceil(temps.Max() * 0.8f + temps.Average() * 0.2f);
    }


    public void showInfoAsteroid(){
        string textInfo = $"Temperatura: {CalculateTemperature()} \u00B0C" +
        $"\nTolerancja: {ToleranceTemperature()} \u00B0C" +
        $"\nSurowce:\n";
        foreach(var m in materials){
            textInfo += $"{m.material.materialName} - {m.amount}\n";
        }
        info.text = textInfo;
    }

    void Start(){
        showInfoAsteroid();
    }
}
