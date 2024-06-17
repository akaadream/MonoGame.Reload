/* ----------------------------------------------------------------------------
MIT License

Copyright (c) 2024 Guillaume Lortet

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
---------------------------------------------------------------------------- */

using AsepriteDotNet.Aseprite;
using AsepriteDotNet.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Aseprite;
using MonoGameReload.Files;

namespace MonoGameReload.Assets
{
    /// <summary>
    /// Asset types
    /// </summary>
    public enum AssetType
    {
        NoProcessing,
        Texture,
        Song,
        SoundEffect,
        Effect,
        Model,
        SpriteFont,
        FontTexture,
        Material,
        LocalizedFont,
        Aseprite,
        Data
    }

    public static class AssetsManager
    {
        /// <summary>
        /// Content manager
        /// </summary>
        public static ContentManager? ContentManager { get; private set; }

        /// <summary>
        /// Textures 2D collection
        /// </summary>
        public static Dictionary<string, Texture2D>? Textures { get; private set; }

        /// <summary>
        /// Songs collection
        /// </summary>
        public static Dictionary<string, Song>? Songs { get; private set; }

        /// <summary>
        /// Sound effects collection (.wav)
        /// </summary>
        public static Dictionary<string, SoundEffect>? SoundEffects { get; private set; }

        /// <summary>
        /// Models collection
        /// </summary>
        public static Dictionary<string, Model>? Models { get; private set; }

        /// <summary>
        /// Sprite fonts collection (.spritefont)
        /// </summary>
        public static Dictionary<string, SpriteFont>? SpriteFonts { get; private set; }

        /// <summary>
        /// Materials collection
        /// </summary>
        public static Dictionary<string, EffectMaterial>? EffectMaterials { get; private set; }

        /// <summary>
        /// Effects collection (.fx)
        /// </summary>
        public static Dictionary<string, Effect>? Effects { get; private set; }

        /// <summary>
        /// Asprite files collection (.ase, .aseprite)
        /// </summary>
        public static Dictionary<string, AsepriteFile>? AsepriteFiles { get; private set; }

        /// <summary>
        /// Data files collection
        /// </summary>
        public static Dictionary<string, string>? DataFiles {  get; private set; }

        /// <summary>
        /// Initialize the asset manager
        /// </summary>
        /// <param name="contentManager"></param>
        public static void Initialize(ContentManager contentManager)
        {
            ContentManager = contentManager;

            Textures = [];
            Songs = [];
            SoundEffects = [];
            Models = [];
            SpriteFonts = [];
            EffectMaterials = [];
            Effects = [];
            AsepriteFiles = [];
            DataFiles = [];
        }

        /// <summary>
        /// Clean up all the assets collections
        /// </summary>
        public static void Cleanup()
        {
            Textures?.Clear();
            Songs?.Clear();
            SoundEffects?.Clear();
            Models?.Clear();
            SpriteFonts?.Clear();
            EffectMaterials?.Clear();
            Effects?.Clear();
            AsepriteFiles?.Clear();
            DataFiles?.Clear();
        }

        /// <summary>
        /// Loading an asset
        /// </summary>
        /// <param name="fileName"></param>
        public static void Load(AssetType type, string fileName, string absoluteFilePath = "")
        {
            if (ContentManager == null)
            {
                return;
            }

            switch (type)
            {
                case AssetType.Texture:
                    if (Textures == null)
                    {
                        return;
                    }
                    LoadTo(Textures, fileName);
                    break;
                case AssetType.SoundEffect:
                    if (SoundEffects == null)
                    {
                        return;
                    }
                    LoadTo(SoundEffects, fileName);
                    break;
                case AssetType.Song:
                    if (Songs == null)
                    {
                        return;
                    }
                    LoadTo(Songs, fileName);
                    break;
                case AssetType.Effect:
                    if (Effects == null)
                    {
                        return;
                    }
                    LoadTo(Effects, fileName);
                    break;
                case AssetType.Model:
                    if (Models == null)
                    {
                        return;
                    }
                    LoadTo(Models, fileName);
                    break;
                case AssetType.SpriteFont:
                    if (SpriteFonts == null)
                    {
                        return;
                    }
                    LoadTo(SpriteFonts, fileName);
                    break;
                case AssetType.Aseprite:
                    if (AsepriteFiles == null)
                    {
                        return;
                    }

                    if (!File.Exists(absoluteFilePath))
                    {
                        return;
                    }

                    var asepriteFile = AsepriteFileLoader.FromFile(absoluteFilePath);
                    if (asepriteFile == null)
                    {
                        return;
                    }

                    AsepriteFiles.Add(fileName, asepriteFile);
                    break;
                case AssetType.Data:
                    if (DataFiles == null)
                    {
                        return;
                    }

                    if (!File.Exists(absoluteFilePath))
                    {
                        return;
                    }

                    string? data = AssetReloader.LoadDataFile(absoluteFilePath);
                    if (data == null)
                    {
                        return;
                    }

                    DataFiles.Add(fileName, data);
                    break;
            }
        }

