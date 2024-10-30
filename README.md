# VRCrypt

## Prepare

- Unity Editor version: `2022.3.22f1`

### Link Dlls

- `ln -s <Unity Editor>\Data\Managed\UnityEditor.dll refs\UnityEditor.dll`
- `ln -s <Unity Editor>\Data\Managed\UnityEngine.dll refs\UnityEngine.dll`
- `ln -s <Project>\Packages\com.vrchat.avatars\Runtime\VRCSDK\Plugins\VRCSDK3A.dll refs\VRCSDK3A.dll`
- `ln -s <Project>\Packages\com.vrchat.base\Runtime\VRCSDK\Plugins\VRCSDKBase.dll refs\VRCSDKBase.dll`

> Or just do `sh link.sh`

### Link FFI

- Update reference to `vrcrypt_lib.cs` in VRCrypt.csproj

### Link Output Directory

- `mkdir -p <Project>\Assets/VRCrypt`
- `ln -s <Project>\Assets\VRCrypt out`

## Debug Build

- `dotnet build`

### Build & Replace

- `b.bat`

> A rebuild is required after each rust build

## Release build

- change lib reference in csproj

## TODOs

- encrypt gameobjects
- Process fbx & meshes in rust
  - https://github.com/I3ck/rust-3d
  - https://github.com/lo48576/fbxcel
  - https://github.com/lo48576/fbxcel-dom
  - https://github.com/lo48576/fbx-viewer
