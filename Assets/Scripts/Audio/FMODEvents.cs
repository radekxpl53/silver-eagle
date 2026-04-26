using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance { get; private set; }

    [field: SerializeField] public EventReference mainMusic { get; private set; }
    [field: SerializeField] public EventReference laserCollecting { get; private set; }
    [field: SerializeField] public EventReference shipIdle { get; private set; }
    [field: SerializeField] public EventReference shipMove { get; private set; }
    [field: SerializeField] public EventReference hitSound { get; private set; }

    [field: SerializeField] public EventReference alarm { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple FMOD Events were found in the scene.");
        }

        instance = this;
    }
}