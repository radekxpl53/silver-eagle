# Silver Eagle — 2 prompty równoległe

**Bez Steama** — build PC/itch, ale jakość jak pod premierę (save, tutorial, UI, content, build script).

Skopiuj **PROMPT A** do jednej sesji, **PROMPT B** do drugiej. Startujcie jednocześnie.
Kontrakty wspólne (nie zmieniaj bez syncu z drugą osobą):
- `Assets/Scripts/Core/GameEvents.cs` — eventy gry
- `Assets/Scripts/ScriptableObjects/SectorDefinition.cs` — dane sektora
- `Assets/Scripts/ScriptableObjects/UpgradeDefinition.cs` — dane ulepszeń
- `Assets/Scripts/Core/SaveDataJSON.cs` — schema zapisu (wersja w polu `saveVersion`)

---

## PROMPT A — Silnik (gameplay, ekonomia, walka, save)

```
Kontekst: Unity projekt Silver Eagle (D:\GitHub\silver-eagle). Space mining/combat. 
Pracujesz RÓWNOLEGLE z osobą B (UI/content/polish). Nie edytujesz prefabów kokpitu, 
Scenes/GameManager.unity UI refs, Scripts/UI/, ScriptableObjects/Sectors/*.

CEL SESJI: Zamknąć backend grywalnej pętli — save, sklep, mining, walka, ekonomia.
Wykonaj WSZYSTKO poniżej. Nie pytaj — implementuj. Na końcu podaj listę eventów 
które dodałeś (B na nich polega).

=== KROK 0: Kontrakty (zrób PIERWSZY, zanim cokolwiek innego) ===

1. Utwórz Assets/Scripts/Core/GameEvents.cs:
   - static events: OnSectorEntered(Vector2Int, SectorDefinition), OnCreditsChanged(float), 
     OnDebtChanged(float), OnUpgradePurchased(string upgradeId), OnCombatStarted, 
     OnCombatEnded, OnHullDamaged(float, Vector3), OnPlayerDestroyed, OnEnemyKilled(EnemyAI)
   - metody Trigger* dla każdego

2. Utwórz Assets/Scripts/ScriptableObjects/UpgradeDefinition.cs (CreateAssetMenu):
   - upgradeId, displayName, description, creditCost, requiredSectorStage
   - effectType enum: EngineThrust, CargoCapacity, MaxHP, Shield, MilitaryScanner, 
     LaserMaxTemp, DrillDurability, AsteroidReport, SectorScanInfo, FastTravel, 
     RepairDrones, RepairKits
   - effectValue (float)

3. Rozszerz SaveDataJSON — saveVersion=1, zapisuj: sector grid state, purchasedUpgrades[], 
   credits, debt, player position/sector, cargo, ship HP/energy. Load z walidacją File.Exists + try/catch.
   Publiczne SaveData() / LoadData() / HasSaveFile() — B podłączy pod PauseMenu.

=== KROK 1: Blockery ===

- Skopiuj FMOD banki z silver-eagle-audio/Build/Desktop/ do Assets/StreamingAssets/
- Fix Sector.cs:28 — użyj instancji repairStation, nie prefabu
- Usuń UnityEditor z Assets/Scripts/Environment/Asteroid.cs (tylko #if UNITY_EDITOR jeśli musi)
- MainMenuManager.OnNewGameClicked() → wywołaj Restart.ResetData() przed load sceny
- DeveloperConsole, InventoryTester → owiń w #if DEVELOPMENT_BUILD
- Usuń EnemyAiScene z EditorBuildSettings (scena dev, nie dla gracza)

=== KROK 2: Ekonomia ===

- ShopSystem.cs w Scripts/Economy & Inventory/:
  - TryPurchase(UpgradeDefinition), CanAfford, ApplyEffect na ShipStats/PlayerData/ChunkManager
  - 12 UpgradeDefinition assetów w Resources/Upgrades/ (po jednym per effectType z GDD)
- EconomyManager: dodaj debt (float), AddDebt, PayDebt, SpendCredits zwraca bool
- Po sprzedaży w SellSystem — trigger OnCreditsChanged
- FactionMissionSystem.cs (prosty): ScriptableObject misja (deliver X resource Y), 
  nagroda kredyty; 3 przykładowe misje w Resources/Missions/

=== KROK 3: Mining (dokończ wg GDD) ===

W MiningGame.cs:
- Przed startem minigry: ekran analizy próbki (prosty Debug.Log / tymczasowy UI w scenie 
  MiningScene — B podmieni na diegetyczny). Pokaż: skład %, średnia temp, LOW/MID/HIGH.
- Niestabilność: poza strefą tolerancji +10%/s (mały błąd), +30%/s (temp > maxOptimal*1.2)
- 10-89% niestabilności: yieldMultiplier spada (Topnienie Surowca) — już częściowo jest, dopracuj
- >=90%: ThermalShock → przerwij mining, DamageCollision na graczu (odległość od asteroidy), 
  wywołaj OnHullDamaged
- ChunkManager/AreaSpawnerManager: hybrid spawn 70% leadingStage / 20% stage-1 / 10% stage+1 
  (clamp 0-4). Respawn pasa po 80% wykopania — zweryfikuj i napraw jeśli nie działa.

SectorDefinition.leadingStage — czytaj z SO jeśli istnieje, fallback na sectorStage z ChunkManager.

=== KROK 4: Walka ===

- ShipStats/PlayerData: shield jako osobna warstwa (CurrentShield, MaxShield, AbsorbDamage)
- EnemyAI: zmień flee na 70% HP wroga (nie gracza). Przy flee → drop loot (ResourceStack[] 
  z puli stage sektora) i OnEnemyKilled
- CombatPromptSystem.cs: przy CustomRadarSystem detection — log/UI-stub "Wróg — walcz/uciekaj" 
  (B podłączy UI). GameState.Fighting już jest.
- HeavyKineticLauncher = Canon Laser (zostaw). Dodaj PlasmaCannon.cs — wolniejszy fire rate, 
  większe dmg, overheat po 3 strzałach
- Turret na graczu (mirror Turret.cs z wroga) — podłącz do Statek.prefab jeśli możliwe bez 
  ruszania kokpitu UI; jeśli nie, zostaw komponent gotowy do podpięcia
- CustomSectorSpawner: maxActiveEnemies skaluj z sector riskLevel (0-1 wróg, 2-2, 3-4 → 3)
- Object pool dla HeavyKineticProjectile (ProstPool.cs wystarczy)

=== KROK 5: Świat + progresja ===

- FastTravelSystem.cs: jeśli PlayerData.fastTravel, skok do sektora z repair station za koszt 
  energy. Bez upgrade — disabled.
- RepairDrones: jeśli upgrade, Heal 5HP/s poza walką
- RepairKit: consumable w cargo, UseRepairKit() → +20% HP
- ShipController: przy cargo > 80% max → maxOverallSpeed * 0.7, fuel drain * 1.5
- BaseDropZone: pełna naprawa HP+energy za kredyty. Death: GameManager.TriggerGameOver → 
  RespawnAtBase odejmuje 30% credits jako koszt naprawy, usuwa 20% cargo losowo

=== KROK 6: Testy ===

- Assets/Tests/PlayMode/SmokeTest.cs — jeden test: scena GameManager się ładuje bez errorów
- .github/workflows/unity-build.yml — compile check (game-ci lub dotnet build jeśli brak license)

=== NIE RUSZAJ ===
- Prefabs/Cockpit/, Scripts/UI/*, ScriptableObjects/Sectors/* (to robi B)
- Scenes/GameManager.unity (tylko jeśli KONIECZNE dla AISceneBootstrap — minimalny diff)

=== ACCEPTANCE CRITERIA ===
[ ] Gra kompiluje się bez błędów
[ ] Save/Load zapisuje i wczytuje pozycję + kredyty + upgrades
[ ] Kupno upgrade w kodzie (DevConsole lub test) zmienia stat statku
[ ] Mining: Thermal Shock zadaje dmg przy >=90% niestabilności
[ ] Wróg ucieka przy 70% HP i dropuje loot
[ ] FMOD banki w StreamingAssets
[ ] GameEvents.cs istnieje z wszystkimi Trigger*

Na końcu wypisz: lista nowych plików, lista eventów, co B musi podłączyć w UI.
```

