/* ----------------------------------------------------------------------------
MIT License

Copyright (c) 2023 Guillaume Lortet

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

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
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
        /// Initialize the asset manager
        /// </summary>
        /// <param name="contentManager"></param>
        public static void Initialize(ContentManager contentManager)
        {
            ContentManager = contentManager;

            Textures = new();
            Songs = new();
            SoundEffects = new();
            Models = new();
            SpriteFonts = new();
            EffectMaterials = new();
            Effects = new();
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
        }

        /// <summary>
        /// Loading an asset
        /// </summary>
        /// <param name="fileName"></param>
        public static void Load(AssetType type, string fileName)
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
            }
        }

        /// <summary>
        /// Load an asset using its file properties
        /// </summary>
        /// <param name="file"></param>
        public static void Load(FileProperties file)
        {
            Load(file.AssetType, file.FullName);
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

            return AssetType.NoProcessing;
        }
    }
}
