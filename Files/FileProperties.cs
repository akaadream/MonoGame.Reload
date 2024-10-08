﻿/* ----------------------------------------------------------------------------
MIT License

Copyright (c) 2023-2024 Guillaume Lortet

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

using MonoGameReload.Assets;

namespace MonoGameReload.Files
{
    public class FileProperties
    {
        /// <summary>
        /// The name of the file without extension
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The full name with the path and the extension
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The absolute path of the file
        /// </summary>
        public string AbsolutePath { get; set; }

        /// <summary>
        /// The file extension
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// The asset type the file is corresponding with
        /// </summary>
        public AssetType AssetType { get; set; }

        /// <summary>
        /// Event called when the file is updated
        /// </summary>
        public event EventHandler<FileSystemEventArgs>? Updated;

        private FileProperties()
        {
            Name = "";
            FullName = "";
            AbsolutePath = "";
            Extension = "";
            AssetType = AssetType.NoProcessing;
        }

        public FileProperties(string filePath, string root = ""): this()
        {
            Rename(filePath, root);
        }

        /// <summary>
        /// Rename the file
        /// </summary>
        /// <param name="newFilePath"></param>
        public void Rename(string newFilePath, string root = "")
        {
            AbsolutePath = newFilePath;
            if (root != string.Empty)
            {
                FullName = newFilePath.Replace(root, "");
            }
            FullName = FullName.Replace(@"\", "/");
            FullName = FullName.Replace(@"\\", "/");
            FullName = FullName[1..];
            if (FullName.Contains('.'))
            {
                int lastDotIndex = FullName.LastIndexOf('.');
                FullName = FullName[..lastDotIndex];
            }

            Extension = Path.GetExtension(newFilePath);

            Name = Path.GetFileNameWithoutExtension(newFilePath);

            AssetType = FindType(Extension);
        }

        public void OnUpdated(object sender, FileSystemEventArgs args)
        {
            Updated?.Invoke(this, args);
        }

        /// <summary>
        /// Find the type of the extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static AssetType FindType(string extension)
        {
            // Texture 2D
            if (FileWatcherExtensions.IsTexture(extension))
            {
                return AssetType.Texture;
            }

            // Sound effect
            if (FileWatcherExtensions.SoundEffectExtension == extension)
            {
                return AssetType.SoundEffect;
            }

            // Song
            if (FileWatcherExtensions.IsSong(extension))
            {
                return AssetType.Song;
            }

            // Model
            if (FileWatcherExtensions.IsModel(extension))
            {
                return AssetType.Model;
            }

            // Effect
            if (FileWatcherExtensions.EffectExtension == extension)
            {
                return AssetType.Effect;
            }

            // Aseprite
            if (FileWatcherExtensions.IsAsepriteProject(extension))
            {
                return AssetType.Aseprite;
            }

            // Data
            if (FileWatcherExtensions.IsData(extension))
            {
                return AssetType.Data;
            }

            return AssetType.NoProcessing;
        }

        public static string GetFilename(string filePath)
        {
            string fileName = filePath.Replace(@"\", "/");
            fileName = fileName.Split('.')[0];
            return fileName;
        }
    }
}
