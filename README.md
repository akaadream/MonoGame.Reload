<h1 align="center">
    <div style="display:flex; justify-content: center; align-items: center;">
        <img src="Icon.png" width="32" style="margin-right: 12px" alt="MonoGame.Reload icon" />
        MonoGame.Reload
    </div>
    <img alt="Static Badge" src="https://img.shields.io/badge/NuGeT-0.2.0?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FMonoGame.Reload%2F0.2.0">
</h1>

MonoGame.Reload is an hot-reloader for your MonoGame application.  
No more thousands of restarts of your game to try out a new version of your assets.  
Set up the hot-reloader in a flash and simply enjoy the magic.  

## Installation

MonoGame.Reload is available as a NuGeT package [here](https://www.nuget.org/packages/MonoGame.Reload/0.2.0).  

#### .NET CLI
```
dotnet add package MonoGame.Reload --version 0.2.0
```

#### Package Manager
```
NuGet\Install-Package MonoGame.Reload -Version 0.2.0
```

#### PackageReference
```
<PackageReference Include="MonoGame.Reload" Version="0.2.0" />
```

## 0.2.0 updates

Since `MonoGame.Reload v0.2.0`, the library can reload Aseprite files using [MonoGame.Aseprite](https://monogameaseprite.net/) written by Aristurtle.  Check it out for more informations!

New assets are now supported :
- Aseprite files
- Data files (.txt, .json, .xml, ...)
  
You can now attach a callback to a file by listening the `Updated` event. The callback will be invoked right after the file has been reloaded.

## Getting started

On your Game class add an instance of `FileWatcher`:
```csharp
private FileWatcher _watcher;
```
Then, **instanciate** it inside your `Initialize` function:
```csharp
protected override void Initialize()
{
    _watcher = new(Content);

    base.Initialize();
}
```
> *Note*: The watcher is instanciated with the ContentManager because it will be used to create all the assets collections.

Now, you'll need to initialize the `AssetReloader`, the class used to hot-reload your content files:
```csharp
protected override void Initialize()
{
    _watcher = new(Content);

    AssetReloader.Initialize(
        _watcher.ProjectRootPath,
        Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform.DesktopGL,
        GraphicsDevice
        );
}
```

Finally, you have to load the current Content files inside your `LoadContent` function:

```csharp
protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    _watcher.LoadFiles();
}
```

Congratulations!  
The Hot-Reloader is now setup and you can easily access to an asset which will be automatically reloaded when you will update its source file:

```csharp
// Access to a Texture2D
AssetsManager.Textures["Sprites/name_of_my_texture"];
```

> *Note*: You have to respect the hierarchy of your files.  
> In the example above, the file "name_of_my_texture" is contained inside the folder Sprites inside the Content directory.


> *2nd Note*: If the key of the texture does not exists, an exception will be thrown, so you may want to check if the texture name exists by using `Textures2D.ContainsKey(string key)`.

## Listen a file update
You can attach a callback to a file for it to be invoked when the file is updated:
```csharp
protected override void LoadContent()
{
    // ...

    // Make sure you already load your files
    _watcher.LoadFiles();

    var file = _watcher.FilesTree.Find("name_of_my_asset");
    if (file != null)
    {
        file.Updated += OnMyAssetUpdate;
    }
}

private void OnMyAssetUpdate(object sender, FileSystemEventArgs args)
{
    // Do something
}
```

## Roadmap

This library is currently limited to the default assets the Content Pipeline is managing. (and even some types of files are missing)  
It's why I want to improve MonoGame.Reload to make it have more files reloaded!  
Do not hesitate to ask for new features!

## Credits
- MonoGame (under Ms-PL licence): https://github.com/MonoGame/MonoGame 
- MonoGame.Aseprite (under MIT licence): https://github.com/AristurtleDev/monogame-aseprite

## Licence

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