using System.IO;
using UnityEditor;
using UnityEngine;

public class SectorGenerator : EditorWindow
{
    [MenuItem("SilverEagle/Generate Sectors")]
    public static void GenerateSectors()
    {
        string dir = "Assets/Resources/Sectors";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        char[] rows = { 'A', 'B', 'C', 'D', 'E', 'F' };
        
        for (int r = 0; r < 6; r++)
        {
            char rowChar = rows[r];
            for (int col = 1; col <= 6; col++)
            {
                string assetName = $"Sector_{rowChar}{col}";
                string assetPath = $"{dir}/{assetName}.asset";

                SectorDefinition sector = AssetDatabase.LoadAssetAtPath<SectorDefinition>(assetPath);
                bool isNew = false;
                if (sector == null)
                {
                    sector = ScriptableObject.CreateInstance<SectorDefinition>();
                    isNew = true;
                }

                // Grid position: row A is 0, F is 5. Col 1 is 0, 6 is 5.
                sector.gridPosition = new Vector2Int(r, col - 1);
                sector.sectorName = GetSectorName(rowChar, col);
                sector.territory = GetSectorTerritory(rowChar, col);
                
                // stage and risk mapping
                sector.leadingStage = GetSectorStage(rowChar, col);
                sector.riskLevel = GetSectorRisk(rowChar, col);

                sector.jurisdictionText = GetSectorJurisdiction(sector.territory);
                sector.profileText = GetSectorProfile(rowChar, col);
                sector.riskAnalysisText = GetSectorRiskAnalysis(sector.riskLevel);
                sector.oreForecastText = GetSectorOreForecast(rowChar, col);
                
                sector.crewNote = GetSectorCrewNote(rowChar, col);
                sector.crtLogEntries = GetSectorCrtLogEntries(rowChar, col);
                
                // Mining setup
                sector.miningComposition = GetMiningComposition(rowChar, col);
                sector.miningThreatLevel = GetMiningThreatLevel(sector.riskLevel);
                sector.miningThermalHint = GetMiningThermalHint(rowChar, col);
                sector.miningSafetyMessage = GetMiningSafetyMessage(sector.miningThreatLevel);
                
                // Patrols and Tax
                sector.patrolPresence = GetPatrolPresence(rowChar, col, sector.territory);
                sector.shopTaxPercent = (rowChar == 'A' && col == 4) ? 45f : 0f;

                if (isNew)
                {
                    AssetDatabase.CreateAsset(sector, assetPath);
                }
                else
                {
                    EditorUtility.SetDirty(sector);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Generated 36 Sectors in Resources/Sectors!");
    }

    private static string GetSectorName(char row, int col)
    {
        string id = $"{row}{col}";
        switch (id)
        {
            case "A6": return "Keimos";
            case "B5": return "Rubieze Borderlands";
            case "C5": return "Pirate Cove";
            case "D5": return "The Rod";
            case "E2": return "Uranus Outpost";
            case "F5": return "Mrainesden";
            case "A3": return "Sein Pfeiser";
            case "A4": return "Erad'os";
            default: return $"Sector {id}";
        }
    }

    private static Territory GetSectorTerritory(char row, int col)
    {
        // row A, B = Rubieze
        // row C, D = Tranzyt
        // row E = Ariandia
        // row F = Cermandia
        if (row == 'A' || row == 'B') return Territory.Rubieze;
        if (row == 'C' || row == 'D') return Territory.Tranzyt;
        if (row == 'E') return Territory.Ariandia;
        return Territory.Cermandia;
    }

    private static int GetSectorStage(char row, int col)
    {
        // Start: row F (stage 0), E (stage 1), D (stage 2), C (stage 3), B/A (stage 4)
        // Adjust for start sector A6 which is Keimos/start (leadingStage 0, riskLevel 0)
        if (row == 'A' && col == 6) return 0;
        
        switch (row)
        {
            case 'F': return 0;
            case 'E': return 1;
            case 'D': return 2;
            case 'C': return 3;
            default: return 4;
        }
    }

    private static int GetSectorRisk(char row, int col)
    {
        if (row == 'A' && col == 6) return 0;
        
        switch (row)
        {
            case 'F': return 0;
            case 'E': return 1;
            case 'D': return 2;
            case 'C': return 3;
            default: return 4;
        }
    }

    private static string GetSectorJurisdiction(Territory territory)
    {
        switch (territory)
        {
            case Territory.Cermandia: return "Cermandia High Imperial Council";
            case Territory.Ariandia: return "Ariandia Corporate Coalition";
            case Territory.Rubieze: return "No Legal Jurisdiction (Rubieze)";
            case Territory.Tranzyt: return "Joint Transit Authority";
            default: return "Unknown Jurisdiction";
        }
    }

    private static string GetSectorProfile(char row, int col)
    {
        string id = $"{row}{col}";
        if (id == "A6") return "Safe starting system. Rich in basic silicates and iron ores.";
        if (id == "B5") return "Outer rim border region, high asteroid density.";
        if (id == "C5") return "Frequent raider sightings and unstable debris fields.";
        if (id == "D5") return "Core mining station zone. Rich heavy metals.";
        if (id == "E2") return "Deep gas giant orbit. Dangerous radioactive zones.";
        if (id == "F5") return "Cermandia military outpost. Restricted airspace.";
        return $"Sector {id} exploration zone.";
    }

    private static string GetSectorRiskAnalysis(int riskLevel)
    {
        switch (riskLevel)
        {
            case 0: return "Risk Level: Safe. High security patrol presence.";
            case 1: return "Risk Level: Low. Minimal local pirate groups.";
            case 2: return "Risk Level: Moderate. Travel in armed groups recommended.";
            case 3: return "Risk Level: High. Hostile scanning and frequent combat.";
            case 4: return "Risk Level: Extreme. Full combat capability required.";
            default: return "Risk Level: Unknown.";
        }
    }

    private static string GetSectorOreForecast(char row, int col)
    {
        if (row == 'F' || row == 'E') return "Iron Ore (80%), Silicates (20%)";
        if (row == 'D' || row == 'C') return "Copper Ore (50%), Platinum (30%), Iron (20%)";
        return "Uranium (40%), Gold (30%), Heavy Isotopes (30%)";
    }

    private static string GetSectorCrewNote(char row, int col)
    {
        string id = $"{row}{col}";
        if (id == "A6") return "Korey: 'Everything is quiet here. Best place to calibrate the mining lasers.'";
        if (id == "B5") return "Eliana: 'Keep an eye on the sensors. Rubieze isn't known for its hospitality.'";
        if (id == "C5") return "Buford: 'Pirates love this sector. Double check shield capacity.'";
        if (id == "D5") return "Młody: 'The mineral yields here are huge. We could make a fortune!'";
        if (id == "E2") return "Korey: 'Radiation levels are high. Don't linger near the gas giant.'";
        if (id == "F5") return "Eliana: 'Mrainesden garrison is always watching. Keep weapons hot anyway.'";
        return "Crew: 'Standard sector protocol applies here.'";
    }

    private static string[] GetSectorCrtLogEntries(char row, int col)
    {
        return new string[]
        {
            $"[LOG] Entering Sector {row}{col}",
            $"[LOG] Security protocols: {GetSectorTerritory(row, col)}",
            $"[LOG] Sensor sweep: Scanning local celestial bodies..."
        };
    }

    private static ResourceStack[] GetMiningComposition(char row, int col)
    {
        // Standard resources
        return new ResourceStack[0];
    }

    private static MiningThreatLevel GetMiningThreatLevel(int riskLevel)
    {
        switch (riskLevel)
        {
            case 0: return MiningThreatLevel.Low;
            case 1: return MiningThreatLevel.Mid;
            case 2: return MiningThreatLevel.Mid;
            case 3: return MiningThreatLevel.High;
            default: return MiningThreatLevel.Critical;
        }
    }

    private static string GetMiningThermalHint(char row, int col)
    {
        if (row == 'A' || row == 'B') return "Thermal levels unstable. Danger of overheating.";
        return "Thermal levels nominal.";
    }

    private static string GetMiningSafetyMessage(MiningThreatLevel threat)
    {
        switch (threat)
        {
            case MiningThreatLevel.Low: return "Safety rating: EXCELLENT.";
            case MiningThreatLevel.Mid: return "Safety rating: MODERATE. Expect friction.";
            case MiningThreatLevel.High: return "Safety rating: DANGEROUS.";
            default: return "Safety rating: HAZARDOUS. Retreat option advised.";
        }
    }

    private static bool GetPatrolPresence(char row, int col, Territory territory)
    {
        if (row == 'A' && col == 4) return false; // Erad'os has no patrol
        return territory == Territory.Cermandia || territory == Territory.Ariandia;
    }
}
