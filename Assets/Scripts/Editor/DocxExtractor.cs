using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class DocxExtractor : EditorWindow
{
    [MenuItem("SilverEagle/Generate Sectors and Lore")]
    public static void GenerateSectorsAndLore()
    {
        string sectorsPath = Path.Combine(Directory.GetCurrentDirectory(), "Sectors.docx");
        string lorePath = Path.Combine(Directory.GetCurrentDirectory(), "SE - LORE.docx");

        if (!File.Exists(sectorsPath))
        {
            Debug.LogError("Could not find Sectors.docx at " + sectorsPath);
            return;
        }

        string sectorsText = ExtractTextFromDocx(sectorsPath);
        string loreText = File.Exists(lorePath) ? ExtractTextFromDocx(lorePath) : "";

        Debug.Log("Sectors docx length: " + sectorsText.Length);
        Debug.Log("Lore docx length: " + loreText.Length);

        // We will write these to temporary files in the project so the AI or developer can inspect them
        File.WriteAllText("Assets/Sectors_Extracted.txt", sectorsText);
        File.WriteAllText("Assets/Lore_Extracted.txt", loreText);

        GenerateSectorAssets(sectorsText, loreText);
        AssetDatabase.Refresh();
        Debug.Log("Sector generation completed successfully!");
    }

    private static string ExtractTextFromDocx(string zipPath)
    {
        using (ZipArchive archive = ZipFile.OpenRead(zipPath))
        {
            ZipArchiveEntry entry = archive.GetEntry("word/document.xml");
            if (entry == null) return "";

            using (Stream stream = entry.Open())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

                XmlNodeList paragraphNodes = doc.SelectNodes("//w:p", nsmgr);
                List<string> paragraphs = new List<string>();

                foreach (XmlNode pNode in paragraphNodes)
                {
                    XmlNodeList textNodes = pNode.SelectNodes(".//w:t", nsmgr);
                    string pText = "";
                    foreach (XmlNode tNode in textNodes)
                    {
                        pText += tNode.InnerText;
                    }
                    if (!string.IsNullOrWhiteSpace(pText))
                    {
                        paragraphs.Add(pText);
                    }
                }
                return string.Join("\n", paragraphs);
            }
        }
    }

    private static void GenerateSectorAssets(string sectorsText, string loreText)
    {
        // Ensure folder exists
        string dir = "Assets/Resources/Sectors";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        // For now, let's parse or generate default definitions.
        // We'll parse the rows/columns A-F and 1-6.
        // Grid: row 1=A (deep Rubieze), row 6=F (safe start Cermandia/Ariandia)
        // Or letter=row, number=col, etc. The GDD says: "Mapa 6x6: wiersz 1=A (Rubieże/deep), wiersz 6=bezpieczny start Cermandia/Ariandia"
        // Let's create all 36 sectors: Sector_A1 to Sector_F6 (or Sector_A1 to Sector_F6).
        // Let's map letters A-F to rows 0-5.
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

                // Grid position
                sector.gridPosition = new Vector2Int(r, col - 1);
                sector.sectorName = GetSectorName(rowChar, col);
                sector.territory = GetSectorTerritory(rowChar, col);
                sector.leadingStage = GetSectorStage(rowChar, col);
                sector.riskLevel = GetSectorRisk(rowChar, col);
                
                // Content defaults
                sector.jurisdictionText = GetSectorJurisdiction(sector.territory);
                sector.profileText = $"Profile of Sector {rowChar}{col}";
                sector.riskAnalysisText = $"Risk Analysis for {rowChar}{col}";
                sector.oreForecastText = "Iron, Copper, Silicates";
                sector.crewNote = $"Crew report on Sector {rowChar}{col}...";
                sector.crtLogEntries = new string[] {
                    $"[CRT LOG] Entered Sector {rowChar}{col}",
                    $"[CRT LOG] Scan complete: No immediate threats detected."
                };
                sector.miningComposition = new ResourceStack[0];
                sector.miningThreatLevel = MiningThreatLevel.Low;
                sector.miningThermalHint = "Thermal levels stable.";
                sector.miningSafetyMessage = "Safety rating: Nominal.";
                sector.patrolPresence = (sector.territory == Territory.Cermandia || sector.territory == Territory.Ariandia);
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
    }

    private static string GetSectorName(char row, int col)
    {
        string id = $"{row}{col}";
        switch (id)
        {
            case "A6": return "Keimos";
            case "B5": return "Outer Rim Borderlands";
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
        if (row == 'A' || row == 'B') return Territory.Rubieze;
        if (row == 'C' || row == 'D') return Territory.Tranzyt;
        if (row == 'E') return Territory.Ariandia;
        return Territory.Cermandia;
    }

    private static int GetSectorStage(char row, int col)
    {
        if (row == 'F') return 0;
        if (row == 'E') return 1;
        if (row == 'D') return 2;
        if (row == 'C') return 3;
        return 4;
    }

    private static int GetSectorRisk(char row, int col)
    {
        if (row == 'F') return 0;
        if (row == 'E') return 1;
        if (row == 'D') return 2;
        if (row == 'C') return 3;
        return 4;
    }

    private static string GetSectorJurisdiction(Territory territory)
    {
        switch (territory)
        {
            case Territory.Cermandia: return "Cermandia Empire Jurisdiction";
            case Territory.Ariandia: return "Ariandia Corporate Starpact";
            case Territory.Rubieze: return "None (Lawless Territory)";
            case Territory.Tranzyt: return "Transit Zone High Command";
            default: return "Unknown";
        }
    }
}
