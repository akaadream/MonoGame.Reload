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
using MonoGameReload.Assets;
using System;
using System.IO;

namespace MonoGameReload.Files
{
    public class FileWatcher
    {
        /// <summary>
        /// The content manager used to initialize the assets library
        /// </summary>
        public ContentManager ContentManager { get; private set; }

        /// <summary>
        /// The root path where to look for files updates. If you want to not use the recursive mode, put the root on the specific folder you want to watch
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// The project root path
        /// </summary>
        public string ProjectRootPath { get; set; }

        /// <summary>
        /// If the file watcher have to look updates of 
        /// </summary>
        public bool Recursive { get; set; }

        /// <summary>
        /// OS system watcher
        /// </summary>
        public FileSystemWatcher? Watcher { get; set; }

        /// <summary>
        /// Files tree of the watcher
        /// </summary>
        public FilesTree FilesTree { get; set; }

        public FileWatcher(ContentManager contentManager, string root = "Content", bool recursive = true)
        {
            ContentManager = contentManager;
            RootPath = root;
            ProjectRootPath = "";

            DirectoryInfo? directoryInfo = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            if (directoryInfo != null && 
                directoryInfo.Parent != null && 
                directoryInfo.Parent.Parent != null && 
                directoryInfo.Parent.Parent.Parent != null)
            {
                ProjectRootPath = directoryInfo.Parent.Parent.Parent.FullName;
                RootPath = Path.Combine(ProjectRootPath, root);
            }
            
            Recursive = recursive;

            FilesTree = new();

            // Initialize assets collections
            Initialize();

            // Configure the file system watcher
            ConfigureHotReloading();
        }

        public void Initialize()
        {
            AssetsManager.Initialize(ContentManager);
        }

        /// <summary>
        /// Reconstruct the files tree of the content directory
        /// </summary>
        public void LoadFiles()
        {
            // Prevent duplication if the user if calling this method multiple times
            FilesTree.Files.Clear();

            // All the files from the content directory
            string[] files = Directory.GetFiles(RootPath, "", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string fullName = files[i].Replace(RootPath, "");
                // Ignore obj and bin folders
                if (fullName.StartsWith(@"\obj") || fullName.StartsWith(@"\bin"))
                {
                    continue;
                }

                // Ignored extensions check
                string extension = Path.GetExtension(files[i]);
                if (FileWatcherExtensions.IsIgnored(extension))
                {
                    continue;
                }

                // Add the file to the files tree
                FilesTree.Files.Add(new(files[i], RootPath));
            }

            // Constructed files properties
            AssetsManager.Cleanup();
            foreach (FileProperties file in FilesTree.Files)
            {
                AssetsManager.Load(file);
            }
        }

        /// <summary>
        /// Watcher events
        /// </summary>
        private void ConfigureHotReloading()
        {
            Watcher = new(RootPath)
            {
                EnableRaisingEvents = true,

                // If true, the watcher will look updates on sub folders of the root directory
                IncludeSubdirectories = Recursive,
                NotifyFilter = NotifyFilters.LastWrite,
            };

            // Watcher events
            Watcher.Changed += UpdateContentFile;
            Watcher.Error += OnError;
        }

        /// <summary>
        /// Called when a file from the root directory is updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void UpdateContentFile(object sender, FileSystemEventArgs args)
        {
            if (args.Name == null)
            {
                return;
            }

            string fileName = args.Name.ToString().Replace(@"\", "/");
            fileName = fileName.Split('.')[0];

            FileProperties? file = FilesTree.Find(fileName);
            if (file == null)
            {
                return;
            }

            UpdateFile(file);
        }

        /// <summary>
        /// Update the given file inside the asset manager
        /// </summary>
        /// <param name="file"></param>
        private void UpdateFile(FileProperties file)
        {
            // We didn't found the file
            if (file == null)
            {
                return;
            }

            // The file asset has no processing, we can't refresh it
            if (file.AssetType == AssetType.NoProcessing)
            {
                return;
            }

            switch (file.AssetType)
            {
                case AssetType.Texture:
                    Texture2D? newTexture2D = AssetReloader.ReloadTexture2D(file.AbsolutePath);
                    if (newTexture2D == null || AssetsManager.Textures == null)
                    {
                        return;
                    }
                    AssetsManager.Textures[file.FullName] = newTexture2D;
                    break;
                case AssetType.Effect:
                    Effect? newEffect = AssetReloader.ReloadEffect(file.AbsolutePath);
                    if (newEffect == null || AssetsManager.Effects == null)
                    {
                        return;
                    }
                    AssetsManager.Effects[file.FullName] = newEffect;
                    break;
                case AssetType.SoundEffect:
                    SoundEffect? newSoundEffect = AssetReloader.ReloadSoundEffect(file.AbsolutePath);
                    if (newSoundEffect == null || AssetsManager.SoundEffects == null)
                    {
                        return;
                    }
                    // Clean the current sound effect
                    AssetsManager.SoundEffects[file.FullName].Dispose();
                    AssetsManager.SoundEffects[file.FullName] = newSoundEffect;
                    break;
                case AssetType.Song:
                    Song? newSong = AssetReloader.ReloadSong(file.AbsolutePath);
                    if (newSong == null || AssetsManager.Songs == null)
                    {
                        return;
                    }
                    // If a song is already playing, stop it
                    MediaPlayer.Stop();
                    AssetsManager.Songs[file.FullName] = newSong;
                    break;
                case AssetType.SpriteFont:
                    SpriteFont? newSpriteFont = AssetReloader.ReloadSpriteFont(file.AbsolutePath);
                    if (newSpriteFont == null || AssetsManager.SpriteFonts == null)
                    {
                        return;
                    }
                    AssetsManager.SpriteFonts[file.FullName] = newSpriteFont;
                    break;
                case AssetType.Model:
                    Model? newModel = AssetReloader.ReloadModel(file.AbsolutePath);
                    if (newModel == null || AssetsManager.Models == null)
                    {
                        return;
                    }
                    AssetsManager.Models[file.FullName] = newModel;
                    break;
            }
        }

        /// <summary>
        /// Called when an error is encountered while watching files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnError(object sender, ErrorEventArgs args)
        {
            throw new Exception("The file system watcher is unable to continue monitoring changes or the internal buffer overflows.");
        }
    }
}
