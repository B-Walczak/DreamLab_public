using System;
using System.Collections.ObjectModel;
using System.Media;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;
using DreamLab.Core;

namespace DreamLab.MVVM.ViewModel.Games
{
    public class ColorsGameViewModel : ObservableObject
    {
        private readonly Random _rand = new();
        private readonly System.Timers.Timer _roundTimer;
        private readonly System.Timers.Timer _testTimer;
        private int _activeIndex;
        private string _activeKey = "";
        private bool _isRunning;
        private int _correct;
        private int _incorrect;
        private DateTime _startTime;
        public event Action<string, int> GameFinished;


        public ObservableCollection<Brush> Circles { get; } = new(new Brush[10]);

        // Widoczność ekranów
        private bool _isTutorialVisible = true;
        private bool _isCountdownVisible;
        private bool _isGameVisible;
        private bool _isResultsVisible;

        public bool IsTutorialVisible
        {
            get => _isTutorialVisible;
            set { _isTutorialVisible = value; OnPropertyChanged(); }
        }

        public bool IsCountdownVisible
        {
            get => _isCountdownVisible;
            set { _isCountdownVisible = value; OnPropertyChanged(); }
        }

        public bool IsGameVisible
        {
            get => _isGameVisible;
            set { _isGameVisible = value; OnPropertyChanged(); }
        }

        public bool IsResultsVisible
        {
            get => _isResultsVisible;
            set { _isResultsVisible = value; OnPropertyChanged(); }
        }

        // Dane testu
        private string _elapsedTime = "00:00";
        public string ElapsedTime
        {
            get => _elapsedTime;
            set { _elapsedTime = value; OnPropertyChanged(); }
        }

        private int _accuracy;
        public int Accuracy
        {
            get => _accuracy;
            set { _accuracy = value; OnPropertyChanged(); }
        }

        public int TestDurationSeconds { get; set; } = 60;
        public int ReactionTimeSeconds { get; set; } = 5;

        public event Action GameExited;

        public ICommand ExitCommand { get; }
        public ICommand RetryCommand { get; }

        public ColorsGameViewModel()
        {
            for (int i = 0; i < Circles.Count; i++)
                Circles[i] = Brushes.Black;

            _roundTimer = new System.Timers.Timer(ReactionTimeSeconds * 1000);
            _roundTimer.Elapsed += (s, e) => Missed();

            _testTimer = new System.Timers.Timer(1000);
            _testTimer.Elapsed += (s, e) => CheckTime();

            ExitCommand = new RelayCommand(_ =>
            {
                _isRunning = false;
                _roundTimer.Stop();
                _testTimer.Stop();
                GameExited?.Invoke();
            });

            RetryCommand = new RelayCommand(_ => StartGame());
        }

        public void StartGame()
        {
            IsTutorialVisible = false;
            IsResultsVisible = false;
            IsGameVisible = true;

            _correct = 0;
            _incorrect = 0;
            _isRunning = true;
            _startTime = DateTime.Now;
            ElapsedTime = "00:00";
            Accuracy = 0;

            _testTimer.Start();
            NextRound();
        }

        private void NextRound()
        {
            if (!_isRunning) return;

            for (int i = 0; i < Circles.Count; i++)
                Circles[i] = Brushes.Black;

            _activeIndex = _rand.Next(10);
            int colorId = _rand.Next(5);

            (_activeKey, Circles[_activeIndex]) = colorId switch
            {
                0 => ("C", Brushes.Red),
                1 => ("E", Brushes.Blue),
                2 => ("Y", Brushes.White),
                3 => ("I", Brushes.Yellow),
                4 => ("M", Brushes.Green),
                _ => ("C", Brushes.Red)
            };

            OnPropertyChanged(nameof(Circles));
            _roundTimer.Stop();
            _roundTimer.Interval = ReactionTimeSeconds * 1000;
            _roundTimer.Start();
        }

        public void HandleKeyPress(System.Windows.Input.Key key)
        {
            if (!_isRunning) return;
            string pressed = key.ToString().ToUpper();

            if (pressed == _activeKey)
                _correct++;
            else
            {
                _incorrect++;
                SystemSounds.Beep.Play();
            }

            UpdateStats();
            NextRound();
        }

        private void Missed()
        {
            if (!_isRunning) return;
            _incorrect++;
            SystemSounds.Beep.Play();
            UpdateStats();
            NextRound();
        }

        private void CheckTime()
        {
            if (!_isRunning) return;

            var elapsed = DateTime.Now - _startTime;
            ElapsedTime = elapsed.ToString(@"mm\:ss");

            if (elapsed.TotalSeconds >= TestDurationSeconds)
                EndGame();
        }

        private void UpdateStats()
        {
            if (_correct + _incorrect == 0) Accuracy = 0;
            else Accuracy = (int)((double)_correct / (_correct + _incorrect) * 100);
        }

        private void EndGame()
        {
            _isRunning = false;
            _roundTimer.Stop();
            _testTimer.Stop();

            UpdateStats();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                IsGameVisible = false;
                IsResultsVisible = true;
                OnPropertyChanged(nameof(IsGameVisible));
                OnPropertyChanged(nameof(IsResultsVisible));
            });

            GameResultsManager.AddResult(new GameResult
            {
                Date = DateTime.Now,
                Correct = _correct,
                Incorrect = _incorrect,
                Accuracy = Accuracy,
                Duration = ElapsedTime
            });
            GameFinished?.Invoke(ElapsedTime, Accuracy);

        }
    }
}
