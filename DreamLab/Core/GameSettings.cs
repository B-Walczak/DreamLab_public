using System;
using System.IO;
using System.Text.Json;

namespace DreamLab.Core
{
    public class GameSettings
    {
        public int GameDuration { get; set; } = 45;
        public int ReactionTime { get; set; } = 3;

        private static readonly string SettingsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "DreamLab", "settings.json");

        public void Save()
        {
            var dir = Path.GetDirectoryName(SettingsPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }

        public static GameSettings Load()
        {
            if (!File.Exists(SettingsPath))
                return new GameSettings();

            try
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<GameSettings>(json) ?? new GameSettings();
            }
            catch
            {
                return new GameSettings();
            }
        }
    }
}
