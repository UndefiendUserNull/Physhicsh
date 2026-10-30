using nkast.Aether.Physics2D.Common;
using PicoToml;

namespace Physhicsh
{
    [PicoTomlSerializable]
    public class Config
    {
        public float PPM { get; set; } = 32f;
        public int SCREEN_WIDTH { get; set; } = 1280;
        public int SCREEN_HEIGHT { get; set; } = 640;
        public int MAX_FPS { get; set; } = 60;
        public bool USE_VSYNC { get; set; } = true;
        public int MAX_SUBSTEPS { get; set; } = 3;
        public float FIXED_DT { get; set; } = 1f / 60f;
        public float MAX_FRAME_TIME { get; set; } = 0.060f;
        public float ROTATION_SPEED { get; set; } = 10f;
        public string TEXTURES_PATH { get; set; } = "textures";

        public static Vector2 ScreenCenter => new(Instance.SCREEN_WIDTH / 2,
                                                  Instance.SCREEN_HEIGHT / 2);
        private static Config? _instance;
        private static readonly object _lock = new();

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
                throw new FileNotFoundException($"Configuration file '{path}' not found.");

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