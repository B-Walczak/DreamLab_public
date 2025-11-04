using DreamLab.Core;
using DreamLab.MVVM.View;
using DreamLab.MVVM.View.Games;
using DreamLab.MVVM.ViewModel.Games;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using DreamLab.MVVM.Model;

namespace DreamLab.MVVM.ViewModel
{



    public class MainViewModel : ObservableObject
    {
        private object _currentView;
        private bool _isInGame;
        private int _gameDuration;
        private int _reactionTime;
        private GameSettings _settings;

        private const string ResultsFilePath = "results.json";
        public ObservableCollection<DreamLab.MVVM.Model.GameResult> Results { get; } = new();
        private double _averageAccuracy;
        public string AverageAccuracyText => Results.Count > 0
            ? $"Na podstawie {Results.Count} gier trafność wynosi {Math.Round(_averageAccuracy, 1)}%"
            : "Brak wyników";


        public string GameDurationText => $"{_gameDuration} sek";
        public string ReactionTimeText => $"{_reactionTime} sek";

        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public bool IsInGame
        {
            get => _isInGame;
            set { _isInGame = value; OnPropertyChanged(); }
        }

        public ICommand StartCommand { get; }
        public ICommand ResultsCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand StartColorsGameCommand { get; }
        public ICommand IncreaseGameTimeCommand { get; }
        public ICommand DecreaseGameTimeCommand { get; }
        public ICommand IncreaseReactionTimeCommand { get; }
        public ICommand DecreaseReactionTimeCommand { get; }
        public ICommand ClearResultsCommand { get; }


        public MainViewModel()
        {
            // Widoki
            var startView = new StartView();
            var resultsView = new ResultsView { DataContext = new ResultsViewModel() };
            var settingsView = new SettingsView();
            _settings = GameSettings.Load();
            _gameDuration = _settings.GameDuration;
            _reactionTime = _settings.ReactionTime;
            LoadResults();


            // Domyślny widok
            CurrentView = startView;

            StartCommand = new RelayCommand(_ =>
            {
                CurrentView = startView;
                IsInGame = false;
            });

            ResultsCommand = new RelayCommand(_ =>
            {
                CurrentView = resultsView;
                IsInGame = false;
            });

            SettingsCommand = new RelayCommand(_ =>
            {
                CurrentView = settingsView;
                IsInGame = false;
            });

            ExitCommand = new RelayCommand(_ =>
            {
                System.Windows.Application.Current.Shutdown();
            });

            IncreaseGameTimeCommand = new RelayCommand(_ =>
            {
                if (_gameDuration < 60)
                {
                    _gameDuration += 5;
                    _settings.GameDuration = _gameDuration;
                    _settings.Save();
                    OnPropertyChanged(nameof(GameDurationText));
                }
            });

            DecreaseGameTimeCommand = new RelayCommand(_ =>
            {
                if (_gameDuration > 15)
                {
                    _gameDuration -= 5;
                    _settings.GameDuration = _gameDuration;
                    _settings.Save();
                    OnPropertyChanged(nameof(GameDurationText));
                }
            });

            IncreaseReactionTimeCommand = new RelayCommand(_ =>
            {
                if (_reactionTime < 10)
                {
                    _reactionTime += 1;
                    _settings.ReactionTime = _reactionTime;
                    _settings.Save();
                    OnPropertyChanged(nameof(ReactionTimeText));
                }
            });

            DecreaseReactionTimeCommand = new RelayCommand(_ =>
            {
                if (_reactionTime > 1)
                {
                    _reactionTime -= 1;
                    _settings.ReactionTime = _reactionTime;
                    _settings.Save();
                    OnPropertyChanged(nameof(ReactionTimeText));
                }
            });

            ClearResultsCommand = new RelayCommand(_ =>
            {
                Results.Clear();
                SaveResults();
                UpdateAverageAccuracy();
            });


            StartColorsGameCommand = new RelayCommand(_ =>
            {


                var gameViewModel = new ColorsGameViewModel
                {
                    TestDurationSeconds = _gameDuration,
                    ReactionTimeSeconds = _reactionTime      
                };
                var gameView = new ColorsGameView { DataContext = gameViewModel };

                gameViewModel.GameFinished += (duration, accuracy) =>
                {
                    var result = new DreamLab.MVVM.Model.GameResult
                    {
                        Date = DateTime.Now,
                        Duration = duration,
                        Accuracy = accuracy
                    };
                    Results.Add(result);
                    SaveResults();
                    UpdateAverageAccuracy();
                };

                gameViewModel.GameExited += () =>
                {
                    CurrentView = startView;
                    IsInGame = false;
                };

                CurrentView = gameView;
                IsInGame = true;
            });
        }
        private void LoadResults()
        {
            if (File.Exists(ResultsFilePath))
            {
                try
                {
                    var json = File.ReadAllText(ResultsFilePath);
                    var loaded = JsonSerializer.Deserialize<ObservableCollection<DreamLab.MVVM.Model.GameResult>>(json);
                    if (loaded != null)
                    {
                        foreach (var result in loaded)
                            Results.Add(result);
                        UpdateAverageAccuracy();
                    }
                }
                catch { /* Ignoruj błędy pliku */ }
            }
        }

        private void SaveResults()
        {
            var json = JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ResultsFilePath, json);
        }

        private void UpdateAverageAccuracy()
        {
            if (Results.Count == 0)
            {
                _averageAccuracy = 0;
            }
            else
            {
                _averageAccuracy = 0;
                foreach (var r in Results)
                    _averageAccuracy += r.Accuracy;
                _averageAccuracy /= Results.Count;
            }
            OnPropertyChanged(nameof(AverageAccuracyText));
        }

    }
}
