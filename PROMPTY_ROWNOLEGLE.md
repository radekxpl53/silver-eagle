# Silver Eagle — 2 prompty dla AI (równoległa praca)

**Projekt:** Unity 6000.3.1f1 · space mining & combat · build PC (bez Steam)  
**Plan:** patrz `plan.md`  
**Dokumenty:** `GDD SilverEagle (1).pdf`, `Sectors.docx`, `SE - LORE.docx`

Skopiuj **cały PROMPT A** do jednej sesji Cursor/AI (osoba z dostępem do plików — silnik).  
Skopiuj **cały PROMPT B** do drugiej sesji (osoba — produkt/UI/content).  
Startujcie równolegle na branchach `a/*` i `b/*`.

---

## Kontrakty wspólne (sync przed edycją!)

| Plik | Kto edytuje |
|------|-------------|
| `Assets/Scripts/Core/GameEvents.cs` | **A** dodaje eventy · **B** tylko subskrybuje |
| `Assets/Scripts/ScriptableObjects/SectorDefinition.cs` | **B** (pola lore) · **A** tylko czyta |
| `Assets/Scripts/ScriptableObjects/UpgradeDefinition.cs` | **A** (enum/efekty) · **B** assety + UI |
| `Assets/Scripts/Core/SaveDataJSON.cs` + `GameSaveData.cs` | **A** schema · **B** podpina menu |

**Poza scope obu promptów (Faza 3):** loading screen, pełny async skok mapy (`JumpToSector` + teleport), procedura startu silników, tryb bojowy WoWS.

---

## Co już jest (nie twórz od zera — tylko dokończ)

- `GameEvents.cs`, `UpgradeDefinition.cs`, `SectorDefinition.cs`, `SaveDataJSON.cs` — istnieją
- Lot (`ShipController` + EnemyAI-style pitch/roll), mining minigra, thermal shock, AI, save backend
- 36 assetów sektorów w `Resources/Sectors/` (lore głównie placeholder)
- Szkielet: `CockpitDisplayManager`, `TutorialManager`, `NarrativeDirector`, `SectorTerritoryRules`
- Eventy tutoriala: `OnMiningComplete`, `OnResourcesSold`, `OnMapToggled` — już emitowane
- `SmokeTest`, `BuildPlayer.cs`, asmdef (`SilverEagle`, `SilverEagle.Editor`, PlayMode tests)

---

## PROMPT A — Silnik (gameplay, ekonomia, walka, save)

