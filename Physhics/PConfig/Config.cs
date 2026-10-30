using nkast.Aether.Physics2D.Common;
using PicoToml;
using System;
using System.IO;
using System.Threading;

namespace Physhicsh.PConfig
{
    [PicoTomlSerializable]
    public class Config
    {
        public ScreenConfig Screen { get; set; } = new();
        public PhysicsConfig Physics { get; set; } = new();
        public ControlsConfig Controls { get; set; } = new();

        public static Vector2 ScreenCenter => new(Instance.Screen.SCREEN_WIDTH / 2,
                                                  Instance.Screen.SCREEN_HEIGHT / 2);
        private static Config? _instance;
        private static readonly Lock _lock = new();

        public static Config Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (_lock)
                    {
                        _instance ??= LoadFromFile("config.toml");
                    }
                }
                return _instance;
            }
        }

        private static Config LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                Config defaults = new Config();
                try
                {
                    string toml = TomlSerializer.Serialize(defaults);
                    File.WriteAllText(path, toml);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    throw new InvalidOperationException(
                        $"Failed to create default configuration file '{path}': {ex.Message}", ex);
                }
                return defaults;
            }

            try
            {
                ReadOnlySpan<byte> toml = File.ReadAllBytes(path).AsSpan();

                Config config = TomlSerializer.Deserialize<Config>(toml);

                return config ?? throw new InvalidOperationException(
                    $"Failed to deserialize configuration from '{path}'.");
            }
            catch (Exception ex) when (ex is FormatException or IOException)
            {
                throw new InvalidOperationException(
                    $"Error parsing configuration file '{path}': {ex.Message}", ex);
            }
        }
    }
}