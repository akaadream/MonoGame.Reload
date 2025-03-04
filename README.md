<h1 align="center">
    <div>
        <img src="Icon.png" width="84" alt="MonoGame.Reload icon" />
        <br />
        MonoGame.Reload
    </div>
</h1>

[![Nuget 0.3.3](https://badgen.net/nuget/v/MonoGame.Reload/latest)](https://www.nuget.org/packages/MonoGame.Reload/0.3.3)
[![MIT licence](https://badgen.net/static/license/MIT/blue)](https://github.com/akaadream/MonoGame.Reload/blob/main/LICENCE)


MonoGame.Reload is an hot-reloader for your MonoGame application.  
No more thousands of restarts of your game to try out a new version of your assets.  
Set up the hot-reloader in a flash and simply enjoy the magic.  

## Installation

MonoGame.Reload is available as a NuGeT package [here](https://www.nuget.org/packages/MonoGame.Reload/0.3.3).  

#### .NET CLI
```
dotnet add package MonoGame.Reload --version 0.3.3
```

#### Package Manager
```
NuGet\Install-Package MonoGame.Reload -Version 0.3.3
```

#### PackageReference
```
<PackageReference Include="MonoGame.Reload" Version="0.3.3" />
```

## 0.3.1 updates

When an asset is updated, the previous one is disposed whenever it's possible.

## 0.3.0 updates

* The way to initialize the library has been reworked and simplified.
* You now have helper to quickly render textures, models, aseprite files or play sounds and songs

## 0.2.0 updates

Since `MonoGame.Reload v0.2.0`, the library can reload Aseprite files using [MonoGame.Aseprite](https://monogameaseprite.net/) written by Aristurtle.  Check it out for more informations!

New assets are now supported :
- Aseprite files
- Data files (.txt, .json, .xml, ...)
  
You can now attach a callback to a file by listening the `Updated` event. The callback will be invoked right after the file has been reloaded.

## Disclaimer

> Things you have to know is the library watch updates of your content directory files. But when you are using Visual Studio to update a specific file (for example, double-clicking on an Effect file will open it inside Visual Studio), and when you are modiying then saving the file, Visual Studio create a temporary file which creating issues with the library.  
I don't know if I can improve the library to avoid this behavior, even using Visual Studio, but at this time, you should avoid it and use Visual Studio Code for example which is the code editor I tested during the library development

## Getting started

Firstly, **initialize** the reloader inside the `Initialize` function:
```csharp
protected override void Initialize()
{
    Reloader.Initialize(Content, GraphicsDevice, Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform.DesktopGL);

    base.Initialize();
}
```
Congratulations! The Hot-Reloader is now setup!  

## Ignore files

In some cases, you don't need to watch some files extensions. You can do so by using a simple function:
```csharp
protected override void Initialize()
{
    // Right after the Reloader.Initialize(...)
    Reloader.Ignore(AssetType.Model, AssetType.Song);
}
```
You can put on the `Ignore` function as many AssetType as you want and all these types will be ignored if they are listened by the Reloader.  

## Access a single file

Sometimes, you'll need to retrieve a single asset. Depending on the type of the asset, you will have access to it inside the `AssetsManager` class.

```csharp
// Access to a Texture2D
AssetsManager.Textures["path/name_of_my_texture"];
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
    Reloader.OnUpdate("path/asset_name", (object sender, FileSystemEventArgs args) => {
        Console.WriteLine("The path/asset_name file has been updated");
    });
}
```

## Assets usage

If you want draw a texture or an Aseprite sprite, you can use a shortcut of the library which will render the asset if the asset is available:
```csharp
AssetsManager.Render2D("path/asset_name", position, source, spriteBatch, graphicsDevice, rotation, scale);
```
You can do the same with a Model:
```csharp
AssetsManager.Render3D("path/asset_name", world, view, projection);
```
And with a sound or a song:
```csharp
AssetsManager.Play("path/asset_name", volume);
```

> Note that these function are basic and do not use all the features you should want to use.
> In that case, you can access the asset via the dictionaries as I mentioned them in the `Access a single file` section.

## Roadmap

I'm always looking to improve this library.  
If you need a feature which is not covered, please send me a message!

## Contribute

All the contribution are welcome.  
If you want to help me improve the library, please send me a message!

## Credits
- MonoGame (under Ms-PL licence): https://github.com/MonoGame/MonoGame 
- MonoGame.Aseprite (under MIT licence): https://github.com/AristurtleDev/monogame-aseprite

## Licence

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