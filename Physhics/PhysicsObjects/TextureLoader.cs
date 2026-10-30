using Physhicsh.PConfig;
using Raylib_cs;

namespace Physhicsh.PhysicsObjects
{
    public class TextureLoader : IDisposable
    {
        private readonly List<Image> _loadedImages = [];
        private readonly List<Texture2D> _loadedTextures = [];

        public Texture2D LoadTexture(string path)
        {
            string combinedPath = Path.Combine(Config.Instance.Controls.TEXTURES_PATH, path);
            if (!Directory.Exists(Config.Instance.Controls.TEXTURES_PATH)) Directory.CreateDirectory(Config.Instance.Controls.TEXTURES_PATH);
            if (File.Exists(combinedPath))
            {
                return Raylib.LoadTexture(combinedPath);
            }
            else
            {
                throw new FileNotFoundException(combinedPath);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            foreach (Texture2D item in _loadedTextures)
            {
                Raylib.UnloadTexture(item);
            }

            foreach (Image item in _loadedImages)
            {
                Raylib.UnloadImage(item);
            }
        }
    }
}
