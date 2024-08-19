/* ----------------------------------------------------------------------------
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

namespace MonoGameReload.Files
{
    public static class FileWatcherExtensions
    {
        /// <summary>
        /// Texture extensions
        /// </summary>
        public static string[] TextureExtensions =
        {
            ".bmp", // Bitmap Image File
            ".cut", // Dr Halo CUT
            ".dds", // Direct Draw Surface
            ".g3", // Raw Fax G3
            ".hdr", // RGBE
            ".gif", // Graphcis Interchange Format
            ".ico", // Microsoft Windows Icon
            ".iff", // Interchange File Format
            ".jbg", ".jbig", // JBIG
            ".jng", ".jpg", ".jpeg", ".jpe", ".jif", ".jfif", ".jfi", // JPEG
            ".jp2", ".j2k", ".jpf", ".jpx", ".jpm", ".mj2", // JPEG 2000
            ".jxr", ".hdp", ".wdp", // JPEG XR
            ".koa", ".gg", // Koala
            ".pcd", // Kodak PhotoCD
            ".mng", // Multiple-Image Network Graphics
            ".pcx", //Personal Computer Exchange
            ".pbm", ".pgm", ".ppm", ".pnm", // Netpbm
            ".pfm", // Printer Font Metrics
            ".png", //Portable Network Graphics
            ".pict", ".pct", ".pic", // PICT
            ".psd", // Photoshop
            ".3fr", ".ari", ".arw", ".bay", ".crw", ".cr2", ".cap", ".dcs", // RAW
            ".dcr", ".dng", ".drf", ".eip", ".erf", ".fff", ".iiq", ".k25", // RAW
            ".kdc", ".mdc", ".mef", ".mos", ".mrw", ".nef", ".nrw", ".obm", // RAW
            ".orf", ".pef", ".ptx", ".pxn", ".r3d", ".raf", ".raw", ".rwl", // RAW
            ".rw2", ".rwz", ".sr2", ".srf", ".srw", ".x3f", // RAW
            ".ras", ".sun", // Sun RAS
            ".sgi", ".rgba", ".bw", ".int", ".inta", // Silicon Graphics Image
            ".tga", // Truevision TGA/TARGA
            ".tiff", ".tif", // Tagged Image File Format
            ".wbmp", // Wireless Application Protocol Bitmap Format
            ".webp", // WebP
            ".xbm", // X BitMap
            ".xpm", // X PixMap
        };

        /// <summary>
        /// Model extensions
        /// </summary>
        public static string[] ModelExtensions =
        {
            ".dae", // Collada
            ".gltf", "glb", // glTF
            ".blend", // Blender 3D
            ".3ds", // 3ds Max 3DS
            ".ase", // 3ds Max ASE
            ".obj", // Wavefront Object
            ".ifc", // Industry Foundation Classes (IFC/Step)
            ".xgl", ".zgl", // XGL
            ".ply", // Stanford Polygon Library
            ".dxf", // AutoCAD DXF
            ".lwo", // LightWave
            ".lws", // LightWave Scene
            ".lxo", // Modo
            ".stl", // Stereolithography
            ".ac", // AC3D
            ".ms3d", // Milkshape 3D
            ".cob", ".scn", // TrueSpace
            ".bvh", // Biovision BVH
            ".csm", // CharacterStudio Motion
            ".irrmesh", // Irrlicht Mesh
            ".irr", // Irrlicht Scene
            ".mdl", // Quake I, 3D GameStudio (3DGS)
            ".md2", // Quake II
            ".md3", // Quake III Mesh
            ".pk3", // Quake III Map/BSP
            ".mdc", // Return to Castle Wolfenstein
            ".md5", // Doom 3
            ".smd", ".vta", // Valve Model 
            ".ogex", // Open Game Engine Exchange
            ".3d", // Unreal
            ".b3d", // BlitzBasic 3D
            ".q3d", ".q3s", // Quick3D
            ".nff", // Neutral File Format, Sense8 WorldToolKit
            ".off", // Object File Format
            ".ter", // Terragen Terrain
            ".hmp", // 3D GameStudio (3DGS) Terrain
            ".ndo", // Izware Nendo
            ".fbx", // Default model object
            ".x", // DirectX object
        };

        /// <summary>
        /// Song extensions
        /// </summary>
        public static string[] SongExtensions =
        {
            ".ogg",
            ".wav",
            ".mp3",
        };

        /// <summary>
        /// Video extensions
        /// </summary>
        public static string[] VideoExtensions =
        {
            ".mp4",
            ".wmv",
        };

        /// <summary>
        /// Extension of files that the watcher will ignore
        /// </summary>
        public static string[] IgnoreExtensions =
        {
            ".mgcb",
            ".xnb"
        };

        /// <summary>
        /// Aseprite project files extensions
        /// </summary>
        public static string[] AsepriteExtensions =
        {
            ".ase",
            ".aseprite"
        };

        /// <summary>
        /// Data files extensions
        /// </summary>
        public static string[] DataExtensions =
        {
            ".txt", // Txt
            ".xml", // XML
            ".json", // JSON
            ".css", // CSS
            ".html", // HTML
            ".js", // JS
            ".jsx", // JS
            ".php", // PHP
            ".cs", // C#
            ".cpp", // C++
            ".c", // C
            ".py", // Python
            ".lua", // Lua
            ".rb", // Ruby
        };

        /// <summary>
        /// Effect extension
        /// </summary>
        public static string EffectExtension = ".fx";

        /// <summary>
        /// Sprite font extension
        /// </summary>
        public static string FontDescriptionExtension = ".spritefont";

        /// <summary>
        /// Sound effect extension
        /// </summary>
        public static string SoundEffectExtension = ".wav";

        /// <summary>
        /// XML extension
        /// </summary>
        public static string XMLExtension = ".xml";

        /// <summary>
        /// Json extension
        /// </summary>
        public static string JsonExtension = ".json";

        /// <summary>
        /// Return true if the given extension is a texture extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsTexture(string extension)
        {
            return Contains(TextureExtensions, extension);
        }

        /// <summary>
        /// Return true if the given extension is a model extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsModel(string extension)
        {
            return Contains(ModelExtensions, extension);
        }

        /// <summary>
        /// Return true if the given extension is a song extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsSong(string extension)
        {
            return Contains(SongExtensions, extension);
        }

        /// <summary>
        /// Return true if the given extension is a video extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsVideo(string extension)
        {
            return Contains(VideoExtensions, extension);
        }

        /// <summary>
        /// Return true if the given extension is an asprite file extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsAsepriteProject(string extension)
        {
            return Contains(AsepriteExtensions, extension);
        }

        /// <summary>
        /// Return true if the given extension is a data file extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsData(string extension)
        {
            return Contains(DataExtensions, extension);
        }

        /// <summary>
        /// Return true if the given extension is ignored
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsIgnored(string extension)
        {
            return Contains(IgnoreExtensions, extension);
        }

        /// <summary>
        /// Return true if the value is contained inside the given array
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool Contains(string[] arr, string value)
        {
            foreach (string ext in arr)
            {
                if (ext == value) return true;
            }

            return false;
        }
    }
}