```
Jesteś AI z pełnym dostępem do plików. Projekt: Silver Eagle
(c:\Users\kacpe\Documents\silver-eagle). Unity 6000.3.1f1.

Pracujesz RÓWNOLEGLE z PROMPT B (UI/content). Przeczytaj plan.md przed startem.
Wykonaj WSZYSTKO poniżej. Implementuj — nie pytaj o zgodę. Na końcu wypisz:
- listę zmienionych plików
- nowe eventy w GameEvents (jeśli dodałeś)
- co B musi podłączyć w UI

=== ZASADY ===
- Edytujesz: Combat/, Core/ (logika), Economy & Inventory/, Player/, Environment/ (logika), Editor/ (tylko jeśli potrzeba assetów runtime)
- NIE edytujesz: Scripts/UI/*, Resources/Sectors/*.asset, prefabów kokpitu, Scenes/* (minimalny diff tylko jeśli blocker)
- Matchuj styl istniejącego kodu. Mały diff > przepisanie modułu.
- Po zmianach: uruchom Unity batchmode compile (Unity 6000.3.1f1) lub zweryfikuj brak error CS w logu.

=== KROK 1 — Save (BLOCKER) ===
Problem: MainMenuManager.LoadGame() czyta stary PlayerData JSON; PauseMenu zapisuje przez SaveDataJSON/GameSaveData.

1. Ujednolić load w MainMenuManager → SaveDataJSON.Instance.LoadData() (lub wspólna ścieżka SavePath)
2. OnNewGameClicked: Restart.ResetData() + opcjonalnie usuń stary save
3. Udostępnić publiczne API dla B: SaveDataJSON.HasSaveFile() — Kontynuuj w menu
4. Save/load musi obejmować: pozycja, sektor gracza, HP/energy/cargo, kredyty, dług, purchasedUpgrades, stan asteroid (GameSaveData)

=== KROK 2 — Ekonomia i progresja ===
1. Utwórz 12 assetów UpgradeDefinition w Assets/Resources/Upgrades/ (po jednym na UpgradeEffectType)
   - Użyj ShopSystem.CreateDefaultCatalog() jako referencji wartości
2. SellSystem: przed AddCredits zastosuj SectorDefinition.shopTaxPercent z bieżącego sektora
   (SectorRegistry.GetDefinition(ChunkManager.Instance.CurrentPlayerSector))
3. Utwórz 3 assety FactionMissionDefinition w Assets/Resources/Missions/
4. Po sprzedaży: GameEvents.TriggerCreditsChanged + TriggerResourcesSold (już jest) — dodaj ewentualnie TriggerDebtChanged jeśli dług rośnie

=== KROK 3 — Mining (dokończenie GDD) ===
1. Zweryfikuj SectorStageResolver: 70% leading / 20% niższy / 10% wyższy (clamp 0–4)
2. AreaSpawnerManager: respawn pasa po ~80% wykopania — przetestuj, napraw jeśli nie działa
3. DODAJ event przed minigrą (B zrobi UI):
   - GameEvents.OnMiningAnalysisReady(SectorDefinition, threatLevel, composition summary, avgTemp)
   - Wywołaj w MiningGame tuż przed startem minigry (lub PlayerInteract przed LoadScene MiningScene)
4. Thermal shock >=90% — potwierdź dmg + OnHullDamaged w Play Mode

=== KROK 4 — Walka ===
1. EnemyLootDropper: zamiast Debug.Log — spawn pickup prefab LUB dodaj ResourceStack do PlayerInventory w zasięgu
2. Podpiąć PlasmaCannon + PlayerShipTurret na prefab statku (Assets/Prefabs — znajdź Statek/gracza, minimalny diff)
3. CombatPromptSystem — dodaj eventy dla B:
   - OnCombatPromptShown (wróg wykryty)
   - OnCombatPromptAnswered(bool fight) — stub odpowiedzi gracza (B podłączy UI)
4. Potwierdź EnemyAI flee przy 70% HP wroga + OnEnemyKilled

=== KROK 5 — Świat i progresja ===
1. FastTravelSystem: rozszerz o skok do Vector2Int sektora (jeśli PlayerData.fastTravel) — BEZ loading screen
   (teleport gracza + ChunkManager refresh — współpraca z B później przy mapie)
2. RepairSupportSystem: publiczna metoda / input do UseRepairKit() — B podłączy przycisk
3. BaseDropZone + GameManager.RespawnAtBase — zweryfikuj koszt 30% credits i utratę 20% cargo
4. Napraw missing scripts na prefabie statku (Console: "referenced script missing" na Statek)

=== KROK 6 — Audio i testy ===
1. Skopiuj banki FMOD z silver-eagle-audio/Build/Desktop/ → Assets/StreamingAssets/ (jeśli brak)
2. Usuń lub wyłącz martwy Move.cs na prefabie (konflikt z ShipController)
3. SmokeTest musi przechodzić (Unity -runTests -testPlatform playmode w batchmode)
4. Zaktualizuj README — sekcja znane bugi (tylko jeśli coś zostaje)

=== NIE RUSZAJ ===
- Scripts/UI/*
- Resources/Sectors/*.asset (to robi B)
- Loading screen, pełny JumpToSector z async load (Faza 3)

=== ACCEPTANCE CRITERIA (odhacz w raporcie) ===
[ ] Gra kompiluje się bez błędów CS
[ ] Save/load przez menu używa jednego formatu (SaveDataJSON)
[ ] 12 upgrade assetów istnieje; TryPurchase działa z DevConsole/testu
[ ] Podatek sektorowy stosowany przy sprzedaży (test A4 Erad'os 45%)
[ ] Wróg dropuje loot do gry (nie tylko log)
[ ] OnMiningAnalysisReady istnieje i jest wywoływany
[ ] SmokeTest passed
[ ] FMOD banki w StreamingAssets (lub README z instrukcją)

Raport końcowy: pliki | eventy | instrukcja dla B | znane problemy.
```

