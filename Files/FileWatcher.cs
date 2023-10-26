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
using MonoGame.Aseprite;
using MonoGameReload.Assets;
using SharpFont.PostScript;

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
        /// Types the watcher have to ignore
        /// </summary>
        public AssetType IgnoreType { get; set; }

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

            // Initialize ignore type
            IgnoreType = AssetType.NoProcessing;

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

                FileProperties file = new(files[i], RootPath);
                if (file.AssetType == IgnoreType)
                {
                    continue;
                }

                // Add the file to the files tree
                FilesTree.Files.Add(file);
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
                NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size,
            };

            // Watcher events
            Watcher.Changed += UpdateContentFile;
            Watcher.Created += CreateContentFile;
            Watcher.Deleted += DeleteContentFile;
            Watcher.Renamed += RenameContentFile;
            Watcher.Error += OnError;
        }

        /// <summary>
        /// Called when a file from the content directory is created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CreateContentFile(object sender, FileSystemEventArgs args)
        {
            if (args.FullPath == null || args.Name == null)
            {
                return;
            }

            FileProperties file = new(args.FullPath, RootPath);

            if (IgnoreType == file.AssetType)
            {
                return;
            }

            UpdateOrCreateFile(file, true);

            FilesTree.Files.Add(file);
        }

        /// <summary>
        /// Called when a file from the content directory is deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DeleteContentFile(object sender, FileSystemEventArgs args)
        {
            if (args.Name == null)
            {
                return;
            }

            FileProperties? file = FilesTree.Find(FileProperties.GetFilename(args.Name.ToString()));
            if (file == null || IgnoreType == file.AssetType)
            {
                return;
            }

            bool? removed = false;

            // Remove the loaded asset
            switch (file.AssetType)
            {
                case AssetType.Texture:
                    removed = AssetsManager.Textures?.Remove(file.FullName);
                    break;
                case AssetType.SpriteFont:
                    removed = AssetsManager.SpriteFonts?.Remove(file.FullName);
                    break;
                case AssetType.Model:
                    removed = AssetsManager.Models?.Remove(file.FullName);
                    break;
                case AssetType.Song:
                    removed = AssetsManager.Songs?.Remove(file.FullName);
                    break;
                case AssetType.SoundEffect:
                    removed = AssetsManager.SoundEffects?.Remove(file.FullName);
                    break;
                case AssetType.Effect:
                    removed = AssetsManager.Effects?.Remove(file.FullName);
                    break;
                case AssetType.Aseprite:
                    removed = AssetsManager.AsepriteFiles?.Remove(file.FullName);
                    break;
                case AssetType.Data:
                    removed = AssetsManager.DataFiles?.Remove(file.FullName);
                    break;
            }

            // The file has been deleted
            if (removed != null && removed == true)
            {
                // Remove the file from the files tree
                FilesTree.Files.Remove(file);
            }
        }

        /// <summary>
        /// Called when a file from the content directory is renamed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void RenameContentFile(object sender, RenamedEventArgs args)
        {
            if (args.Name == null || args.OldName == null)
            {
                return;
            }

            string oldFileName = FileProperties.GetFilename(args.OldName);
            string newFileName = FileProperties.GetFilename(args.Name);
            string newFilePath = args.FullPath;

            FileProperties? file = FilesTree.Find(oldFileName);
            if (file == null || IgnoreType == file.AssetType)
            {
                return;
            }

            // Update collections by adding the new file name and remove the old one
            switch (file.AssetType)
            {
                case AssetType.Texture:
                    if (AssetsManager.Textures == null || AssetsManager.Textures.ContainsKey(newFileName))
                    {
                        return;
                    }
                    AssetsManager.Textures.Add(newFileName, AssetsManager.Textures[file.FullName]);
                    AssetsManager.Textures.Remove(oldFileName);
                    break;
                case AssetType.SpriteFont:
                    if (AssetsManager.SpriteFonts == null || AssetsManager.SpriteFonts.ContainsKey(newFileName))
                    {
                        return;
                    }
                    AssetsManager.SpriteFonts.Add(newFileName, AssetsManager.SpriteFonts[file.FullName]);
                    AssetsManager.SpriteFonts.Remove(oldFileName);
                    break;
                case AssetType.Model:
                    if (AssetsManager.Models == null || AssetsManager.Models.ContainsKey(newFileName))
                    {
                        return;
                    }
                    AssetsManager.Models.Add(newFileName, AssetsManager.Models[file.FullName]);
                    AssetsManager.Models.Remove(oldFileName);
                    break;
                case AssetType.Song:
                    if (AssetsManager.Songs == null || AssetsManager.Songs.ContainsKey(newFileName))
                    {
                        return;
                    }
                    AssetsManager.Songs.Add(newFileName, AssetsManager.Songs[file.FullName]);
                    AssetsManager.Songs.Remove(oldFileName);
                    break;
                case AssetType.SoundEffect:
                    if (AssetsManager.SoundEffects == null || AssetsManager.SoundEffects.ContainsKey(newFileName))
                    {
                        return;
                    }
                    AssetsManager.SoundEffects.Add(newFileName, AssetsManager.SoundEffects[file.FullName]);
                    AssetsManager.SoundEffects.Remove(oldFileName);
                    break;
                case AssetType.Effect:
                    if (AssetsManager.Effects == null || AssetsManager.Effects.ContainsKey(newFileName))
                    {
                        return;
                    }
                    AssetsManager.Effects.Add(newFileName, AssetsManager.Effects[file.FullName]);
                    AssetsManager.Effects.Remove(oldFileName);
                    break;
                case AssetType.Aseprite:
                    if (AssetsManager.AsepriteFiles == null || AssetsManager.AsepriteFiles.ContainsKey(newFileName))
                    {
                        return;
                    }
                    AssetsManager.AsepriteFiles.Add(newFileName, AssetsManager.AsepriteFiles[file.FullName]);
                    AssetsManager.AsepriteFiles.Remove(oldFileName);
                    break;
                case AssetType.Data:
                    if (AssetsManager.DataFiles == null || AssetsManager.DataFiles.ContainsKey(newFileName))
                    {
                        return;
                    }
                    AssetsManager.DataFiles.Add(newFileName, AssetsManager.DataFiles[file.FullName]);
                    AssetsManager.DataFiles.Remove(oldFileName);
                    break;
            }

            file.Rename(newFilePath, RootPath);
        }

        /// <summary>
        /// Called when a file from the content directory is updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void UpdateContentFile(object sender, FileSystemEventArgs args)
        {
            if (args.Name == null)
            {
                return;
            }

            FileProperties? file = FilesTree.Find(FileProperties.GetFilename(args.Name.ToString()));
            if (file == null || IgnoreType == file.AssetType)
            {
                return;
            }

            UpdateOrCreateFile(file);

            // Call the updated event of the file
            file.OnUpdated(sender, args);
        }

        /// <summary>
        /// Update the given file inside the asset manager
        /// </summary>
        /// <param name="file"></param>
        private static void UpdateOrCreateFile(FileProperties file, bool create = false)
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
                    Texture2D? newTexture2D = AssetReloader.LoadTexture2D(file.AbsolutePath);
                    if (newTexture2D == null || AssetsManager.Textures == null)
                    {
                        return;
                    }

                    if (create)
                    {
                        AssetsManager.Textures.Add(file.FullName, newTexture2D);
                    }
                    else
                    {
                        AssetsManager.Textures[file.FullName] = newTexture2D;
                    }
                    break;
                case AssetType.Effect:
                    Effect? newEffect = AssetReloader.LoadEffect(file.AbsolutePath);
                    if (newEffect == null || AssetsManager.Effects == null)
                    {
                        return;
                    }

                    if (create)
                    {
                        AssetsManager.Effects.Add(file.FullName, newEffect);
                    }
                    else
                    {
                        AssetsManager.Effects[file.FullName] = newEffect;
                    }
                    break;
                case AssetType.SoundEffect:
                    SoundEffect? newSoundEffect = AssetReloader.LoadSoundEffect(file.AbsolutePath);
                    if (newSoundEffect == null || AssetsManager.SoundEffects == null)
                    {
                        return;
                    }

                    if (create)
                    {
                        AssetsManager.SoundEffects.Add(file.FullName, newSoundEffect);
                    }
                    else
                    {
                        // Clean the current sound effect
                        AssetsManager.SoundEffects[file.FullName].Dispose();
                        AssetsManager.SoundEffects[file.FullName] = newSoundEffect;
                    }
                    break;
                case AssetType.Song:
                    Song? newSong = AssetReloader.LoadSong(file.AbsolutePath);
                    if (newSong == null || AssetsManager.Songs == null)
                    {
                        return;
                    }
                    
                    if (create)
                    {
                        AssetsManager.Songs.Add(file.FullName, newSong);
                    }
                    else
                    {
                        // If a song is already playing, stop it
                        MediaPlayer.Stop();
                        AssetsManager.Songs[file.FullName] = newSong;
                    }
                    
                    break;
                case AssetType.SpriteFont:
                    SpriteFont? newSpriteFont = AssetReloader.LoadSpriteFont(file.AbsolutePath);
                    if (newSpriteFont == null || AssetsManager.SpriteFonts == null)
                    {
                        return;
                    }

                    if (create)
                    {
                        AssetsManager.SpriteFonts.Add(file.FullName, newSpriteFont);
                    }
                    else
                    {
                        AssetsManager.SpriteFonts[file.FullName] = newSpriteFont;
                    }
                    break;
                case AssetType.Model:
                    Model? newModel = AssetReloader.LoadModel(file.AbsolutePath);
                    if (newModel == null || AssetsManager.Models == null)
                    {
                        return;
                    }

                    if (create)
                    {
                        AssetsManager.Models.Add(file.FullName, newModel);
                    }
                    else
                    {
                        AssetsManager.Models[file.FullName] = newModel;
                    }
                    break;
                case AssetType.Aseprite:
                    AsepriteFile? newAsepriteFile = AssetReloader.LoadAsepriteFile(file.AbsolutePath);
                    if (newAsepriteFile == null || AssetsManager.AsepriteFiles == null)
                    {
                        return;
                    }

                    if (create)
                    {
                        AssetsManager.AsepriteFiles.Add(file.FullName, newAsepriteFile);
                    }
                    else
                    {
                        AssetsManager.AsepriteFiles[file.FullName] = newAsepriteFile;
                    }
                    break;
                case AssetType.Data:
                    string? newData = AssetReloader.LoadDataFile(file.AbsolutePath);
                    if (newData == null || AssetsManager.DataFiles == null)
                    {
                        return;
                    }

                    if (create)
                    {
                        AssetsManager.DataFiles.Add(file.FullName, newData);
                    }
                    else
                    {
                        AssetsManager.DataFiles[file.FullName] = newData;
                    }
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
