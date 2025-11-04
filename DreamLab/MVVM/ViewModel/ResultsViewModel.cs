using DreamLab.Core;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace DreamLab.MVVM.ViewModel
{
    public class ResultRecord
    {
        public string Date { get; set; }
        public string Duration { get; set; }
        public double Accuracy { get; set; }
    }

    public class ResultsViewModel : ObservableObject
    {
        private const string ResultsPath = "results.json";

        public ObservableCollection<ResultRecord> Results { get; set; } = new();

        public ICommand ClearResultsCommand { get; }
        public ICommand BackCommand { get; }

        public ResultsViewModel()
        {
            LoadResults();

            ClearResultsCommand = new RelayCommand(_ =>
            {
                Results.Clear();
                if (File.Exists(ResultsPath))
                    File.Delete(ResultsPath);
            });

            BackCommand = new RelayCommand(_ =>
            {
                var mainVM = (App.Current.MainWindow.DataContext as MainViewModel);
                mainVM?.StartCommand.Execute(null);
            });
        }

        private void LoadResults()
        {
            if (File.Exists(ResultsPath))
            {
                var json = File.ReadAllText(ResultsPath);
                var list = JsonSerializer.Deserialize<ObservableCollection<ResultRecord>>(json);
                if (list != null)
                    Results = list;
            }
            OnPropertyChanged(nameof(Results));
        }
    }
}