        /// <summary>
        /// Load an asset using its file properties
        /// </summary>
        /// <param name="file"></param>
        public static void Load(FileProperties file)
        {
            Load(file.AssetType, file.FullName, file.AbsolutePath);
        }

        /// <summary>
        /// Load an asset on the given dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool LoadTo<T>(Dictionary<string, T> dictionary, string fileName)
        {
            if (dictionary == null)
            {
                return false;
            }

            if (dictionary.ContainsKey(fileName))
            {
                return false;
            }

            if (ContentManager == null)
            {
                return false;
            }

            T asset = ContentManager.Load<T>(fileName);

            if (asset == null)
            {
                return false;
            }

            dictionary.Add(fileName, asset);
            return true;
        }

        /// <summary>
        /// Find the asset type from a filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static AssetType GetAssetTypeOf(string fileName)
        {
            if (Textures != null && Textures.ContainsKey(fileName))
            {
                return AssetType.Texture;
            }

            if (SoundEffects != null && SoundEffects.ContainsKey(fileName))
            {
                return AssetType.SoundEffect;
            }

            if (Songs != null && Songs.ContainsKey(fileName))
            {
                return AssetType.Song;
            }

            if (EffectMaterials != null && EffectMaterials.ContainsKey(fileName))
            {
                return AssetType.Material;
            }

            if (Effects != null && Effects.ContainsKey(fileName))
            {
                return AssetType.Effect;
            }

            if (Models != null && Models.ContainsKey(fileName))
            {
                return AssetType.Model;
            }

            if (AsepriteFiles != null && AsepriteFiles.ContainsKey(fileName))
            {
                return AssetType.Aseprite;
            }

            if (DataFiles != null && DataFiles.ContainsKey(fileName))
            {
                return AssetType.Data;
            }

            return AssetType.NoProcessing;
        }

        /// <summary>
        /// Find a texture by its key name and render it at the given postion using the given sprite batch.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="position"></param>
        /// <param name="rectangle"></param>
        /// <param name="spriteBatch"></param>
        /// <returns>True if a texture has been drawn on the screen</returns>
        public static bool Render2D(string key, Vector2 position, Rectangle? rectangle, SpriteBatch spriteBatch, GraphicsDevice? graphicsDevice = null, float rotation = 0f, float scale = 1f)
        {
            if (Textures != null && Textures.TryGetValue(key, out var texture))
            {
                spriteBatch.Draw(texture, position, rectangle, Color.White, rotation, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 1f);
                return true;
            }
            if (graphicsDevice != null && AsepriteFiles != null && AsepriteFiles.TryGetValue(key, out var asepriteFile))
            {
                Sprite sprite = asepriteFile.CreateSprite(graphicsDevice, 0);
                sprite.Scale = new Vector2(scale);
                sprite.Rotation = rotation;
                sprite.Origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
                sprite.Draw(spriteBatch, position);
            }
            return false;
        }

        /// <summary>
        /// Find a model by its key name and render it using the given matrixes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="world"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        /// <returns>True if a model has been drawn on the screen</returns>
        public static bool Render3D(string key, Matrix world, Matrix view, Matrix projection)
        {
            if (Models != null && Models.TryGetValue(key, out var model))
            {
                model.Draw(world, view, projection);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Find a sound or a song by its key name and play it with the specified volume
        /// </summary>
        /// <param name="key"></param>
        /// <param name="volume"></param>
        /// <returns>True if a sound or song has been played</returns>
        public static bool Play(string key, float volume = 1f)
        {
            if (SoundEffects != null && SoundEffects.TryGetValue(key, out var effect))
            {
                return effect.Play(1f, 0f, 0f);
            }
            if (Songs != null && Songs.TryGetValue(key, out var song))
            {
                MediaPlayer.Play(song);
                MediaPlayer.Volume = volume;
            }
            return false;
        }
    }
}