---

## PROMPT B — Produkt (sektory, UI, narracja, polish)

```
Kontekst: Unity projekt Silver Eagle (D:\GitHub\silver-eagle). Space mining/combat.
Pracujesz RÓWNOLEGLE z osobą A (gameplay/save/combat). Nie edytujesz: Scripts/Combat/*, 
Scripts/Core/SaveDataJSON.cs, Scripts/Core/ChunkManager.cs logiki, ShopSystem, MiningGame logiki.

CEL SESJI: Content 36 sektorów + diegetyczny UI + narracja + polish jak pod komercyjną premierę
(bez Steam/SDK — gra idzie jako build PC/itch, ale jakość ma być „ship-ready”).
Wykonaj WSZYSTKO poniżej. Mockuj dane od A jeśli jeszcze nie ma — podłączysz po merge.

=== KROK 0: Kontrakty (zrób PIERWSZY) ===

1. Utwórz Assets/Scripts/ScriptableObjects/SectorDefinition.cs (CreateAssetMenu "SilverEagle/Sector"):
   - gridPosition (Vector2Int)
   - sectorName, territory enum (Cermandia, Ariandia, Rubieze, Tranzyt)
   - leadingStage (0-4), riskLevel (0-4)
   - jurisdictionText, profileText, riskAnalysisText, oreForecastText
   - crewNote (TextArea) — jedna notatka (Korey/Eliana/Buford/Młody)
   - crtLogEntries (string[])
   - miningComposition (ResourceStack[] lub string z %)
   - miningThreatLevel enum (Low, Mid, High, Critical)
   - miningThermalHint, miningSafetyMessage
   - patrolPresence bool, shopTaxPercent float (0 lub 45 dla A4)

2. Utwórz SectorContentDatabase.cs (Singleton/ScriptableObject):
   - SectorDefinition GetSector(Vector2Int grid)
   - Ładuje z Resources/Sectors/ lub tablicy w Inspectorze

3. Utwórz IDiegeticDisplay.cs:
   - void SetCredits(float), SetHP(float, float), SetEnergy(float, float), SetCargo(float, float)
   - void ShowSectorBriefing(SectorDefinition), ShowCRTLog(string[]), ShowNotification(string, Color)

=== KROK 1: 36 sektorów z Sectors.docx ===

Repo ma plik Sectors.docx w root. Wyciągnij dane i stwórz 36 assetów w Assets/Resources/Sectors/:
- Nazewnictwo: Sector_A6, Sector_B5, ... Sector_F1
- Użyj danych z doca: A6=Keimos/start, B5=Rubieże etap0, C5=piraci etap2, D5=Rod etap3, 
  E2=Uran etap4, F5=Mrainesden, A3=SeinPfeiser, A4=Erad'os (podatek 45%), itd.
- Mapa 6x6: wiersz 1=A (Rubieże/deep), wiersz 6=bezpieczny start Cermandia/Ariandia
- Jeśli nie możesz przeczytać docx — użyj WebFetch/raw albo PowerShell ZipFile na word/document.xml

Minimum 6 sektorów MUSI mieć pełne lore (A6, B5, C5, D5, F5, E2). Reszta — szablon z poprawnym 
stage/risk/territory, lore placeholder OK.

=== KROK 2: Diegetyczny UI ===

Cel GDD: ZERO screen-space HUD. Wszystko na monitorach w statku.

- CockpitDisplayManager.cs implementuje IDiegeticDisplay
- Podłącz do istniejących mesh/monitów w Statek.prefab LUB stwórz Prefabs/Cockpit/MonitorCanvas.prefab 
  (World Space Canvas na quadzie) z tekstem TMPro
- Przenieś dane z UI_ShipStatus na monitory (nie usuwaj UI_ShipStatus jeszcze — wyłącz w Inspectorze)
- MapDisplay: przy kliknięciu sektora pokaż briefing (SectorDefinition) w panelu InfoPanel
- Subskrybuj GameEvents (jeśli istnieje — jeśli nie, użyj ChunkManager + FindObjectOfType fallback):
  - OnSectorEntered → ShowSectorBriefing + ShowCRTLog
  - OnCreditsChanged → SetCredits
  - OnHullDamaged → flash czerwony na monitorze

Holograficzny stół nawigacyjny:
- NavTableInteract.cs na MapDisplay lub osobny collider — E do otwarcia, kursor na mapie wybiera sektor
- Przed wylotem: FuelVsCargoPanel (prosty) — wybór % paliwa vs cargo capacity na następny lot

=== KROK 3: Narracja ===

- NarrativeDirector.cs — subskrybuje OnSectorEntered, losuje crewNote jeśli puste, 
  wywołuje CockpitDisplayManager.ShowNotification
- CrewBarks.cs — static string[] per postać (Korey, Eliana, Buford, Młody), 
  GetBark(CrewMember, EventType) — 3-5 barków per postać per event (sectorEnter, combat, lowFuel, debt)
- LoadingScreenTips.cs — 20 tipów z SE - LORE.docx / GDD, losowy przy ładowaniu sceny

=== KROK 4: Tutorial ===

- TutorialManager.cs — flaga PlayerPrefs "tutorialDone"
- Sektor A6 (lub 0,0): 5 kroków sekwencyjnych:
  1. "WASD lot" — wykryj ruch
  2. "Podleć do asteroidy, E" — PlayerInteract
  3. "Ukończ minigrę" — OnMiningComplete event (dodaj w GameEvents jeśli brak — stub)
  4. "Sprzedaj C" — SellZone
  5. "Otwórz mapę M" — MapToggle
- Overlay z TMPro (może być screen-space TYLKO dla tutorialu)

=== KROK 5: Audio UX (FMOD już jest — tylko parametry) ===

- MusicStateController.cs: GameState.Exploration/Mining → param MusicState=0, Fighting/GameOver → =1
- HullAudioController.cs: subskrybuje OnHullDamaged → FMOD one-shot + param HullStress spike decay
- Nie ruszaj banków FMOD — to robi A

=== KROK 6: Polish premiery (bez Steam) ===

- Localization: Assets/Localization/Strings.csv — kolumny Key,pl,en.
  Minimum: 36 sector names + 6 tutorial steps + menu + combat/mining komunikaty.
  LocalizationManager.cs GetString(key) — fallback na pl
- MainMenu: przycisk „Kontynuuj” tylko gdy save istnieje; „Nowa gra” z potwierdzeniem nadpisania
- PauseMenu: sekcja sterowania (lista klawiszy z GDD), przycisk „Zapisz grę” → wywołaj SaveDataJSON.SaveData()
- DeathScreen: tekst wg GDD (wystrzelenie, koszt naprawy, utrata ładunku) — podłącz pod eventy od A
- CreditsScreen.cs — scroll z autorami z GDD + podziękowania; dostęp z MainMenu
- SettingsMenu: osobne slidery Music/SFX (FMOD busy jeśli są, inaczej master)
- README.md: jak zbudować, sterowanie, wymagania, znane bugi, struktura folderów
- Build script: Scripts/Editor/BuildPlayer.cs — menu „SilverEagle/Build Windows“ → folder Builds/Win64/
- Usuń/wyłącz dev: EnemyAiScene poza buildem (EditorBuildSettings), Console tylko DEVELOPMENT_BUILD

=== KROK 7: Territory polish ===

- SectorTerritoryRules.cs: przy OnSectorEntered:
  - Cermandia/Ariandia + patrolPresence: GameManager.ShowSectorInfo "Patrol w pobliżu", 
    ewentualnie ogranicz max speed (wywołaj ShipController.SetSpeedLimit(0.3f) — dodaj metodę jeśli brak)
  - Rubieze: "Brak ochrony prawnej. SOS niedostępne."
  - Ariandia: "Wyłącz uzbrojenie w strefie kontrolnej" — log warning jeśli Fighting w sektorze Ariandia

=== NIE RUSZAJ ===
- MiningGame.cs logiki, EnemyAI.cs, ShopSystem.cs, SaveDataJSON.cs, HeavyKineticProjectile

=== ACCEPTANCE CRITERIA ===
[ ] 36 SectorDefinition assetów w Resources/Sectors/
[ ] Klik na mapie pokazuje briefing sektora z nazwą i lore
[ ] Monitory w kokpicie pokazują HP/energy/cargo/credits (world space)
[ ] Wejście w sektor wyświetla CRT log
[ ] Tutorial 5 kroków działa w sektorze startowym
[ ] Strings.csv ma min. 50 kluczy pl+en
[ ] PauseMenu ma „Zapisz grę” i działa
[ ] Build Windows z menu Editor generuje playable .exe
[ ] Gra kompiluje się bez błędów

Na końcu wypisz: lista assetów sektorów, lista prefabów UI, jakie GameEvents wymagasz od A.
```

---

## Po obu promptach — 15 min sync

1. A merge → B rebase (lub odwrotnie)
2. Sprawdź czy `GameEvents.cs` + `SectorDefinition.cs` + `UpgradeDefinition.cs` się zgadzają
3. B podłącza `CockpitDisplayManager` do eventów z A
4. A podłącza `ChunkManager` do `SectorContentDatabase.GetSector()`
5. Playtest: nowa gra → A6 → kop → sprzedaj → kup upgrade → wleć w B5 → walka

**Konflikty git:** jedyny wspólny plik do ręcznego merge — `GameEvents.cs`. Reszta rozłączona.
