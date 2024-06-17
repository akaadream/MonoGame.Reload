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

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using MonoGameReload.Assets;
using MonoGameReload.Files;

namespace MonoGameReload
{
    public static class Reloader
    {
        /// <summary>
        /// The file watcher which will check each file changes
        /// </summary>
        public static FileWatcher? FileWatcher { get; private set; }

        /// <summary>
        /// Initialize the reloader
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="target"></param>
        public static void Initialize(ContentManager content, GraphicsDevice graphicsDevice, TargetPlatform target)
        {
            FileWatcher = new(content);
            AssetReloader.Initialize(FileWatcher.RootPath, target, graphicsDevice);
        }

        /// <summary>
        /// Define all the asset types which would be ignored
        /// </summary>
        /// <param name="types"></param>
        public static void Ignore(params AssetType[] types)
        {
            if (types.Length == 0 ||
                FileWatcher == null)
            {
                return;
            }

            AssetType ignore = types[0];
            for (int i = 1; i < types.Length; i++)
            {
                ignore |= types[i];
            }

            FileWatcher.IgnoreType = ignore;
        }

        /// <summary>
        /// Link an update event callback to a specific file
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="callback"></param>
        public static void OnUpdate(string asset, EventHandler<FileSystemEventArgs> callback)
        {
            if (FileWatcher == null)
            {
                return;
            }

            var file = FileWatcher.FilesTree.Find(asset);
            if (file != null)
            {
                file.Updated += callback;
            }
        }
    }
}
