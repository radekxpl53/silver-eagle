# Silver Eagle - Space Mining & Combat

Gra zręcznościowa z gatunku space mining & combat osadzona w uniwersum rywalizujących frakcji: Cermandii, Ariandii oraz bezprawnego obszaru Rubieży.

## 📂 Struktura projektu
```
Assets/  
├── Art/                     # Wizualia, modele, animacje, VFX
├── Audio/                   # Dźwięki i konfiguracja FMOD
├── Localization/            # Plik Strings.csv zawierający wersje językowe PL/EN
├── Prefabs/                 # Prefaby (w tym Cockpit i MonitorCanvas)
├── Scenes/                  # Sceny Unity (MainMenu, GameScene, itp.)
├── Scripts/                 # Skrypty C#
│   ├── Audio/               # Kontrolery FMOD (MusicStateController, HullAudioController)
│   ├── Core/                # Główne systemy gry (GameManager, TutorialManager, itp.)
│   ├── Editor/              # Skrypty edytorowe (SectorGenerator, BuildPlayer)
│   ├── ScriptableObjects/   # SectorDefinition
│   └── UI/                  # Systemy interfejsu (CockpitDisplayManager, PauseMenu)
└── Plugins/                 # Zewnętrzne wtyczki (w tym FMOD)
```

## 🎮 Sterowanie (GDD)

System lotu oparty na fizyce z [EnemyAI](https://github.com/Kaparee/EnemyAI): pitch/roll z limitami kąta (±40° / ±30°) i auto-wypoziomowaniem po puszczeniu klawiszy.

* **W / S** — Ciąg do przodu / wstecz
* **A / D** — Obrót lewo / prawo (Yaw)
* **Q / E** — Przechył lewo / prawo (Roll)
* **Space / Left Shift** — Pitch: nos w dół / w górę
* **Myszka** — Celowanie i obrót (tylko tryb FPP)
* **LPM** — Strzał z broni głównej (Heavy Kinetic Railgun)
* **E** — Interakcja ze stołem nawigacyjnym / asteroidami (gdy jesteś w zasięgu; ten sam klawisz co roll w locie)
* **V** — Zmiana widoku kamery (FPP / TPP)
* **X** — Przełącznik asystenta lotu (Flight Assist)
* **Escape** — Menu pauzy

## 🚀 Jak zbudować grę (Build Windows)
1. Otwórz projekt w Unity.
2. Z górnego menu wybierz opcję **SilverEagle -> Generate Sectors** (wygeneruje to 36 assetów sektorów z odpowiednimi metadanymi w folderze `Assets/Resources/Sectors/`).
3. Z górnego menu wybierz **SilverEagle -> Build Windows**.
4. Po ukończeniu kompilacji, gotowy build znajdziesz w katalogu `Builds/Win64/SilverEagle.exe`. Sceny testowe (takie jak `EnemyAiScene`) zostaną automatycznie pominięte w buildzie.

## 🛠️ Nowe Systemy
1. **Diegetyczny UI**: Kokpit statku wyświetla pancerz, energię, ładowność oraz kredyty. Stół nawigacyjny pozwala na podgląd sektorów i rekonfigurację pojemności ładunkowej oraz zasobów energetycznych.
2. **System Terytoriów**: W sektorach Cermandii i Ariandii obecność patroli ogranicza prędkość maksymalną statku do 30% normy. Rubieże wyłączają możliwość wysłania sygnału SOS.
3. **Tutorial (5 Kroków)**: Krok po kroku uczy manewrowania, namierzania asteroid, minigry górniczej, sprzedaży urobku i skoku przez stół nawigacyjny.
4. **Lokalizacja**: Dynamicznie wczytywana z pliku `Strings.csv`. Obsługuje dwa języki (pl/en).

## 🐛 Znane Błędy / Ograniczenia
* FMOD wymaga uprzedniego zaimportowania banków dźwięków w edytorze Unity.
* Brak obsługi nakładki Steam SDK (gra wydawana w modelu Direct PC / itch.io).
