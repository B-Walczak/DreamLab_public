using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DreamLab.Core
{
    public class GameResult
    {
        public DateTime Date { get; set; }
        public int Correct { get; set; }
        public int Incorrect { get; set; }
        public int Accuracy { get; set; }
        public string Duration { get; set; } = "";
    }

    public class GameResultsManager
    {
        private static readonly string ResultsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "DreamLab", "results.json");

        public static List<GameResult> LoadResults()
        {
            if (!File.Exists(ResultsPath))
                return new List<GameResult>();

            try
            {
                var json = File.ReadAllText(ResultsPath);
                return JsonSerializer.Deserialize<List<GameResult>>(json) ?? new List<GameResult>();
            }
            catch
            {
                return new List<GameResult>();
            }
        }

        public static void SaveResults(List<GameResult> results)
        {
            var dir = Path.GetDirectoryName(ResultsPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ResultsPath, json);
        }

        public static void AddResult(GameResult result)
        {
            var results = LoadResults();
            results.Insert(0, result); // najnowszy na górze
            SaveResults(results);
        }

        public static void ClearResults()
        {
            if (File.Exists(ResultsPath))
                File.Delete(ResultsPath);
        }
    }
}
