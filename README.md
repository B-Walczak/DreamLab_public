# DreamLab 🧠🎮

![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![WPF](https://img.shields.io/badge/WPF-5C2D91.svg?style=for-the-badge&logo=.net&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)

## 📖 O projekcie
**DreamLab** to aplikacja desktopowa stworzona przy użyciu technologii **WPF (Windows Presentation Foundation)**. Projekt stanowi zestaw interaktywnych modułów/gier (m.in. gry oparte na kolorach i kształtach), pozwalających na interakcję z użytkownikiem, mierzenie jego wyników oraz konfigurację ustawień. 

Głównym celem projektu było praktyczne zastosowanie wzorca architektonicznego **MVVM (Model-View-ViewModel)** w czystym C#, bez użycia zewnętrznych bibliotek ułatwiających ten proces (takich jak np. MVVM Light czy CommunityToolkit).

## ✨ Główne funkcjonalności
* **Moduły gier/zadań:** Zaimplementowane zróżnicowane widoki gier (`ColorsGame`, `ShapeGame`).
* **Zarządzanie wynikami:** Dedykowany moduł `GameResultsManager` do śledzenia i zapisywania postępów użytkownika.
* **Konfiguracja:** Możliwość dostosowywania ustawień aplikacji i poszczególnych modułów (`GameSettings`, `SettingsView`).
* **Obsługa multimediów:** Zintegrowane sygnały dźwiękowe wspierające interakcję z użytkownikiem (`high_tone.wav`, `low_tone.wav`).
* **Własny system motywów:** Wykorzystanie stylów i zasobów (folder `Theme`) do zarządzania wyglądem interfejsu.

## 🛠 Architektura i Wzorce Projektowe

Aplikacja została zbudowana w oparciu o rygorystyczne przestrzeganie wzorca **MVVM**. Aby udowodnić zrozumienie mechanizmów WPF, kluczowe elementy architektury zostały napisane od zera:

* **Separacja logiki:** Wyraźny podział na warstwy `Model`, `View` oraz `ViewModel`.
* **ObservableObject:** Własna implementacja interfejsu `INotifyPropertyChanged` zapewniająca dwukierunkowe wiązanie danych (Data Binding) pomiędzy widokiem a modelem widoku.
* **RelayCommand:** Autorska implementacja interfejsu `ICommand`, pozwalająca na eleganckie wywoływanie akcji z poziomu widoków `.xaml` bez zaśmiecania pliku "code-behind".
* **Value Converters:** Zastosowanie konwerterów (np. `InverseBoolToVisibilityConverter`) do dynamicznego zarządzania widocznością elementów UI na podstawie stanu logiki biznesowej.

## 💻 Struktura Projektu (Skrócona)
```text
DreamLab/
├── Core/               # Współdzielona logika, Konwertery, Ustawienia
│   ├── ObservableObject.cs
│   ├── RelayCommand.cs
│   └── GameResultsManager.cs
├── MVVM/
│   ├── Model/          # Logika domenowa
│   ├── View/           # Interfejs użytkownika (.xaml)
│   └── ViewModel/      # Logika prezentacji (np. MainViewModel.cs)
├── Sounds/             # Zasoby audio
├── Theme/              # Style i definicje UI
└── App.xaml            # Punkt wejścia aplikacji
