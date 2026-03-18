using System.Collections.Generic;
using UnityEngine;

public static class MiningData {
    // Skrypt trzyma dane asteroidy, ¿eby zmieaæ scene na zbieranie
    public static List<ResourceStack> currentAsteroidLoot;
    public static Asteroid currentAsteroidObject;

    public static AreaSpawnerManager currentManager;
    public static BeltSavedData currentBelt;
    public static GameObject currentArea;
}