---

## PROMPT B — Produkt (sektory, UI, narracja, polish)

```
Jesteś AI z pełnym dostępem do plików. Projekt: Silver Eagle
(c:\Users\kacpe\Documents\silver-eagle). Unity 6000.3.1f1.

Pracujesz RÓWNOLEGLE z PROMPT A (silnik). Przeczytaj plan.md przed startem.
Wykonaj WSZYSTKO poniżej. Implementuj — nie pytaj o zgodę. Na końcu wypisz:
- listę zmienionych plików i prefabów
- ile sektorów ma pełne lore
- jakie GameEvents subskrybujesz / czego oczekujesz od A

=== ZASADY ===
- Edytujesz: Scripts/UI/*, Resources/Sectors/, Resources/Localization/, Prefabs/ (kokpit, menu), Scenes/ (UI refs)
- NIE edytujesz logiki: MiningGame.cs, EnemyAI.cs, ShopSystem.cs, SaveDataJSON.cs, ChunkManager.cs (logika spawnu)
- Możesz czytać GameEvents i dodawać TYLKO subskrypcje w UI — nie zmieniaj sygnatur eventów bez syncu z A
- Docx: użyj menu SilverEagle → Generate Sectors and Lore LUB PowerShell/ZipFile na Sectors.docx i SE - LORE.docx (word/document.xml)
- Matchuj styl projektu. Diegetyczny UI = zero screen-space HUD (wyjątek: overlay tutorialu).

=== KROK 1 — Sektory i lore z docx ===
1. Uruchom / ulepsz DocxExtractor — import z Sectors.docx + SE - LORE.docx
2. Popraw 6+ kluczowych sektorów (pełne lore):
   A6 Keimos (START — territory Cermandia/Ariandia, stage 0, NIE Rubieże!)
   B5 Rubieże, C5 Pirate Cove, D5 The Rod, E2 Uranus Outpost, F5 Mrainesden, A4 Erad'os (shopTaxPercent 45)
3. Pozostałe 30 sektorów: poprawne leadingStage, riskLevel, territory, patrolPresence — lore placeholder OK
4. SectorContentDatabase ładuje z Resources/Sectors/ — zweryfikuj GetSector(grid)

=== KROK 2 — Diegetyczny UI (GDD §9) ===
1. CockpitDisplayManager = jedyne źródło HP/energy/cargo/credits na monitorach
2. Wyłącz UI_ShipStatus na prefabie statku (nie usuwaj skryptu — disable component)
3. MapDisplay / MapSectorButton: klik sektora → ShowSectorBriefing + CRT log
4. NavTableInteract: FuelVsCargo panel — dopracuj UX (teksty, % paliwa/ładunku)
5. Subskrybuj GameEvents:
   OnSectorEntered, OnCreditsChanged, OnHullDamaged
   (+ od A gdy gotowe: OnMiningAnalysisReady, OnCombatPromptShown)

=== KROK 3 — Tutorial i lokalizacja ===
1. TutorialManager: kroki 3–5 na eventach (OnMiningComplete, OnResourcesSold, OnMapToggled) — już są emitowane
2. Zamień hardcoded EN w tutorialu na LocalizationManager.GetString()
3. Rozszerz Assets/Resources/Localization/Strings.csv — min. 50 kluczy PL+EN:
   menu, tutorial 5 kroków, combat/mining komunikaty, 36 nazw sektorów (SEC_A1…)
4. Podłącz LocalizationManager w MainMenu, PauseMenu, DeathScreen, TutorialManager

=== KROK 4 — Menu i ekrany ===
1. MainMenuManager:
   - Kontynuuj widoczny tylko gdy SaveDataJSON.HasSaveFile()
   - Nowa gra → dialog potwierdzenia nadpisania zapisu
   - LoadGame → SaveDataJSON (po merge z A — jeśli A jeszcze nie zrobił, użyj HasSaveFile + stub)
2. PauseMenu: Zapisz grę → SaveDataJSON.Instance.SaveData() (już częściowo — zweryfikuj)
3. DeathScreenUI: tekst GDD — wystrzelenie z kokpitu, koszt naprawy (30% credits), utrata ładunku
   Pobierz dane z GameManager / EconomyManager przy GameOver
4. CreditsScreen.cs — autorzy z GDD, przycisk w MainMenu
5. SettingsMenu: slidery Music/SFX (FMOD bus jeśli dostępne)

=== KROK 5 — Sklep, misje, sprzedaż (UI) ===
1. ShopUI.cs (nowy): lista upgrade z ShopSystem.Instance, CanAfford, TryPurchase, oznaczenie „kupione”
   Wyświetl na monitorze stacji / w panelu sklepu w sektorze ze shopPrefab
2. MissionUI.cs (nowy): 3 misje z FactionMissionSystem, przycisk oddaj surowiec
3. SellSummaryUI.cs (nowy): po sprzedaży — zarobek, podatek sektora, aktualny dług (EconomyManager.Debt)

=== KROK 6 — Combat / mining UI (pod eventy A) ===
1. CombatPromptUI.cs: subskrybuje OnCombatPromptShown — „Walcz / Uciekaj” (diegetyczny monitor)
2. MiningAnalysisUI.cs: subskrybuje OnMiningAnalysisReady — skład %, temp, LOW/MID/HIGH przed minigrą
   Jeśli A jeszcze nie dodał eventu — przygotuj UI + fallback z SectorDefinition

=== KROK 7 — Narracja i territory ===
1. NarrativeDirector + CrewBarks — rozszerz barki (3–5 per postać: sectorEnter, combat, debt, lowFuel)
2. SectorTerritoryRules — komunikaty na CockpitDisplayManager (patrol, Rubieże, Ariandia disarm)
3. MapToggle: przy otwarciu mapy wywołaj GameEvents.TriggerMapToggled (jeśli brak — dodaj wywołanie tutaj, sync z A)

=== KROK 8 — Polish ===
1. README: zgodność ze sterowaniem (pitch/roll z EnemyAI), build, znane bugi
2. Zweryfikuj Build Windows (SilverEagle → Build Windows)
3. NIE implementuj loading screen (Faza 3)

=== NIE RUSZAJ ===
- MiningGame logika, EnemyAI, ShopSystem logika, SaveDataJSON schema
- Pełny JumpToSector z teleportem gracza (czeka na A / Faza 3)

=== ACCEPTANCE CRITERIA (odhacz w raporcie) ===
[ ] Keimos ma poprawne territory/stage (start bezpieczny)
[ ] Min. 6 sektorów z pełnym lore z docx
[ ] Stats tylko na monitorach (UI_ShipStatus off)
[ ] Mapa pokazuje briefing + CRT przy wejściu w sektor
[ ] Tutorial 5 kroków przechodzi
[ ] Strings.csv ≥50 kluczy PL+EN, używane w UI
[ ] MainMenu: Kontynuuj / nadpisanie save
[ ] ShopUI + MissionUI + SellSummaryUI działają
[ ] DeathScreen + CreditsScreen
[ ] Gra kompiluje się bez błędów

Raport końcowy: assety sektorów | prefaby UI | eventy od A | screenshot checklist.
```

---

## Po obu promptach — sync (~15 min)

1. **A merge → main**, potem **B rebase** (lub odwrotnie — ustalcie raz)
2. Sprawdź konflikt tylko w `GameEvents.cs` — reszta rozłączona
3. B podłącza UI do nowych eventów A (`OnMiningAnalysisReady`, `OnCombatPromptShown`)
4. A podłącza `SellSystem` tax do `SectorDefinition` z assetów B
5. **Playtest MVP** (plan.md — sekcja Sync)

**Definition of done klasy:** checklist na końcu `plan.md` — oboje odhaczacie po Fazie 2.
