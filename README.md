# VRCrypt

Considering using `rust` for encryption process as C# is easy to be disassembled and replaced at runtime.

## Prerequisites

- Unity Editor version: `2022.3.22f1`

### Update paths

- in [VRCrypt.csproj](./VRCrypt.csproj)
- [link.sh](./scripts/link.sh)

### Link Dlls & Output Directory

`make link`

### Link FFI

- Update reference to `vrcrypt_lib.cs` in VRCrypt.csproj

## Debug Build

- `dotnet build`

### Build & Replace

- `make build`

> A rebuild is required after each rust build

## Release build

- Update lib reference in csproj

## TODOs

- Encrypt/Verify at build stage
- Encrypt GameObjects
- Process fbx & meshes in rust?
  - https://github.com/I3ck/rust-3d
  - https://github.com/lo48576/fbxcel
  - https://github.com/lo48576/fbxcel-dom
  - https://github.com/lo48576/fbx-viewer

## Single DLL

Embedded Resource Approach

```cs
public class DllLoader
{
    static DllLoader()
    {
        // Extract the Rust DLL from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var rustDllBytes = GetResourceBytes("YourNamespace.rust_dll.dll");
        var tempPath = Path.Combine(Path.GetTempPath(), "rust_dll.dll");

        // Write to temp directory if doesn't exist
        if (!File.Exists(tempPath))
        {
            File.WriteAllBytes(tempPath, rustDllBytes);
        }
    }

    private static byte[] GetResourceBytes(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null) throw new Exception($"Resource {resourceName} not found");
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}

```

Here's how to implement this approach:

1. Add your Rust DLL as an embedded resource in your C# project

- Add the Rust DLL to your C# project
- Set its Build Action to "Embedded Resource"

2. Create a loader class that extracts the Rust DLL at runtime

- The loader extracts the Rust DLL to a temporary location
- Your P/Invoke calls will work with the extracted DLL

3. In your project file (.csproj)

```csproj
<ItemGroup>
    <EmbeddedResource Include="path\to\rust_dll.dll" />
</ItemGroup>
```

### Advantages:

- Single DLL distribution
- Works cross-platform
- No special tools required

### Disadvantages:

- The Rust DLL is extracted at runtime
- Needs write permissions to temp directory
- Slightly larger file size
