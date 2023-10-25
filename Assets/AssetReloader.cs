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
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Framework.Content.Pipeline.Builder;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace MonoGameReload.Assets
{
    public static class AssetReloader
    {
        private static GraphicsDevice? GraphicsDevice;

        private static EffectProcessor? EffectProcessor;
        private static EffectImporter? EffectImporter;

        private static ModelProcessor? ModelProcessor;
        private static OpenAssetImporter? OpenImporter;

        private static FontDescriptionProcessor? FontDescriptionProcessor;
        private static FontDescriptionImporter? FontDescriptionImporter;

        private static PipelineImporterContext? PipelineImporterContext;
        private static PipelineProcessorContext? PipelineProcessorContext;

        private static BasicEffect? BasicEffect;

        /// <summary>
        /// Initialize all the components required to reload any files of the content folder
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="graphicsDevice"></param>
        public static void Initialize(string projectPath, TargetPlatform targetPlatform, GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;

            EffectProcessor = new();
            EffectImporter = new();

            ModelProcessor = new();
            OpenImporter = new();

            FontDescriptionProcessor = new();
            FontDescriptionImporter = new();

            PipelineManager pipelineManager = new(projectPath, "bin", "obj");
            pipelineManager.Platform = targetPlatform;

            PipelineImporterContext = new(pipelineManager);
            PipelineProcessorContext = new(pipelineManager, new());

            BasicEffect = new(graphicsDevice);
        }

        /// <summary>
        /// Reload a texture 2D from its source file
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Texture2D? ReloadTexture2D(string filePath)
        {
            Texture2D? texture = null;

            if (File.Exists(filePath))
            {
                Thread.Sleep(100);
                using FileStream stream = new(filePath, FileMode.Open);
                try
                {
                    texture = Texture2D.FromStream(GraphicsDevice, stream);
                }
                catch (Exception)
                {
                }
            }

            return texture;
        }

        /// <summary>
        /// Reload an effect from its source file
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Effect? ReloadEffect(string filePath)
        {
            Effect? effect = null;

            if (File.Exists(filePath))
            {
                try
                {
                    Thread.Sleep(100);

                    var effectContent = EffectImporter?.Import(filePath, PipelineImporterContext);
                    var effectData = EffectProcessor?.Process(effectContent, PipelineProcessorContext);
                    var dataBuffer = effectData?.GetEffectCode();

                    if (dataBuffer == null)
                    {
                        return null;
                    }

                    effect = new Effect(GraphicsDevice, dataBuffer, 0, dataBuffer.Length);
                }
                catch (Exception)
                {

                }
            }

            return effect;
        }

        /// <summary>
        /// Reload a sound effect from its source file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static SoundEffect? ReloadSoundEffect(string filePath)
        {
            SoundEffect? soundEffect = null;

            if (File.Exists(filePath))
            {
                Thread.Sleep(100);
                using FileStream stream = new(filePath, FileMode.Open);
                try
                {
                    soundEffect = SoundEffect.FromStream(stream);
                }
                catch (Exception)
                {

                }
            }

            return soundEffect;
        }

        /// <summary>
        /// Reload a song from its source file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Song? ReloadSong(string filePath)
        {
            Song? song = null;

            if (File.Exists(filePath))
            {
                Thread.Sleep(100);
                try
                {
                    string name = Path.GetFileNameWithoutExtension(filePath);
                    song = Song.FromUri(name, new(filePath));
                }
                catch (Exception)
                {

                }
            }

            return song;
        }

        /// <summary>
        /// Reload a model from its source file
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Model? ReloadModel(string filePath)
        {
            Model? model = null;

            if (File.Exists(filePath))
            {
                Thread.Sleep(100);
                try
                {
                    var node = OpenImporter?.Import(filePath, PipelineImporterContext);
                    var modelContent = ModelProcessor?.Process(node, PipelineProcessorContext);

                    if (modelContent == null)
                    {
                        return null;
                    }

                    List<ModelBone> modelBones = new();
                    foreach (ModelBoneContent modelBoneContent in modelContent.Bones)
                    {
                        ModelBone modelBone = new()
                        {
                            Transform = modelBoneContent.Transform,
                            Index = modelBoneContent.Index,
                            Name = modelBoneContent.Name,
                            ModelTransform = modelContent.Root.Transform,
                        };
                        modelBones.Add(modelBone);
                    }

                    for (int i = 0; i < modelBones.Count; i++)
                    {
                        var modelBone = modelBones[i];
                        var content = modelContent.Bones[i];
                        if (content.Parent != null && content.Parent.Index != -1)
                        {
                            modelBone.Parent = modelBones[content.Parent.Index];
                            modelBone.Parent.AddChild(modelBone);
                        }
                    }

                    List<ModelMesh> modelMeshes = new();
                    foreach (ModelMeshContent meshContent in modelContent.Meshes)
                    {
                        var name = meshContent.Name;
                        var parentBoneIndex = meshContent.ParentBone.Index;
                        var boudingSphere = meshContent.BoundingSphere;
                        var meshTag = meshContent.Tag;

                        List<ModelMeshPart> parts = new();
                        foreach (var partContent in meshContent.MeshParts)
                        {
                            IndexBuffer indexBuffer = new(GraphicsDevice, IndexElementSize.ThirtyTwoBits, partContent.IndexBuffer.Count, BufferUsage.WriteOnly);
                            {
                                int[] data = new int[partContent.IndexBuffer.Count];
                                partContent.IndexBuffer.CopyTo(data, 0);
                                indexBuffer.SetData(data);
                            }

                            VertexDeclarationContent vertexBufferDeclaration = partContent.VertexBuffer.VertexDeclaration;
                            List<VertexElement> elements = new();
                            foreach (var declareContentElement in vertexBufferDeclaration.VertexElements)
                            {
                                elements.Add(new VertexElement(declareContentElement.Offset, declareContentElement.VertexElementFormat, declareContentElement.VertexElementUsage, declareContentElement.UsageIndex));
                            }

                            VertexDeclaration vertexBufferDeclare = new(elements.ToArray());
                            VertexBuffer vertexBuffer = new(GraphicsDevice, vertexBufferDeclare, partContent.NumVertices, BufferUsage.WriteOnly);
                            {
                                vertexBuffer.SetData(partContent.VertexBuffer.VertexData);
                            }

#pragma warning disable CS0618 // Constructor is obsolete and will be internal in a future update
                            ModelMeshPart part = new()
                            {
                                VertexOffset = partContent.VertexOffset,
                                NumVertices = partContent.NumVertices,
                                PrimitiveCount = partContent.PrimitiveCount,
                                StartIndex = partContent.StartIndex,
                                Tag = partContent.Tag,
                                IndexBuffer = indexBuffer,
                                VertexBuffer = vertexBuffer
                            };
#pragma warning restore CS0618

                            parts.Add(part);
                        }

                        ModelMesh mesh = new(GraphicsDevice, parts)
                        {
                            Name = name,
                            BoundingSphere = boudingSphere,
                            Tag = meshTag
                        };
                        modelMeshes.Add(mesh);

                        foreach (var part in parts)
                        {
                            part.Effect = BasicEffect;
                        }

                        if (parentBoneIndex != -1)
                        {
                            mesh.ParentBone = modelBones[parentBoneIndex];
                            mesh.ParentBone.AddMesh(mesh);
                        }
                    }

                    model = new(GraphicsDevice, modelBones, modelMeshes);
                    model.Root = modelBones[modelContent.Root.Index];
                    model.Tag = modelContent.Tag;

                    var methods = model.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    var buildHierarchy = methods.Where(x => x.Name == "BuildHierarchy" && x.GetParameters().Length == 0).First();
                    buildHierarchy.Invoke(model, null);
                }
                catch (Exception)
                {

                }
            }

            return model;
        }

        /// <summary>
        /// Reload a sprite font description from its source file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static SpriteFont? ReloadSpriteFont(string filePath)
        {
            SpriteFont? spriteFont = null;

            if (File.Exists(filePath))
            {
                Thread.Sleep(100);
                try
                {
                    var fontDescription = FontDescriptionImporter?.Import(filePath, PipelineImporterContext);
                    var spriteFontContent = FontDescriptionProcessor?.Process(fontDescription, PipelineProcessorContext);

                    if (spriteFontContent == null)
                    {
                        return null;
                    }

                    var textureContent = spriteFontContent.Texture.Mipmaps[0];
                    textureContent.TryGetFormat(out SurfaceFormat format);

                    Texture2D texture = new(GraphicsDevice, textureContent.Width, textureContent.Height, false, format);
                    texture.SetData(textureContent.GetPixelData());

                    List<Rectangle> glyphBounds = spriteFontContent.Glyphs;
                    List<Rectangle> cropping = spriteFontContent.Cropping;
                    List<char> characters = spriteFontContent.CharacterMap;
                    int lineSpacing = spriteFontContent.VerticalLineSpacing;
                    float spacing = spriteFontContent.HorizontalSpacing;
                    List<Vector3> kerning = spriteFontContent.Kerning;
                    char? defaultCharacter = spriteFontContent.DefaultCharacter;

                    spriteFont = new SpriteFont(texture, glyphBounds, cropping, characters, lineSpacing, spacing, kerning, defaultCharacter);
                }
                catch (Exception)
                {

                }
            }

            return spriteFont;
        }
    }
}
